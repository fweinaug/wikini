const sectionMargin = 50;

let sections = [];
let timer;

function registerScrollEvents() {
  window.addEventListener("resize", function (event) {
    _refresh();
  });

  window.addEventListener("scroll", function (event) {
    _updateScrollPosition(this.scrollX, this.scrollY);
  });

  _updateSections(true);
  _updateScrollPosition(window.scrollX, window.scrollY);
}

function scrollToTop() {
  document.body.scrollTop = 0;
  document.documentElement.scrollTop = 0;
}

function scrollToSection(id) {
  const element = document.getElementById(id);

  if (element != null) {
    let parent = element.parentElement;

    while (parent != null && parent.tagName !== "SECTION") {
      parent = parent.parentElement;
    }

    if (parent != null && !parent.classList.contains("open-block")) {
      parent.previousSibling.click();

      _refresh();
    }

    _scrollToElement(element);
  }
}

function scrollToElement(id) {
  const element = document.getElementById(id);

  if (element != null) {
    _scrollToElement(element);
  }
}

function _scrollToElement(element) {

  const rect = element.getBoundingClientRect();
  const margin = document.body.style.marginTop.substring(0, document.body.style.marginTop.length - 2);

  window.scrollTo(0, rect.top + window.scrollY - 20 - margin);
}

function _refresh() {
  _updateSections(false);
  _updateScrollPosition(window.scrollX, window.scrollY);
}

function _updateSections(registerEvents) {
  sections = [];

  document.querySelectorAll(".mw-headline").forEach(function (e) {
    if (registerEvents && e.parentElement.classList.contains("section-heading")) {
      e.parentElement.addEventListener("click", function (event) {
        clearTimeout(timer);
        timer = setTimeout(_refresh, 150);
      });
    }

    const top = e.parentElement.offsetTop;

    if (top > 0) {
      sections.push({ id: e.id, top: top, element: e });
    }
  });
}

function _updateScrollPosition(left, top) {
  const element = document.documentElement;
  const width = element.scrollWidth - element.clientWidth;
  const height = element.scrollHeight - element.clientHeight;

  const current = sections.length - [...sections].reverse().findIndex((section) => top >= section.top - sectionMargin) - 1;
  const sectionId = current >= 0 && current < sections.length ? sections[current].id.replace(/'/g, "\\'") : "";

  window.external.notify(`{ 'Message': 'Scroll', 'X': ${left}, 'Y': ${top}, 'Width': ${width}, 'Height': ${height}, 'Text': '${sectionId}' }`);
}
