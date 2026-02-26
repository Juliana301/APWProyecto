// ===== MOCK DATA =====
const mockNews = [
    { id: 1, source: 'NewsAPI', type: 'api', title: 'OpenAI lanza GPT-5 con capacidades multimodales avanzadas', description: 'La nueva version del modelo de lenguaje promete revolucionar la interaccion humano-maquina con comprension visual y auditiva integrada.', date: '2026-02-12', tags: ['IA', 'Tecnologia'] },
    { id: 2, source: 'TechCrunch', type: 'feed', title: 'Ciberataque masivo afecta a instituciones bancarias en Europa', description: 'Un grupo de hackers logro vulnerar sistemas de seguridad de multiples bancos, exponiendo datos de millones de usuarios.', date: '2026-02-11', tags: ['Ciberseguridad', 'Finanzas'] },
    { id: 3, source: 'HackerNews', type: 'widget', title: 'Kubernetes 2.0: La nueva era de la orquestacion de contenedores', description: 'La actualizacion trae mejoras significativas en rendimiento, seguridad y facilidad de uso para despliegues a escala.', date: '2026-02-11', tags: ['Cloud', 'DevOps'] },
    { id: 4, source: 'NewsAPI', type: 'api', title: 'Apple presenta Vision Pro 2 con nuevo chip M5', description: 'El nuevo dispositivo de realidad mixta ofrece mayor campo de vision y menor peso, junto con un ecosistema de apps ampliado.', date: '2026-02-10', tags: ['Hardware', 'Apple'] },
    { id: 5, source: 'TechCrunch', type: 'feed', title: 'Startup de energia limpia recauda $2B en Serie D', description: 'SolarGrid Technologies planea expandir su red de paneles solares inteligentes a 50 paises en los proximos dos anos.', date: '2026-02-10', tags: ['Startups', 'Energia'] },
    { id: 6, source: 'HackerNews', type: 'widget', title: 'Rust supera a Python en el indice TIOBE por primera vez', description: 'El lenguaje de sistemas gana popularidad gracias a su seguridad de memoria y adopcion en proyectos de infraestructura critica.', date: '2026-02-09', tags: ['Desarrollo', 'Open Source'] },
    { id: 7, source: 'NewsAPI', type: 'api', title: 'La UE aprueba nueva regulacion de inteligencia artificial', description: 'El marco regulatorio establece categorias de riesgo y requisitos de transparencia para sistemas de IA desplegados en la union.', date: '2026-02-09', tags: ['IA', 'Regulacion'] },
    { id: 8, source: 'TechCrunch', type: 'feed', title: 'GitHub Copilot ahora genera tests automaticos para proyectos completos', description: 'La nueva funcionalidad analiza toda la base de codigo y genera suites de pruebas con cobertura superior al 90%.', date: '2026-02-08', tags: ['Desarrollo', 'IA'] },
    { id: 9, source: 'HackerNews', type: 'widget', title: 'AWS lanza servicio de computacion cuantica en la nube', description: 'Amazon Braket Pro permite a los desarrolladores ejecutar algoritmos cuanticos complejos sin hardware especializado.', date: '2026-02-08', tags: ['Cloud', 'AWS'] },
];

const savedItems = [
    { id: 101, source: 'NewsAPI', type: 'api', title: 'OpenAI lanza GPT-5 con capacidades multimodales avanzadas', description: 'La nueva version del modelo de lenguaje promete revolucionar la interaccion humano-maquina con comprension visual y auditiva integrada.', date: '2026-02-12', tags: ['IA', 'Tecnologia'] },
    { id: 102, source: 'HackerNews', type: 'widget', title: 'Rust supera a Python en el indice TIOBE por primera vez', description: 'El lenguaje de sistemas gana popularidad gracias a su seguridad de memoria y adopcion en proyectos de infraestructura critica.', date: '2026-02-09', tags: ['Desarrollo', 'Open Source'] },
];

// ===== INIT =====
lucide.createIcons();
if (localStorage.getItem('darkMode') === 'true') document.documentElement.classList.add('dark');

const userName = localStorage.getItem('userName') || 'Usuario';
document.getElementById('sidebar-username').textContent = userName;
document.getElementById('sidebar-role').textContent = 'Conectado';

renderFeed();
renderSaved();

// ===== FUNCTIONS =====
function toggleDarkMode() {
    document.documentElement.classList.toggle('dark');
    localStorage.setItem('darkMode', document.documentElement.classList.contains('dark'));
    lucide.createIcons();
}

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebar-overlay');
    sidebar.classList.toggle('-translate-x-full');
    overlay.classList.toggle('hidden');
}

function toggleAdminBar() {
    const content = document.getElementById('admin-content');
    const chevron = document.getElementById('admin-chevron');
    content.classList.toggle('hidden');
    chevron.style.transform = content.classList.contains('hidden') ? '' : 'rotate(180deg)';
}

