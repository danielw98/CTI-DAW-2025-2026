import { promises as fs } from "node:fs";
import path from "node:path";
import matter from "gray-matter";
import MarkdownIt from "markdown-it";
import markdownItAnchor from "markdown-it-anchor";
import hljs from "highlight.js";

const rootDir = process.cwd();
const configPath = path.join(rootDir, "site.config.json");
const config = JSON.parse(await fs.readFile(configPath, "utf-8"));

const distDir = path.join(rootDir, "dist");
const contentDir = path.join(rootDir, config.contentDir ?? "content/labs");
const downloadsDir = path.join(rootDir, config.downloadsDir ?? "public/downloads");
const assetsSourceDir = path.join(rootDir, "src/assets");

function normalizeBasePath(inputBasePath = "/") {
  const cleaned = inputBasePath.trim();
  if (!cleaned || cleaned === "/") {
    return "/";
  }
  return `/${cleaned.replace(/^\/+|\/+$/g, "")}/`;
}

const basePath = normalizeBasePath(config.basePath ?? "/");

function withBase(urlPath = "/") {
  if (/^https?:\/\//i.test(urlPath)) {
    return urlPath;
  }

  const trimmed = urlPath.replace(/^\/+/, "");
  if (!trimmed) {
    return basePath;
  }

  return `${basePath}${trimmed}`;
}

const md = new MarkdownIt({
  html: true,
  linkify: true,
  typographer: true,
  highlight(code, lang) {
    if (lang && hljs.getLanguage(lang)) {
      const highlighted = hljs.highlight(code, { language: lang, ignoreIllegals: true }).value;
      return `<pre><code class="hljs language-${lang}">${highlighted}</code></pre>`;
    }

    const escaped = md.utils.escapeHtml(code);
    return `<pre><code class="hljs">${escaped}</code></pre>`;
  }
});

md.use(markdownItAnchor, {
  permalink: markdownItAnchor.permalink.headerLink()
});

async function pathExists(targetPath) {
  try {
    await fs.access(targetPath);
    return true;
  } catch {
    return false;
  }
}

function isWindowsLockError(error) {
  return error?.code === "EPERM" || error?.code === "EBUSY" || error?.code === "ENOTEMPTY";
}

async function sleep(ms) {
  await new Promise((resolve) => setTimeout(resolve, ms));
}

async function ensureCleanDir(targetPath) {
  try {
    await fs.rm(targetPath, { recursive: true, force: true });
  } catch (error) {
    if (!isWindowsLockError(error)) {
      throw error;
    }

    console.warn(`Nu am putut curata complet ${targetPath} (${error.code}). Continui cu suprascriere in-place.`);
  }

  await fs.mkdir(targetPath, { recursive: true });
}

async function writeFileWithRetry(fullPath, content, retries = 3) {
  for (let attempt = 1; attempt <= retries; attempt += 1) {
    try {
      await fs.writeFile(fullPath, content, "utf-8");
      return true;
    } catch (error) {
      if (!isWindowsLockError(error) || attempt === retries) {
        if (isWindowsLockError(error)) {
          console.warn(`Skip write (locked): ${fullPath} (${error.code})`);
          return false;
        }
        throw error;
      }
      await sleep(120 * attempt);
    }
  }

  return false;
}

async function copyFileWithRetry(sourcePath, destinationPath, retries = 3) {
  for (let attempt = 1; attempt <= retries; attempt += 1) {
    try {
      await fs.copyFile(sourcePath, destinationPath);
      return true;
    } catch (error) {
      if (!isWindowsLockError(error) || attempt === retries) {
        if (isWindowsLockError(error)) {
          console.warn(`Skip copy (locked): ${destinationPath} (${error.code})`);
          return false;
        }
        throw error;
      }
      await sleep(120 * attempt);
    }
  }

  return false;
}

async function resolveIncludes(content, currentDir, includeStack = []) {
  const includeRegex = /^!INCLUDE\s+"([^"]+)"\s*$/gm;
  let result = content;
  let match;

  while ((match = includeRegex.exec(result)) !== null) {
    const includeRawPath = match[1].trim();
    const includeFullPath = path.resolve(currentDir, includeRawPath);

    if (includeStack.includes(includeFullPath)) {
      throw new Error(`Include circular detectat: ${[...includeStack, includeFullPath].join(" -> ")}`);
    }

    if (!(await pathExists(includeFullPath))) {
      throw new Error(`Fisierul inclus nu exista: ${includeFullPath}`);
    }

    const includeSource = await fs.readFile(includeFullPath, "utf-8");
    const includeResolved = await resolveIncludes(
      includeSource,
      path.dirname(includeFullPath),
      [...includeStack, includeFullPath]
    );

    result = result.slice(0, match.index) + includeResolved + result.slice(match.index + match[0].length);
    includeRegex.lastIndex = 0;
  }

  return result;
}

