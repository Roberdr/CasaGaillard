(function () {
    document.querySelectorAll('[data-material-rapido]').forEach(function (root) {
        var select = root.querySelector('select');
        var input = root.querySelector('[data-material-nombre]');
        var button = root.querySelector('[data-material-crear]');
        var status = root.querySelector('[data-material-estado]');
        var token = root.querySelector('input[name="__RequestVerificationToken"]');
        if (!select || !input || !button || !token) return;

        button.addEventListener('click', async function () {
            var nombre = input.value.trim();
            if (!nombre) { status.textContent = 'Escribe el nombre del material.'; input.focus(); return; }
            button.disabled = true;
            status.textContent = 'Guardando material…';
            try {
                var body = new URLSearchParams();
                body.set('nombre', nombre);
                body.set('__RequestVerificationToken', token.value);
                var response = await fetch(root.dataset.url, { method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' }, body: body });
                var result = await response.json();
                if (!response.ok) throw new Error(result.mensaje || 'No se pudo crear el material.');
                var option = Array.from(select.options).find(function (item) { return item.value === String(result.id); });
                if (!option) { option = new Option(result.nombre, result.id); select.add(option); }
                select.value = String(result.id);
                input.value = '';
                status.textContent = 'Material seleccionado.';
            } catch (error) {
                status.textContent = error.message || 'No se pudo crear el material.';
            } finally {
                button.disabled = false;
            }
        });
    });
})();
