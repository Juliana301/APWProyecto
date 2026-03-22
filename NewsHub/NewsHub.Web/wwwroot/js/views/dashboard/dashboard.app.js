// ===== INIT =====

// Cambiar tab por URL
const hash = window.location.hash.replace("#", "");

if (hash) {
    switchTab(hash);
}


// Iconos
lucide.createIcons();


// Dark mode
if (localStorage.getItem('darkMode') === 'true') document.documentElement.classList.add('dark');


// Render inicial
//renderFeed();
loadFeed();
renderSaved();


// ===== FILTER =====

function filterCards() {
    const q = document.getElementById('search-input').value.toLowerCase();
    document.querySelectorAll('#feed-grid .news-card').forEach(card => {
        const text = card.textContent.toLowerCase();
        card.style.display = text.includes(q) ? '' : 'none';
    });
}

// ===== FILE HANDLER =====

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

// ===== CLEAR PREVIEW =====

function clearPreview() {
    document.getElementById('json-preview').classList.add('hidden');
    document.getElementById('json-content').textContent = '';
    document.getElementById('file-input').value = '';
}


// ===== DRAG & DROP =====

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