function attachCopyButtons() {
  const blocks = document.querySelectorAll("pre > code");
  blocks.forEach((codeBlock) => {
    const pre = codeBlock.parentElement;
    if (!pre || pre.querySelector(".copy-button")) {
      return;
    }

    const button = document.createElement("button");
    button.type = "button";
    button.className = "copy-button";
    button.textContent = "Copy";

    const fallbackCopy = (text) => {
      const area = document.createElement("textarea");
      area.value = text;
      area.setAttribute("readonly", "");
      area.style.position = "fixed";
      area.style.opacity = "0";
      area.style.pointerEvents = "none";
      document.body.appendChild(area);
      area.focus();
      area.select();

      let copied = false;
      try {
        copied = document.execCommand("copy");
      } catch {
        copied = false;
      }

      document.body.removeChild(area);
      return copied;
    };

    button.addEventListener("click", async () => {
      const text = codeBlock.textContent || "";
      try {
        if (navigator.clipboard && window.isSecureContext) {
          await navigator.clipboard.writeText(text);
        } else if (!fallbackCopy(text)) {
          throw new Error("Clipboard unavailable");
        }

        button.textContent = "Copied";
        button.classList.add("success");
        window.setTimeout(() => {
          button.textContent = "Copy";
          button.classList.remove("success");
        }, 1200);
      } catch {
        button.textContent = "Error";
      }
    });

    pre.appendChild(button);
  });
}

function attachSearch() {
  const searchInput = document.getElementById("searchInput");
  if (!searchInput) {
    return;
  }

  const cards = Array.from(document.querySelectorAll(".lab-card"));
  if (cards.length === 0) {
    return;
  }

  searchInput.addEventListener("input", () => {
    const query = searchInput.value.trim().toLowerCase();
    cards.forEach((card) => {
      const haystack = (card.getAttribute("data-search") || "").toLowerCase();
      const visible = !query || haystack.includes(query);
      card.style.display = visible ? "block" : "none";
    });
  });
}

function revealCards() {
  const cards = document.querySelectorAll(".lab-card");
  cards.forEach((card, index) => {
    card.style.animationDelay = `${index * 60}ms`;
  });
}

document.addEventListener("DOMContentLoaded", () => {
  attachCopyButtons();
  attachSearch();
  revealCards();
});
