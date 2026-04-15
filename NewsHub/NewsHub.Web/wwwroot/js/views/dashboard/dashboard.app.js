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
loadSavedItems();


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

    const file =
        event.target.files[0];

    if (!file) return;

    const reader =
        new FileReader();

    reader.onload =
        function (e) {

            try {

                const jsonText =
                    e.target.result;

                const json =
                    JSON.parse(jsonText);

                if (!validateOfficialSchema(jsonText)) {

                    alert(
                        "El JSON no tiene un schema válido.");

                    return;
                }

                document
                    .getElementById('json-content')
                    .textContent =
                    JSON.stringify(json, null, 2);

                document
                    .getElementById('json-preview')
                    .classList
                    .remove('hidden');

            } catch {

                alert(
                    'El archivo no es un JSON válido.');
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

// ===== VALIDATE JSON =====
function validateOfficialSchema(jsonText) {

    try {

        const obj =
            JSON.parse(jsonText);

        return obj.schemaVersion ===
               "edu.univ.ingest.v1";

    } catch {

        return false;
    }
}

// ===== Upload JSON =====

async function uploadJsonToServer() {

    const button =
        document.querySelector(
            "#panel-upload button");

    const fileInput =
        document.getElementById(
            "file-input");

    if (!fileInput.files.length) {

        alert("Selecciona un archivo primero.");
        return;
    }

    const file =
        fileInput.files[0];

    const formData =
        new FormData();

    formData.append("file", file);

    try {

        button.disabled = true;
        button.innerText = "Importando...";

        const response =
            await fetch(
                "/api/ImportExportApi/import/news",
                {
                    method: "POST",
                    body: formData
                });

        if (!response.ok) {

            const text =
                await response.text();

            throw new Error(text);
        }

        const result =
            await response.json();

        alert(
            "Importación exitosa:\n" +
            result.title);

        loadSavedItems();
        
        clearPreview();


    } catch (err) {

        alert(
            "Error al importar:\n" +
            err.message);

    } finally {

        button.disabled = false;
        button.innerText =
            "Cargar a la base de datos";
    }
}