function switchTab(name) {
    document.querySelectorAll('.tab-panel').forEach(p => p.classList.add('hidden'));
    document.querySelectorAll('.tab-btn').forEach(b => { b.classList.remove('tab-active'); b.classList.add('text-surface-500'); });
    document.querySelectorAll('.nav-item').forEach(n => {
        n.classList.remove('bg-brand-50', 'dark:bg-brand-900/20', 'text-brand-600', 'dark:text-brand-400');
        n.classList.add('text-surface-600', 'dark:text-surface-400');
    });

    document.getElementById('panel-' + name).classList.remove('hidden');
    const tabBtn = document.getElementById('tab-' + name);
    tabBtn.classList.add('tab-active');
    tabBtn.classList.remove('text-surface-500');

    const navBtn = document.getElementById('nav-' + name);
    if (navBtn) {
        navBtn.classList.add('bg-brand-50', 'dark:bg-brand-900/20', 'text-brand-600', 'dark:text-brand-400');
        navBtn.classList.remove('text-surface-600', 'dark:text-surface-400');
    }
}

function sourceColor(type) {
    if (type === 'api') return 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300';
    if (type === 'feed') return 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300';
    return 'bg-brand-100 dark:bg-brand-900/30 text-brand-700 dark:text-brand-300';
}

function createCard(item, isSaved) {
    const tagHtml = item.tags.map(t => `<span class="px-2 py-0.5 text-xs rounded-full bg-surface-100 dark:bg-surface-700 text-surface-500 dark:text-surface-400">${t}</span>`).join('');
    const actionBtn = isSaved
        ? `<button class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"><i data-lucide="trash-2" class="w-3.5 h-3.5"></i>Eliminar</button>`
        : `<button class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-brand-600 hover:bg-brand-50 dark:hover:bg-brand-900/20 transition-colors"><i data-lucide="bookmark" class="w-3.5 h-3.5"></i>Guardar</button>`;

    return `
            <article class="news-card card-enter bg-white dark:bg-surface-800 rounded-2xl border border-surface-200 dark:border-surface-700 shadow-sm hover:shadow-md transition-shadow overflow-hidden flex flex-col">
              <div class="p-5 flex-1 flex flex-col">
                <div class="flex items-center gap-2 mb-3">
                  <span class="px-2 py-0.5 text-xs font-medium rounded-full ${sourceColor(item.type)}">${item.source}</span>
                  <span class="text-xs text-surface-400 ml-auto">${item.date}</span>
                </div>
                <a href="item-detail.html?id=${item.id}" class="text-base font-semibold leading-snug hover:text-brand-600 dark:hover:text-brand-400 transition-colors mb-2 line-clamp-2">${item.title}</a>
                <p class="text-sm text-surface-500 dark:text-surface-400 leading-relaxed line-clamp-3 flex-1">${item.description}</p>
                <div class="flex flex-wrap gap-1.5 mt-3">${tagHtml}</div>
              </div>
              <div class="px-5 py-3 border-t border-surface-100 dark:border-surface-700 flex items-center justify-between">
                ${actionBtn}
                <div class="flex items-center gap-1">
                  <a href="item-detail.html?id=${item.id}" class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-surface-500 hover:bg-surface-100 dark:hover:bg-surface-700 transition-colors"><i data-lucide="eye" class="w-3.5 h-3.5"></i>Detalle</a>
                  <button class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-surface-500 hover:bg-surface-100 dark:hover:bg-surface-700 transition-colors"><i data-lucide="download" class="w-3.5 h-3.5"></i>JSON</button>
                </div>
              </div>
            </article>`;
}

function renderFeed() {
    const grid = document.getElementById('feed-grid');
    grid.innerHTML = mockNews.map(n => createCard(n, false)).join('');
    lucide.createIcons();
}

function renderSaved() {
    const grid = document.getElementById('saved-grid');
    const empty = document.getElementById('saved-empty');
    if (savedItems.length > 0) {
        grid.innerHTML = savedItems.map(n => createCard(n, true)).join('');
        empty.classList.add('hidden');
        grid.classList.remove('hidden');
    } else {
        grid.classList.add('hidden');
        empty.classList.remove('hidden');
    }
    lucide.createIcons();
}

function filterCards() {
    const q = document.getElementById('search-input').value.toLowerCase();
    document.querySelectorAll('#feed-grid .news-card').forEach(card => {
        const text = card.textContent.toLowerCase();
        card.style.display = text.includes(q) ? '' : 'none';
    });
}

function handleFileSelect(event) {
    const file = event.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = function (e) {
        try {
            const json = JSON.parse(e.target.result);
            document.getElementById('json-content').textContent = JSON.stringify(json, null, 2);
            document.getElementById('json-preview').classList.remove('hidden');
        } catch (err) {
            alert('El archivo no es un JSON valido.');
        }
    };
    reader.readAsText(file);
}

function clearPreview() {
    document.getElementById('json-preview').classList.add('hidden');
    document.getElementById('json-content').textContent = '';
    document.getElementById('file-input').value = '';
}

// Drag and drop
const dropZone = document.getElementById('drop-zone');
dropZone.addEventListener('dragover', e => { e.preventDefault(); dropZone.classList.add('border-brand-400', 'bg-brand-50/30'); });
dropZone.addEventListener('dragleave', () => { dropZone.classList.remove('border-brand-400', 'bg-brand-50/30'); });
dropZone.addEventListener('drop', e => {
    e.preventDefault();
    dropZone.classList.remove('border-brand-400', 'bg-brand-50/30');
    const file = e.dataTransfer.files[0];
    if (file && file.name.endsWith('.json')) {
        const input = document.getElementById('file-input');
        const dt = new DataTransfer();
        dt.items.add(file);
        input.files = dt.files;
        handleFileSelect({ target: input });
    }
});