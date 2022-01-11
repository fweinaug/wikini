function changeTheme(name) {
  if (name === 'theme-light') {
    document.documentElement.classList.replace('theme-dark', name);
    document.body.classList.replace('theme-dark', name);
  } else if (name === 'theme-dark') {
    document.documentElement.classList.replace('theme-light', name);
    document.body.classList.replace('theme-light', name);
  }
}

function changeFontSize(size) {
    document.documentElement.style.setProperty('--font-size', `${size}px`);
}

function changeTypeface(name) {
    if (name === 'sans-serif') {
        const element = document.querySelector('.serif');
        element.classList.replace('serif', name);
    } else if (name === 'serif') {
        const element = document.querySelector('.sans-serif');
        element.classList.replace('sans-serif', name);
    }
}
