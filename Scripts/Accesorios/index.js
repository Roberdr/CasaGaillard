// Scripts para la página Index de Accesorios: calcular pageSize para evitar scroll y hacer filtros compactos
(function ($) {
    $(function () {
        function getParam(name) {
            var results = new RegExp('[?&]' + name + '=([^&#]*)').exec(window.location.href);
            return results ? decodeURIComponent(results[1].replace(/\+/g, ' ')) : null;
        }

        // Solo actuar si no hemos probado recientemente para evitar bucles
        if (sessionStorage.getItem('accesorios_index_resize_done')) return;

        // Esperar a que la tabla exista
        var $table = $('.public-table table, .table').first();
        if (!$table || !$table.length) return;

        // Calcular altura disponible para filas
        var tableTop = $table.offset().top;
        var viewportH = window.innerHeight || document.documentElement.clientHeight;
        // Reservar espacio para header, filtros y footer aproximado
        var reserved = 220; // ajuste empírico

        var available = Math.max(200, viewportH - tableTop - reserved);

        // Medir altura de una fila (thead+one row)
        var $firstRow = $table.find('tbody tr').first();
        var rowH = $firstRow.length ? $firstRow.outerHeight(true) : 40;
        var headerH = $table.find('thead').outerHeight(true) || 40;

        var rowsPerPage = Math.max(1, Math.floor((available - headerH) / Math.max(1, rowH)));

        var currentPageSize = parseInt(getParam('pageSize'), 10) || null;
        if (currentPageSize !== rowsPerPage) {
            // Reconstruir querystring preservando otros parámetros
            var url = new URL(window.location.href);
            url.searchParams.set('pageSize', rowsPerPage);
            url.searchParams.set('page', 1);
            sessionStorage.setItem('accesorios_index_resize_done', '1');
            window.location.replace(url.toString());
        }
    });
})(jQuery);
