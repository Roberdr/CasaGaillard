(function () {
    document.querySelectorAll('[data-foto-pies]').forEach(function (root) {
        var fileInput = root.querySelector('input[type="file"]');
        var captions = root.querySelector('[data-foto-pies-lista]');
        var field = root.dataset.campo;
        if (!fileInput || !captions || !field) return;

        fileInput.addEventListener('change', function () {
            captions.replaceChildren();
            Array.from(fileInput.files || []).forEach(function (file, index) {
                var wrapper = document.createElement('div');
                wrapper.className = 'mb-2';
                var label = document.createElement('label');
                label.className = 'form-label small mb-1';
                label.textContent = 'Pie de foto para ' + file.name;
                var input = document.createElement('input');
                input.type = 'text';
                input.name = field + '[' + index + ']';
                input.maxLength = 500;
                input.className = 'form-control';
                input.placeholder = 'Nombre u observación (opcional)';
                wrapper.append(label, input);
                captions.appendChild(wrapper);
            });
        });
    });
})();
