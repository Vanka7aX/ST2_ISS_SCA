(() => {
    const list = document.getElementById('historyPageList');
    const search = document.getElementById('historySearch');
    const refresh = document.getElementById('refreshHistory');
    let history = [];
    const number = value => Number(value).toLocaleString('bg-BG', { maximumFractionDigits: 4 });
    function render() {
        const term = search.value.trim().toUpperCase();
        const entries = history.filter(item => `${item.fromCurrency} ${item.toCurrency}`.toUpperCase().includes(term));
        list.replaceChildren();
        if (!entries.length) { list.textContent = history.length ? 'Няма записи за тази валута.' : 'Все още няма записани конвертирания.'; return; }
        const table = document.createElement('table'); table.className = 'history-table';
        const head = document.createElement('thead'); const headRow = document.createElement('tr');
        ['Конвертиране', 'Курс', 'Дата (UTC)'].forEach(label => { const th = document.createElement('th'); th.scope = 'col'; th.textContent = label; headRow.append(th); });
        head.append(headRow); table.append(head);
        const body = document.createElement('tbody');
        entries.forEach(item => {
            const row = document.createElement('tr');
            const values = [`${number(item.amount)} ${item.fromCurrency} → ${number(item.convertedAmount)} ${item.toCurrency}`, `1 ${item.fromCurrency} = ${number(item.exchangeRate)} ${item.toCurrency}`, item.convertedAtUtc ? item.convertedAtUtc.slice(0, 16).replace('T', ' ') : '–'];
            values.forEach(value => { const cell = document.createElement('td'); cell.textContent = value; row.append(cell); });
            body.append(row);
        });
        table.append(body); list.append(table);
    }
    async function load() {
        refresh.disabled = true; list.textContent = 'Зареждане на историята...';
        try {
            const response = await fetch('/api/Currency/history');
            if (!response.ok) throw new Error(`HTTP ${response.status}`);
            const data = await response.json();
            if (!Array.isArray(data)) throw new Error('Невалиден отговор.');
            history = data; render();
        } catch (error) { list.textContent = `Историята не можа да бъде заредена: ${error.message}`; }
        finally { refresh.disabled = false; }
    }
    refresh.addEventListener('click', load); search.addEventListener('input', render); load();
})();