function slugify(input) {
  return input
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .trim();
}

function stripMarkdown(markdown) {
  return markdown
    .replace(/```[\s\S]*?```/g, "")
    .replace(/`[^`]*`/g, "")
    .replace(/!\[[^\]]*\]\([^)]*\)/g, "")
    .replace(/\[[^\]]*\]\([^)]*\)/g, "")
    .replace(/[>#*_~-]/g, "")
    .replace(/\n+/g, " ")
    .trim();
}

function extractTitle(rawContent, fallback) {
  const match = rawContent.match(/^#\s+(.+)$/m);
  if (match?.[1]) {
    return match[1].trim();
  }
  return fallback;
}

function extractExcerpt(rawContent, maxLen = 180) {
  const text = stripMarkdown(rawContent);
  if (text.length <= maxLen) {
    return text;
  }
  return `${text.slice(0, maxLen - 3)}...`;
}

function normalizeAssetPath(rawPath = "") {
  const trimmed = rawPath.trim().replace(/\\/g, "/");
  const withoutQuotes = trimmed.replace(/^['"]|['"]$/g, "");
  const withoutRelativePrefix = withoutQuotes.replace(/^\.\//, "");

  const marker = "/content/labs/";
  const markerIndex = withoutRelativePrefix.toLowerCase().indexOf(marker);
  if (markerIndex >= 0) {
    return withoutRelativePrefix.slice(markerIndex + marker.length).replace(/^\/+/, "");
  }

  const contentPrefix = "content/labs/";
  if (withoutRelativePrefix.toLowerCase().startsWith(contentPrefix)) {
    return withoutRelativePrefix.slice(contentPrefix.length).replace(/^\/+/, "");
  }

  return withoutRelativePrefix.replace(/^\/+/, "");
}

function collectAssetPathsFromMarkdown(markdown) {
  const assets = new Set();

  const mdImgRegex = /!\[[^\]]*\]\(([^)\s]+)(?:\s+"[^"]*")?\)/g;
  let match;
  while ((match = mdImgRegex.exec(markdown)) !== null) {
    const candidate = match[1];
    if (/^(https?:|data:|\/|#)/i.test(candidate)) {
      continue;
    }
    assets.add(normalizeAssetPath(candidate));
  }

  const htmlImgRegex = /<img[^>]*\ssrc="([^"]+)"[^>]*>/gi;
  while ((match = htmlImgRegex.exec(markdown)) !== null) {
    const candidate = match[1];
    if (/^(https?:|data:|\/|#)/i.test(candidate)) {
      continue;
    }
    assets.add(normalizeAssetPath(candidate));
  }

  return [...assets].filter(Boolean);
}

function rewriteAssetUrlsInMarkdown(markdown, labSlug) {
  const rewritePath = (sourcePath) => withBase(`media/${labSlug}/${normalizeAssetPath(sourcePath)}`);

  const rewrittenMarkdownImages = markdown.replace(
    /(!\[[^\]]*\]\()([^)\s]+)([^)]*\))/g,
    (full, before, imagePath, after) => {
      if (/^(https?:|data:|\/|#)/i.test(imagePath)) {
        return full;
      }
      return `${before}${rewritePath(imagePath)}${after}`;
    }
  );

  return rewrittenMarkdownImages.replace(
    /(<img[^>]*\ssrc=")([^"]+)("[^>]*>)/gi,
    (full, before, imagePath, after) => {
      if (/^(https?:|data:|\/|#)/i.test(imagePath)) {
        return full;
      }
      return `${before}${rewritePath(imagePath)}${after}`;
    }
  );
}

function splitLabIntoParts(markdown) {
  const matches = [...markdown.matchAll(/^#\s*Parte\s+\d+.*$/gim)];
  if (matches.length === 0) {
    return [markdown.trim()];
  }

  const starts = matches.map((entry) => entry.index ?? 0);
  const parts = [];
  const prefix = markdown.slice(0, starts[0]).trim();

  for (let i = 0; i < starts.length; i += 1) {
    const start = starts[i];
    const end = i + 1 < starts.length ? starts[i + 1] : markdown.length;
    let chunk = markdown.slice(start, end).trim();

    if (i === 0 && prefix) {
      chunk = `${prefix}\n\n${chunk}`;
    }

    parts.push(chunk);
  }

  return parts.length > 0 ? parts : [markdown.trim()];
}

function renderPartPager(lab, currentPart, totalParts) {
  if (totalParts <= 1) {
    return "";
  }

  const pageLinks = Array.from({ length: totalParts }, (_, index) => {
    const partNumber = index + 1;
    const activeClass = partNumber === currentPart ? " active" : "";
    return `<a class="part-link${activeClass}" href="${withBase(`labs/${lab.slug}/${partNumber}/`)}">${partNumber}</a>`;
  }).join("");

  const previousLink = currentPart > 1
    ? `<a class="part-nav" href="${withBase(`labs/${lab.slug}/${currentPart - 1}/`)}">Anterior</a>`
    : '<span class="part-nav disabled">Anterior</span>';

  const nextLink = currentPart < totalParts
    ? `<a class="part-nav" href="${withBase(`labs/${lab.slug}/${currentPart + 1}/`)}">Urmator</a>`
    : '<span class="part-nav disabled">Urmator</span>';

  return `<nav class="part-pagination" aria-label="Paginare laborator">
  ${previousLink}
  <div class="part-pages">${pageLinks}</div>
  ${nextLink}
</nav>`;
}

function htmlPageTemplate({ title, description, body, pageClass = "" }) {
  const pageTitle = `${title} | ${config.siteTitle}`;
  return `<!doctype html>
<html lang="ro">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>${pageTitle}</title>
    <meta name="description" content="${description}" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Source+Sans+3:wght@400;500;700&family=Merriweather:ital,wght@0,400;0,700;1,400&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="${withBase("assets/site.css")}" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.11.1/styles/github-dark.min.css" crossorigin="anonymous" referrerpolicy="no-referrer" />
  </head>
  <body class="${pageClass}">
    ${body}
    <script src="${withBase("assets/site.js")}" defer></script>
  </body>
</html>`;
}

function shellLayout({
  heroTitle,
  heroSubtitle = "",
  innerHtml,
  showSearch = false,
  searchInHeader = false,
  hideHero = false
}) {
  const subtitleMarkup = heroSubtitle ? `<p>${heroSubtitle}</p>` : "";
  const searchMarkup = showSearch
    ? '<label class="search-wrap"><input id="searchInput" type="search" placeholder="Cauta laborator sau cuvant-cheie..." /><span>Search</span></label>'
    : "";

  const heroMarkup = hideHero
    ? ""
    : `<section class="hero">
    <h1>${heroTitle}</h1>
    ${subtitleMarkup}
    ${searchInHeader ? "" : searchMarkup}
  </section>`;

  const headerSearchMarkup = searchInHeader
    ? searchMarkup.replace('class="search-wrap"', 'class="search-wrap header-search"')
    : "";

  return `<div class="bg-shape bg-shape-1"></div>
<div class="bg-shape bg-shape-2"></div>
<header class="site-header">
  <div>
    <a class="brand" href="${withBase("/")}">${config.siteTitle}</a>
    <p class="subtitle">${config.siteSubtitle}</p>
  </div>
  <div class="header-tools">
    <nav class="top-nav">
      <a href="${withBase("/")}">Acasa</a>
      <a href="${withBase("labs/")}">Laboratoare</a>
      <a href="${config.githubRepoUrl}" target="_blank" rel="noreferrer">GitHub</a>
    </nav>
    ${headerSearchMarkup}
  </div>
</header>
<main class="main-shell">
  ${heroMarkup}
  ${innerHtml}
</main>
<footer class="site-footer">
  <p>DAW 2025-2026 · suport de laborator.</p>
</footer>`;
}

async function readLabFiles() {
  const allFiles = await fs.readdir(contentDir, { withFileTypes: true });
  const mdFiles = allFiles.filter((entry) => entry.isFile() && entry.name.toLowerCase().endsWith(".md"));

  const labs = [];
  for (const file of mdFiles) {
    const fullPath = path.join(contentDir, file.name);
    const source = await fs.readFile(fullPath, "utf-8");
    const parsed = matter(source);
    const resolvedContent = await resolveIncludes(parsed.content, path.dirname(fullPath));

    const fallbackTitle = file.name.replace(/\.md$/i, "");
    const title = parsed.data.title?.toString().trim() || extractTitle(resolvedContent, fallbackTitle);
    const slug = parsed.data.slug?.toString().trim() || slugify(file.name.replace(/\.md$/i, ""));
    const order = Number(parsed.data.order ?? Number.MAX_SAFE_INTEGER);
    const excerpt = parsed.data.excerpt?.toString().trim() || extractExcerpt(resolvedContent);
    const downloadUrl = parsed.data.downloadUrl?.toString().trim() || "";
    const repoUrl = parsed.data.repoUrl?.toString().trim() || config.githubRepoUrl;

    const featured = parsed.data.featured === true;
    const youtubeId = parsed.data.youtubeId?.toString().trim() || "";
    const assetPaths = collectAssetPathsFromMarkdown(resolvedContent);
    const rewrittenContent = rewriteAssetUrlsInMarkdown(resolvedContent, slug);
    const parts = splitLabIntoParts(rewrittenContent).map((partMarkdown, index) => ({
      index: index + 1,
      markdown: partMarkdown,
      html: md.render(partMarkdown)
    }));

    labs.push({
      title,
      slug,
      order,
      excerpt,
      downloadUrl,
      repoUrl,
      featured,
      youtubeId,
      html: md.render(rewrittenContent),
      parts,
      assets: assetPaths,
      markdownFile: file.name
    });
  }

  labs.sort((a, b) => {
    if (a.order !== b.order) {
      return a.order - b.order;
    }
    return a.title.localeCompare(b.title, "ro");
  });

  return labs;
}

function renderFeaturedCard(lab) {
  const actions = [
    `<a class="btn btn-primary btn-lg" href="${withBase(`labs/${lab.slug}/1/`)}">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" style="vertical-align:-2px;margin-right:6px"><polygon points="5,3 19,12 5,21"/></svg>Deschide prelegerea</a>`,
    lab.downloadUrl ? `<a class="btn btn-ghost" href="${withBase(lab.downloadUrl)}">Download proiect demo</a>` : ""
  ].filter(Boolean).join("");

  const thumbnail = lab.youtubeId
    ? `<a class="featured-thumb" href="${withBase(`labs/${lab.slug}/1/`)}">
        <img src="https://img.youtube.com/vi/${lab.youtubeId}/maxresdefault.jpg" alt="Video thumbnail" loading="lazy" />
        <span class="play-overlay"><svg width="48" height="48" viewBox="0 0 24 24" fill="#fff"><polygon points="5,3 19,12 5,21"/></svg></span>
      </a>`
    : "";

  return `<article class="featured-card">
  <div class="featured-body">
    <div class="featured-text">
      <div class="featured-badge">Eveniment special</div>
      <h2 class="featured-title">${lab.title}</h2>
      <p class="featured-excerpt">${lab.excerpt}</p>
      <div class="card-actions">${actions}</div>
    </div>
    ${thumbnail}
  </div>
</article>`;
}

