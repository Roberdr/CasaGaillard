(() => {
    const root = document.querySelector('[data-stock-filter-root]');
    if (!root) return;
    const input = root.querySelector('#stockFilter');
    const cards = Array.from(document.querySelectorAll('[data-stock-card]'));
    const counter = root.querySelector('#stockCounter');
    const clear = root.querySelector('#stockClear');
    const update = () => {
        const query = (input.value || '').trim().toLocaleLowerCase('es');
        let visible = 0;
        cards.forEach(card => {
            const match = (card.dataset.search || '').toLocaleLowerCase('es').includes(query);
            card.hidden = !match;
            if (match) visible++;
        });
        counter.textContent = `${visible} visibles`;
    };
    input.addEventListener('input', update);
    clear.addEventListener('click', () => { input.value = ''; update(); input.focus(); });
    update();
})();
