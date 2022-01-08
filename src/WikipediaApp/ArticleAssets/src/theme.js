function changeTheme(name) {
  if (name === 'theme-light') {
    document.documentElement.classList.replace('theme-dark', name);
    document.body.classList.replace('theme-dark', name);
  } else if (name === 'theme-dark') {
    document.documentElement.classList.replace('theme-light', name);
    document.body.classList.replace('theme-light', name);
  }
}
