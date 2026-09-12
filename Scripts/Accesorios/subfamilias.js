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

    function clearOptions() { sub.options.length = 0; }
    function addEmpty() { var opt = document.createElement('option'); opt.value = ''; opt.text = '-- Sin subfamilia --'; sub.appendChild(opt); }

    /** @param {string|null} familiaId @param {string|null} selectValueToChoose */
    function loadSubfamilias(familiaId, selectValueToChoose) {
        clearOptions();
        if (!familiaId) { addEmpty(); if (selectValueToChoose) sub.value = selectValueToChoose; return; }
        var url = sub.getAttribute('data-url') || '/Mantenimiento/Accesorios/SubfamiliasPorFamilia';
        url += '?familiaId=' + encodeURIComponent(familiaId);
        fetch(url, { credentials: 'same-origin' })
            .then(function(r){ return r.json(); })
            .then(function(data){
                addEmpty();
                data.forEach(function(item){
                    var opt = document.createElement('option');
                    opt.value = item.id;
                    opt.text = item.text;
                    sub.appendChild(opt);
                });
                if (selectValueToChoose) sub.value = selectValueToChoose;
            }).catch(function(){ /* ignore */ });
    }

    familia.addEventListener('change', function(){ loadSubfamilias(familia.value, null); });

    var selectedFamilia = familia.value;
    var selectedSub = sub.getAttribute('data-selected') || sub.getAttribute('data_selected') || (sub.dataset ? sub.dataset.selected : null);
    if (selectedFamilia) loadSubfamilias(selectedFamilia, selectedSub);
})();
