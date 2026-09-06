// Scripts para la vista Details de Accesorios
(function ($) {
    $(function () {
        // abrir modal en la imagen clicada y mostrar slide correcto
        var $modal = $('#fotoModal');
        var $items = $('#fotosCarousel .carousel-item');

        function showModalAt(index) {
            index = Math.max(0, Math.min(index, $items.length - 1));
            $items.removeClass('active').eq(index).addClass('active');
            // Mostrar modal simple sin depender de plugin bootstrap
            $modal.addClass('show').css('display', 'block').attr('aria-hidden', 'false');
            if ($('.modal-backdrop').length === 0) {
                $('<div class="modal-backdrop fade show"></div>').appendTo(document.body);
            }
        }

        function hideModal() {
            $modal.removeClass('show').css('display', 'none').attr('aria-hidden', 'true');
            $('.modal-backdrop').remove();
        }

        $('.accesorio-fotos-galeria a[data-index]').on('click', function (ev) {
            ev.preventDefault();
            var index = parseInt($(this).attr('data-index'), 10) || 0;
            showModalAt(index);
        });

        // cerrar modal desde el footer/close button
        $modal.find('[data-dismiss="modal"]').on('click', function (e) {
            e.preventDefault();
            hideModal();
        });

        // navegación prev/next
        $modal.on('click', '.carousel-control-prev, .carousel-control-next', function (e) {
            e.preventDefault();
            var $current = $items.filter('.active');
            var idx = $items.index($current);
            if ($(this).hasClass('carousel-control-prev')) idx = idx - 1;
            else idx = idx + 1;
            if (idx < 0) idx = 0;
            if (idx >= $items.length) idx = $items.length - 1;
            showModalAt(idx);
        });

        // ESC para cerrar
        $(document).on('keydown', function (ev) {
            if (ev.key === 'Escape' || ev.keyCode === 27) {
                if ($modal.hasClass('show')) {
                    hideModal();
                }
            }
        });

        // confirmación al eliminar
        $('#deleteForm').on('submit', function (e) {
            if (!confirm('¿Confirma que desea eliminar este accesorio? Esta acción no se puede deshacer.')) {
                e.preventDefault();
            }
        });
    });
})(jQuery);
