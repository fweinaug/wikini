function registerContextmenuEvents() {
  document.oncontextmenu = function (e) {
    const target = e.target.closest("a");

    if (target) {
      _handleLinkContextmenu(target);
    }
  }
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