function extractLabNumber(title) {
  const match = title.match(/(?:lab(?:orator)?)\s*(\d+)/i);
  return match ? match[1].padStart(2, "0") : null;
}

function renderLabCard(lab) {
  const downloadLabel = (() => {
    const url = (lab.downloadUrl || "").toLowerCase();
    if (!url) {
      return "";
    }
    if (url.endsWith(".zip")) {
      return "Download start";
    }
    if (url.endsWith(".pdf")) {
      return "Suport PDF";
    }
    return "Descarca";
  })();

  const labNum = extractLabNumber(lab.title);
  const badge = labNum ? `<span class="lab-badge">${labNum}</span>` : "";

  const actions = [
    `<a class="btn btn-primary" href="${withBase(`labs/${lab.slug}/1/`)}">Deschide</a>`,
    lab.downloadUrl ? `<a class="btn btn-ghost" href="${withBase(lab.downloadUrl)}">${downloadLabel}</a>` : ""
  ].filter(Boolean).join("");

  return `<article class="lab-card" data-search="${(lab.title + " " + lab.excerpt).toLowerCase()}">
  <div class="lab-card-header">${badge}<h3>${lab.title}</h3></div>
  <p>${lab.excerpt}</p>
  <div class="card-actions">${actions}</div>
</article>`;
}

