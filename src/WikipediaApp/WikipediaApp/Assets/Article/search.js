let results = [];
let currentIndex = 0;

let markInstance = null;
let content = null;

function search(query) {
  if (!markInstance) {
    content = document.getElementById("content");
    markInstance = new Mark(content);
  }

  markInstance.unmark({
    done: function() {
      markInstance.mark(query, 
      {
        separateWordSearch: false,
        acrossElements: true,
        done: function() {
          results = content.getElementsByTagName("mark");

          currentIndex = 0;
          _jumpTo();

          window.external.notify(`{ 'Message': 'SearchResults', 'Number': ${results.length.toString()} }`);
        }
      });
    }
  });
}

function searchForward() {
  if (results.length) {
    if (++currentIndex >= results.length) {
      currentIndex = 0;
    }
    _jumpTo();
  }
}

function searchBackward() {
  if (results.length) {
    if (--currentIndex < 0) {
      currentIndex = results.length - 1;
    }
    _jumpTo();
  }
}

function _jumpTo() {
  const currentClass = "current";

  if (results.length) {
    for (let i = 0; i < results.length; i++) {
      results[i].classList.remove(currentClass);
    }

    const current = results.item(currentIndex);
    if (current) {
      current.classList.add(currentClass);

      _scrollIntoView(current);
    }
  }
}

function _scrollIntoView(target) {
  const rect = target.getBoundingClientRect();
  const margin = document.body.style.marginTop.substring(0, document.body.style.marginTop.length - 2);

  if (rect.bottom + 50 > window.innerHeight) {
    window.scrollBy(0, rect.bottom - window.innerHeight + 50);
  }
  if (rect.top < 0) {
    window.scrollBy(0, rect.top - margin - 20);
  }
}
