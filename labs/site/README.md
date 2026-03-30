# DAW Labs Site

Site static pentru laboratoarele DAW, pregatit pentru GitHub Pages.

## Ce ofera

- Landing page cu lista de laboratoare
- Pagini HTML randate din Markdown
- Buton de copiere pentru blocuri de cod
- Layout responsive
- Linkuri pentru download si repository principal
- Adaugare laboratoare noi fara modificari de cod (doar fisiere `.md`)

## Structura

- `content/labs/` - aici adaugi fisierele markdown (`lab06.md`, `lab07.md`, etc.)
- `public/downloads/` - aici pui arhivele pentru download (`lab06-start.zip`, etc.)
- `scripts/build.mjs` - generatorul static
- `dist/` - output-ul final pentru deploy

## Configurare GitHub Pages

In `site.config.json` setezi:

- `githubRepoUrl` catre repo-ul real
- `basePath`:
	- `/` daca ai custom domain pe root
	- `/<nume-repo>/` daca publici pe `https://<user>.github.io/<nume-repo>/`

## Frontmatter recomandat pentru fiecare laborator

```yaml
---
title: "Lab 06 - MVC, Async, Service, Repository"
order: 6
excerpt: "Refactorizare la MVC si introducerea pattern-urilor de arhitectura."
downloadUrl: "/downloads/lab06-start.zip"
repoUrl: "https://github.com/your-org/DAW-2025-2026/tree/main/Lab06_start"
---
```

Daca lipseste frontmatter, generatorul foloseste valori implicite.

## Compunere din subfisiere (sectiuni)

Generatorul suporta include-uri recursive in continutul markdown:

```md
!INCLUDE "./lab06/sections/00-intro.md"
!INCLUDE "./lab06/sections/01-parte1-mvc.md"
```

Reguli:

- calea este relativa la fisierul curent
- include-urile pot fi imbinate recursiv
- frontmatter ramane doar in fisierul principal (de ex. `lab06.md`)

## Comenzi

```bash
npm install
npm run build
```

## Deploy pe GitHub Pages

1. Rulezi `npm run build`.
2. Publici continutul din `dist/` pe branch-ul GitHub Pages.
3. Optional: configurezi Action de deploy pentru build automat.

## Cum adaugi un lab nou

1. Copiezi un nou markdown in `content/labs/`.
2. (Optional) Adaugi frontmatter cu `title`, `order`, `downloadUrl`.
3. Daca ai arhiva de start, o pui in `public/downloads/`.
4. Rulezi `npm run build`.

Atat. Nu trebuie sa modifici codul generatorului.
