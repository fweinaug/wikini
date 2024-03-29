﻿function registerContextmenuEvents() {
  document.oncontextmenu = function (e) {
    e.preventDefault();

    const selection = document.getSelection();

    if (selection.containsNode(e.target, true)) {
      _handleSelectionContextmenu(e, selection);
    } else {
      const target = e.target.closest("a");

      if (target) {
        _handleLinkContextmenu(target);
      } else {
        _handleContextmenu(e);
      }
    }
  }
}

function _handleSelectionContextmenu(event, selection) {
  const left = event.clientX;
  const top = event.clientY;
  const text = selection.toString().replace(/'/g, "\\'");

  window.external.notify(`{ 'Message': 'Contextmenu', 'X': ${left}, 'Y': ${top}, 'Text': '${text}' }`);
}

function _handleLinkContextmenu(target) {
  const bounds = target.getBoundingClientRect();
  const left = bounds.left;
  const top = bounds.top;
  const width = bounds.right - left;
  const height = bounds.bottom - top;

  const url = target.href;
  const title = target.title.replace(/'/g, "\\'");

  window.external.notify(`{ 'Message': 'Contextmenu', 'X': ${left}, 'Y': ${top}, 'Width': ${width}, 'Height': ${height}, 'Url': '${url}', 'Text': '${title}' }`);
}

function _handleContextmenu(event) {
  const left = event.clientX;
  const top = event.clientY;

  window.external.notify(`{ 'Message': 'Contextmenu', 'X': ${left}, 'Y': ${top} }`);
}
