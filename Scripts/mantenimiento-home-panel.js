(function () {
    function findClosestByClasses(el, classNames) {
        while (el) {
            if (el.nodeType === 1) {
                for (var i = 0; i < classNames.length; i++) {
                    if (el.classList && el.classList.contains(classNames[i])) {
                        return el;
                    }
                }
            }
            el = el.parentElement;
        }
        return null;
    }

    document.addEventListener('dblclick', function (event) {
        var target = event && event.target ? event.target : null;
        if (!target) {
            return;
        }

        var row = null;
        var tgt = target;
        if (tgt && typeof tgt.closest === 'function') {
            row = tgt.closest('.revision-cuba-panel-row, .revision-vehiculo-panel-row');
        } else {
            row = findClosestByClasses(target, ['revision-cuba-panel-row', 'revision-vehiculo-panel-row']);
        }

        if (!row) {
            if (tgt && typeof tgt.closest === 'function') {
                row = tgt.closest('.tarea-panel-row');
            } else {
                row = findClosestByClasses(target, ['tarea-panel-row']);
            }
        }

        if (!row) {
            return;
        }

        var url = row.getAttribute('data-revisiones-url') || row.getAttribute('data-tarea-url');
        if (!url) {
            return;
        }

        event.preventDefault();
        window.location.href = url;
    });
})();
