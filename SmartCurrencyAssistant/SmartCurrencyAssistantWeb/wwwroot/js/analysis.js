(() => {
    const form = document.getElementById('analysisForm');
    const from = document.getElementById('analysisFrom');
    const to = document.getElementById('analysisTo');
    const start = document.getElementById('analysisStart');
    const end = document.getElementById('analysisEnd');
    const trendResult = document.getElementById('trendResult');
    const aiResult = document.getElementById('analysisAiResult');
    const aiButton = document.getElementById('analysisAiButton');
    const isoDate = date => `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
    const today = new Date();
    const monthAgo = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
    end.value = end.max = start.max = isoDate(today);
    start.value = isoDate(monthAgo);

    async function populateCurrencies() {
        try {
            const response = await fetch('/api/Currency/currencies');
            if (!response.ok) throw new Error();
            const currencies = await response.json();
            if (!Array.isArray(currencies) || !currencies.length) return;
            for (const select of [from, to]) {
                const selected = select.value;
                const options = currencies.map(c => ({ code: c.iso_code ?? c.isoCode ?? c.code, name: c.name })).filter(c => c.code).map(c => new Option(`${c.code} - ${c.name ?? c.code}`, c.code));
                if (!options.length) continue;
                select.replaceChildren(...options);
                if (Array.from(select.options).some(option => option.value === selected)) select.value = selected;
            }
        } catch { /* Keep the default choices if the provider is unavailable. */ }
    }
    function parameters() {
        if (!form.reportValidity()) return null;
        if (start.value > end.value) { trendResult.textContent = 'Началната дата трябва да е преди крайната.'; return null; }
        if ((Date.parse(end.value) - Date.parse(start.value)) / 86400000 > 365) { trendResult.textContent = 'Изберете период до 365 дни.'; return null; }
        return new URLSearchParams({ fromCurrency: from.value, toCurrency: to.value, startDate: start.value, endDate: end.value });
    }
    async function getData(endpoint, params) {
        const response = await fetch(`/api/Currency/${endpoint}?${params}`);
        if (!response.ok) {
            let message = `Грешка ${response.status}. Опитайте отново.`;
            try { const data = await response.json(); if (data.detail) message = data.detail; } catch { }
            throw new Error(message);
        }
        return response.json();
    }
    form.addEventListener('submit', async event => {
        event.preventDefault();
        const params = parameters(); if (!params) return;
        const button = form.querySelector('button'); button.disabled = true;
        trendResult.textContent = 'Зареждане на данните...';
        try {
            const data = await getData('trend', params);
            const number = value => Number(value).toLocaleString('bg-BG', { maximumFractionDigits: 6 });
            const rows = [['Начален курс', number(data.startRate)], ['Краен курс', number(data.endRate)], ['Среден курс', number(data.averageRate)], ['Най-нисък курс', number(data.minRate)], ['Най-висок курс', number(data.maxRate)], ['Промяна', `${number(data.percentageChange)}%`]];
            trendResult.replaceChildren();
            const title = document.createElement('p'); title.className = 'trend-pair'; title.textContent = `${data.fromCurrency} / ${data.toCurrency}`; trendResult.append(title);
            const list = document.createElement('dl'); list.className = 'trend-stats';
            rows.forEach(([label, value]) => { const dt = document.createElement('dt'); dt.textContent = label; const dd = document.createElement('dd'); dd.textContent = value; list.append(dt, dd); });
            trendResult.append(list);
        } catch (error) { trendResult.textContent = `Неуспешно зареждане: ${error.message}`; }
        finally { button.disabled = false; }
    });
    aiButton.addEventListener('click', async () => {
        const params = parameters(); if (!params) return;
        aiButton.disabled = true; aiResult.textContent = 'Генериране на анализ...';
        try { const data = await getData('trend/analysis', params); aiResult.textContent = data.analysis || 'Няма върнат анализ.'; }
        catch (error) { aiResult.textContent = `Анализът не можа да бъде зареден: ${error.message}`; }
        finally { aiButton.disabled = false; }
    });
    populateCurrencies();
})();
