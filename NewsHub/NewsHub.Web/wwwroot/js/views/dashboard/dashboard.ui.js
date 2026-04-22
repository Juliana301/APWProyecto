// ===== UI FUNCTIONS =====

// Toggle Admin
function toggleAdminBar() {
    const content = document.getElementById('admin-content');
    const chevron = document.getElementById('admin-chevron');
    content.classList.toggle('hidden');
    chevron.style.transform = content.classList.contains('hidden') ? '' : 'rotate(180deg)';
}


// Tabs
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

// Badge color
function sourceColor(type) {
    if (type === 'Api') return 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300';
    if (type === 'Rss') return 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300';
    return 'bg-brand-100 dark:bg-brand-900/30 text-brand-700 dark:text-brand-300';
}

function escapeHtml(value) {
    return String(value ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function createCard(item, isSaved) {
    const safeId = encodeURIComponent(item.id ?? '');
    const safeSource = escapeHtml(item.source);
    const safeDate = escapeHtml(item.date);
    const safeTitle = escapeHtml(item.title);
    const safeDescription = escapeHtml(item.description);

    const tags = Array.isArray(item.tags) ? item.tags : [];

    const rawUrl = typeof item.url === 'string' ? item.url.trim() : '';
    const hasExternalUrl = rawUrl !== '';
    const safeUrl = escapeHtml(rawUrl);

    const titleHref = hasExternalUrl
        ? safeUrl
        : `item-detail.html?id=${safeId}`;

    const titleTarget = hasExternalUrl
        ? `target="_blank" rel="noopener noreferrer"`
        : '';

    const tagHtml = tags
        .filter(t => t != null && String(t).trim() !== '')
        .map(t => `
            <span class="px-2 py-0.5 text-xs rounded-full bg-surface-100 dark:bg-surface-700 text-surface-500 dark:text-surface-400">
                ${escapeHtml(t)}
            </span>
        `)
        .join('');

    const actionBtn = isSaved
        ? `<button class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-green-600 bg-green-50 dark:bg-green-900/20 hover:bg-green-100 dark:hover:bg-green-900/30 transition-colors">
        <i data-lucide="bookmark-check" class="w-3.5 h-3.5"></i>Saved
       </button>`
        : `<button onclick="saveNews('${safeId}')" class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-brand-600 hover:bg-brand-50 dark:hover:bg-brand-900/20 transition-colors">
        <i data-lucide="bookmark" class="w-3.5 h-3.5"></i>Guardar
       </button>`;

    const detailButton = `
        <a href="item-detail.html?id=${safeId}"
           class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-surface-500 hover:bg-surface-100 dark:hover:bg-surface-700 transition-colors">
           <i data-lucide="eye" class="w-3.5 h-3.5"></i>Detalle
        </a>`;

    return `
        <article class="news-card card-enter bg-white dark:bg-surface-800 rounded-2xl border border-surface-200 dark:border-surface-700 shadow-sm hover:shadow-md transition-shadow overflow-hidden flex flex-col">
          <div class="p-5 flex-1 flex flex-col">
            <div class="flex items-center gap-2 mb-3">
              <span class="px-2 py-0.5 text-xs font-medium rounded-full ${sourceColor(item.type)}">${safeSource}</span>
              <span class="text-xs text-surface-400 ml-auto">${safeDate}</span>
            </div>
            <a href="${titleHref}" ${titleTarget}
               class="text-base font-semibold leading-snug hover:text-brand-600 dark:hover:text-brand-400 transition-colors mb-2 line-clamp-2">
               ${safeTitle}
            </a>
            <p class="text-sm text-surface-500 dark:text-surface-400 leading-relaxed line-clamp-3 flex-1">${safeDescription}</p>
            <div class="flex flex-wrap gap-1.5 mt-3">${tagHtml}</div>
          </div>
          <div class="px-5 py-3 border-t border-surface-100 dark:border-surface-700 flex items-center justify-between">
            ${actionBtn}
            <div class="flex items-center gap-1">
              
              <button class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium rounded-lg text-surface-500 hover:bg-surface-100 dark:hover:bg-surface-700 transition-colors" onclick="downloadNewsJson('${safeId}')">
                <i data-lucide="download" class="w-3.5 h-3.5"></i>JSON
              </button>
            </div>
          </div>
        </article>`;
}

// Render Feed
function renderFeed() {

    const grid =
        document.getElementById('feed-grid');

    const savedIds =
        new Set(
            savedItems.map(
                x => `"${String(x.id)}"`
            )
        );

    console.log(
        "SavedIds RAW:",
        savedItems.map(
            x => `"${String(x.id)}"`
        )
    );

    grid.innerHTML =
        mockNews
            .map(n => {

                const newsId =
                    `"${String(n.id)}"`;

                console.log(
                    "Comparing:",
                    newsId
                );

                const isSaved =
                    savedIds.has(newsId);

                if (isSaved) {

                    console.log(
                        "MATCH:",
                        newsId
                    );

                }

                return createCard(
                    n,
                    isSaved
                );

            })
            .join('');

    lucide.createIcons();
}


// Render Guardados
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