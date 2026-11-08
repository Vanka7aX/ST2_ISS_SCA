// The currency list uses the options already loaded by the converter.
document.addEventListener('DOMContentLoaded', () => {
    const button = document.getElementById('toggleCurrenciesButton');
    const panel = document.getElementById('currenciesPanel');
    const search = document.getElementById('currencySearch');
    const list = document.getElementById('currenciesList');
    const select = document.getElementById('fromCurrency');
    if (!button || !panel || !search || !list || !select) return;
    function renderCurrencies() {
        const term = search.value.trim().toLocaleLowerCase('bg');
        const matches = Array.from(select.options).filter(option => option.text.toLocaleLowerCase('bg').includes(term));
        list.replaceChildren();
        if (!matches.length) { list.textContent = 'Няма намерени валути.'; return; }
        for (const option of matches) {
            const item = document.createElement('div');
            item.className = 'currency-item';
            item.textContent = option.text;
            list.appendChild(item);
        }
    }
    button.addEventListener('click', () => {
        panel.hidden = !panel.hidden;
        button.setAttribute('aria-expanded', String(!panel.hidden));
        if (!panel.hidden) renderCurrencies();
    });
    search.addEventListener('input', renderCurrencies);
    new MutationObserver(() => { if (!panel.hidden) renderCurrencies(); }).observe(select, { childList: true });
});
