(function () {
    const themeKey = 'themePreference';

    function getTheme() {
        return localStorage.getItem(themeKey) || 'light';
    }

    function setTheme(theme) {
        localStorage.setItem(themeKey, theme);
        if (theme === 'dark') {
            document.body.classList.add('dark-theme');
        } else {
            document.body.classList.remove('dark-theme');
        }
    }

    function toggleTheme() {
        const currentTheme = getTheme();
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        setTheme(newTheme);
        return newTheme;
    }

    // Apply theme on initial load
    document.addEventListener('DOMContentLoaded', () => {
        setTheme(getTheme());
    });

    window.themeManager = {
        getTheme,
        setTheme,
        toggleTheme
    };
})();
