(function () {
    try {
        if (typeof window !== 'undefined' && window.feather && typeof window.feather.replace === 'function') {
            window.feather.replace();
        }
    } catch (e) {
        // ignore
    }
})();