async function writePage(relativePath, html) {
  const fullPath = path.join(distDir, relativePath);
  const dir = path.dirname(fullPath);
  await fs.mkdir(dir, { recursive: true });
  await writeFileWithRetry(fullPath, html);
}

async function copyPublicAssets() {
  const distAssetsDir = path.join(distDir, "assets");
  await fs.mkdir(distAssetsDir, { recursive: true });

  const assetFiles = await fs.readdir(assetsSourceDir);
  for (const file of assetFiles) {
    await copyFileWithRetry(path.join(assetsSourceDir, file), path.join(distAssetsDir, file));
  }

  if (await pathExists(downloadsDir)) {
    const targetDownloads = path.join(distDir, "downloads");
    await fs.mkdir(targetDownloads, { recursive: true });
    const downloadFiles = await fs.readdir(downloadsDir);
    for (const file of downloadFiles) {
      await copyFileWithRetry(path.join(downloadsDir, file), path.join(targetDownloads, file));
    }
  }
}

async function copyLabMediaAssets(labs) {
  for (const lab of labs) {
    for (const assetRelativePath of lab.assets ?? []) {
      const sourcePath = path.join(contentDir, ...assetRelativePath.split("/"));
      if (!(await pathExists(sourcePath))) {
        continue;
      }

      const targetPath = path.join(distDir, "media", lab.slug, ...assetRelativePath.split("/"));
      await fs.mkdir(path.dirname(targetPath), { recursive: true });
      await copyFileWithRetry(sourcePath, targetPath);
    }
  }
}

