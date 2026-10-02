(function(){
    /**
     * Externalized subfamilias loader to avoid inline TS analyzer issues in Razor views.
     * Expects select#FamiliaID and select#SubfamiliaID to exist. The Subfamilia select may have a
     * data-url attribute with the endpoint to fetch subfamilias for a familiaId. It may also have
     * data-selected with the preselected subfamilia id.
     */

    function query(sel) { return document.querySelector(sel); }
    var familia = /** @type {HTMLSelectElement|null} */ (query('#FamiliaID') || query('[name="FamiliaID"]'));
    var sub = /** @type {HTMLSelectElement|null} */ (document.getElementById('SubfamiliaID'));
    if (!familia || !sub) return;
    var status = document.getElementById('SubfamiliaStatus');

    function setStatus(message, isError) {
        if (!status) return;
        status.textContent = message || '';
        status.className = isError ? 'help-block text-danger' : 'help-block text-muted';
    }

    function clearOptions() { sub.options.length = 0; }
    function addEmpty() { var opt = document.createElement('option'); opt.value = ''; opt.text = '-- Sin subfamilia --'; sub.appendChild(opt); }

    /** @param {string|null} familiaId @param {string|null} selectValueToChoose */
    function loadSubfamilias(familiaId, selectValueToChoose) {
        clearOptions();
        sub.disabled = true;
        if (!familiaId) {
            addEmpty();
            sub.value = '';
            sub.disabled = false;
            setStatus('Selecciona una familia para ver sus subfamilias.');
            return;
        }
        setStatus('Cargando subfamilias...');
        var url = new URL(sub.getAttribute('data-url') || '/Mantenimiento/Accesorios/SubfamiliasPorFamilia', window.location.href);
        url.searchParams.set('familiaId', familiaId);
        fetch(url.toString(), { credentials: 'same-origin' })
            .then(function(r){ if (!r.ok) throw new Error('No se pudieron cargar las subfamilias.'); return r.json(); })
            .then(function(data){
                if (!Array.isArray(data)) throw new Error('Respuesta no válida al cargar subfamilias.');
                addEmpty();
                data.forEach(function(item){
                    var opt = document.createElement('option');
                    opt.value = item.id;
                    opt.text = item.text;
                    sub.appendChild(opt);
                });
                if (selectValueToChoose) sub.value = selectValueToChoose;
                setStatus(data.length ? '' : 'Esta familia todavía no tiene subfamilias.');
            }).catch(function(error){
                addEmpty();
                sub.value = '';
                setStatus(error.message || 'No se pudieron cargar las subfamilias.', true);
            }).finally(function(){ sub.disabled = false; });
    }

    familia.addEventListener('change', function(){ loadSubfamilias(familia.value, null); });

    var selectedFamilia = familia.value;
    var selectedSub = sub.getAttribute('data-selected') || sub.getAttribute('data_selected') || (sub.dataset ? sub.dataset.selected : null);
    if (selectedFamilia) {
        loadSubfamilias(selectedFamilia, selectedSub);
    } else {
        loadSubfamilias(null, null);
    }
})();
