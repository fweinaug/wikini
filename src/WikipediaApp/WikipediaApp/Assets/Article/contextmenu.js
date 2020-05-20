function registerContextmenuEvents() {
  var linkElements = document.getElementsByTagName('a');

  for (let i = 0; i < linkElements.length; i++) {
    linkElements[i].addEventListener('contextmenu', _handleContextmenu);
  }
}

function _handleContextmenu(e) {
  const href = e.target.getAttribute('href');
  if (!href || href.startsWith('#'))
    return;

  const bounds = e.target.getBoundingClientRect();
  const left = bounds.left;
  const top = bounds.top;
  const width = bounds.right - left;
  const height = bounds.bottom - top;

  const url = e.target.href;
  const title = e.target.title.replace(/'/g, "\\'");

  window.external.notify(`{ 'Message': 'Contextmenu', 'X': ${left}, 'Y': ${top}, 'Width': ${width}, 'Height': ${height}, 'Url': '${url}', 'Text': '${title}' }`);
}