async function build() {
  if (!(await pathExists(contentDir))) {
    throw new Error(`Nu exista directorul de continut: ${contentDir}`);
  }

  await ensureCleanDir(distDir);

  const labs = await readLabFiles();
  if (labs.length === 0) {
    throw new Error("Nu exista fisiere markdown in content/labs. Adauga cel putin un laborator.");
  }

  const featuredLabs = labs.filter((lab) => lab.featured);
  const regularLabs = labs.filter((lab) => !lab.featured);
  const listHtml = labs.map((lab) => renderLabCard(lab)).join("\n");

  for (const lab of labs) {
    const totalParts = lab.parts.length;

    for (const part of lab.parts) {
      const pager = renderPartPager(lab, part.index, totalParts);
      const labContent = shellLayout({
        heroTitle: lab.title,
        heroSubtitle: totalParts > 1
          ? `Partea ${part.index} din ${totalParts}. Navigarea publica foloseste doar numerotare.`
          : "",
        innerHtml: `${pager}<article class="lab-content markdown-body">${part.html}</article>${pager}`
      });

      await writePage(
        `labs/${lab.slug}/${part.index}/index.html`,
        htmlPageTemplate({
          title: `${lab.title} - ${part.index}`,
          description: lab.excerpt,
          body: labContent,
          pageClass: "lab-page"
        })
      );
    }

    const firstPart = lab.parts[0];
    const firstPager = renderPartPager(lab, 1, totalParts);
    const compatibilityContent = shellLayout({
      heroTitle: lab.title,
      heroSubtitle: totalParts > 1 ? `Partea 1 din ${totalParts}.` : "",
      innerHtml: `${firstPager}<article class="lab-content markdown-body">${firstPart.html}</article>${firstPager}`
    });

    await writePage(
      `labs/${lab.slug}/index.html`,
      htmlPageTemplate({
        title: lab.title,
        description: lab.excerpt,
        body: compatibilityContent,
        pageClass: "lab-page"
      })
    );
  }

  const labsPageBody = shellLayout({
    heroTitle: "Laboratoare",
    heroSubtitle: "Lista este generata automat din fisierele markdown.",
    showSearch: true,
    innerHtml: `<section id="labsGrid" class="labs-grid">${listHtml}</section>`
  });

  await writePage(
    "labs/index.html",
    htmlPageTemplate({
      title: "Laboratoare",
      description: "Lista completa de laboratoare DAW",
      body: labsPageBody,
      pageClass: "labs-page"
    })
  );

  const featuredHtml = featuredLabs.length > 0
    ? `<div class="landing-featured">${featuredLabs.map(renderFeaturedCard).join("\n")}</div>`
    : "";

  const regularLabsHtml = regularLabs.length > 0
    ? `<div class="landing-labs-section">
        <p class="section-label">Laboratoare</p>
        <section id="labsGrid" class="labs-grid">${regularLabs.map(renderLabCard).join("\n")}</section>
      </div>`
    : "";

  const landingBody = shellLayout({
    heroTitle: config.siteTitle,
    heroSubtitle: config.siteSubtitle,
    showSearch: true,
    searchInHeader: true,
    hideHero: true,
    innerHtml: featuredHtml + regularLabsHtml
  });

  await writePage(
    "index.html",
    htmlPageTemplate({
      title: "Landing",
      description: config.description,
      body: landingBody,
      pageClass: "landing-page"
    })
  );

  await writePage(
    "labs.json",
    JSON.stringify(
      labs.map((lab) => ({
        title: lab.title,
        slug: lab.slug,
        excerpt: lab.excerpt,
        order: lab.order
      })),
      null,
      2
    )
  );

  await writePage(".nojekyll", "");

  await copyPublicAssets();
  await copyLabMediaAssets(labs);

  console.log(`Build finalizat. Pagini generate: ${labs.length} laboratoare.`);
}

build().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
