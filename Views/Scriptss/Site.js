// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$(function () {
//    $("#loaderbody").addClass('hide');

//    $(document).bind('ajaxStart', function () {
//        $("#loaderbody").removeClass('hide');
//    }).bind('ajaxStop', function () {
//        $("#loaderbody").addClass('hide');
//    });
//});

function showInPopup (url, title) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
            // to make popup draggable
            $('.modal-dialog').draggable({
                handle: ".modal-header"
            });
        }
    })
}

$(function () {
    var $root = $('[data-stock-filter-root]');
    if (!$root.length) {
        return;
    }

    var $input = $('#stockFilter');
    var $cards = $('[data-stock-card]');
    var $counter = $('#stockCounter');
    var $clear = $('#stockClear');

    function normalize(value) {
        return (value || '').toString().toLowerCase();
    }

    function refreshCounter() {
        var visible = $cards.filter(':visible').length;
        $counter.text(visible + ' visibles');
    }

    function applyFilter() {
        var query = normalize($input.val()).trim();

        $cards.each(function () {
            var $card = $(this);
            var haystack = normalize($card.data('search') || $card.text());
            var match = !query || haystack.indexOf(query) !== -1;
            $card.toggle(match);
        });

        refreshCounter();
    }

    $input.on('input', applyFilter);
    $clear.on('click', function () {
        $input.val('');
        applyFilter();
        $input.trigger('focus');
    });

    applyFilter();
});

//jQueryAjaxPost = form => {
//    try {
//        $.ajax({
//            type: 'POST',
//            url: form.action,
//            data: new FormData(form),
//            contentType: false,
//            processData: false,
//            success: function (res) {
//                if (res.isValid) {
//                    $('#view-all').html(res.html)
//                    $('#form-modal .modal-body').html('');
//                    $('#form-modal .modal-title').html('');
//                    $('#form-modal').modal('hide');
//                }
//                else
//                    $('#form-modal .modal-body').html(res.html);
//            },
//            error: function (err) {
//                console.log(err)
//            }
//        })
//        //to prevent default form submit event
//        return false;
//    } catch (ex) {
//        console.log(ex)
//    }
//}

//jQueryAjaxDelete = form => {
//    if (confirm('Are you sure to delete this record ?')) {
//        try {
//            $.ajax({
//                type: 'POST',
//                url: form.action,
//                data: new FormData(form),
//                contentType: false,
//                processData: false,
//                success: function (res) {
//                    $('#view-all').html(res.html);
//                },
//                error: function (err) {
//                    console.log(err)
//                }
//            })
//        } catch (ex) {
//            console.log(ex)
//        }
//    }

//    //prevent default form submit event
//    return false;
//}
