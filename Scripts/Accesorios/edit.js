// Scripts para la página Edit de Accesorios
(function ($) {
    $(function () {
        function toNumber(value) {
            if (!value) return 0;
            value = ('' + value).replace(',', '.');
            var num = parseFloat(value);
            return isNaN(num) ? 0 : num;
        }

        var $form = $('#accesorioForm');
        if ($form.length) {
            $form.on('submit', function () {
                $form.find('.campo-numerico').each(function () {
                    var val = $(this).val();
                    $(this).val(toNumber(val));
                });
            });
        }

        var $btn = $('#btnAddDetalle');
        if ($btn && $btn.length) {
            $btn.on('click', function () {
                var $tbody = $('.accesorio-detalles-table tbody');
                if (!$tbody.length) return;
                var $rows = $tbody.find('tr');
                var newIndex = $rows.length;
                var $lastRow = $rows.last();
                if (!$lastRow.length) return;
                var $newRow = $lastRow.clone();

                $newRow.find('input, select, textarea').each(function () {
                    var $c = $(this);
                    var name = $c.attr('name') || '';
                    if (name) {
                        $c.attr('name', name.replace(/Detalles\[\d+\]/g, 'Detalles[' + newIndex + ']'));
                    }
                    $c.removeAttr('id');
                    if ($c.is('select')) {
                        $c.prop('selectedIndex', 0);
                    } else if ($c.is(':checkbox') || $c.is(':radio')) {
                        $c.prop('checked', false);
                    } else if ($c.is('input[type=hidden]') && /\.ID$/.test(name)) {
                        $c.val('0');
                    } else {
                        $c.val('');
                    }
                });

                $tbody.append($newRow);
            });
        }
    });
})(jQuery);
