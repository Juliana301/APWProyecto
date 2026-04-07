// ===== API CONFIG MANAGEMENT =====

// Mostrar / ocultar configuración API según el tipo
function toggleApiConfig(type) {
    const container = document.getElementById("api-config-container");
    const templates = document.getElementById("api-templates");

    if (!container) return;

    // 0 = Api
    if (type === "0") {
        container.classList.remove("hidden");
        templates.classList.remove("hidden");
    } else {
        container.classList.add("hidden");
        templates.classList.add("hidden");
    }
}

// ==========================================
// TEMPLATES ESENCIALES
// ==========================================

const apiTemplates = {

    // =========================================
    // AUTO — SIN API KEY
    // Detecta todo automáticamente
    // =========================================

    auto: {
        Type: "Auto",

        Limit: 10
    },

    // =========================================
    // AUTO — CON API KEY
    // Muy común hoy en día
    // =========================================

    autoWithKey: {
        Type: "Auto",

        Headers: {
            "X-Api-Key": "{{API_KEY}}"
        },

        Limit: 10
    },

    // =========================================
    // SIMPLE — Mapping manual
    // Para APIs que no funcionan con Auto
    // =========================================

    simple: {
        Type: "Simple",

        Root: "articles",

        Limit: 10,

        Mapping: {
            Title: "title",
            Description: "description",
            Url: "url",
            PublishedAt: "publishedAt",
            Category: "source.name"
        }
    },

    // =========================================
    // ID PIPELINE — Hacker News
    // =========================================

    idPipeline: {
        Type: "IdPipeline",

        IdsUrl:
            "https://hacker-news.firebaseio.com/v0/topstories.json",

        ItemUrlTemplate:
            "https://hacker-news.firebaseio.com/v0/item/{id}.json",

        Limit: 5,

        Mapping: {
            Title: "title",
            Description: "text",
            Url: "url",
            PublishedAt: "time",
            Category: "type"
        }
    }
};

// Event listener para selección de plantillas API
document.addEventListener("change", function (e) {
    if (e.target.name !== "apiTemplate") return;

    const selected = e.target.value;
    const template = apiTemplates[selected];
    const textarea = document.getElementById("EditSource_ApiConfigJson");

    if (textarea.value.trim() !== "") {
        if (!confirm("Esto reemplazará el JSON actual. ¿Continuar?")) return;
    }

    if (!textarea || !template) return;

    textarea.value = JSON.stringify(template, null, 2);
});

// Limpiar selección de plantillas
function clearTemplateSelection() {
    document.querySelectorAll("input[name='apiTemplate']").forEach(r => r.checked = false);
}

// Escuchar cambio manual del tipo
const editTypeEl = document.getElementById("EditSource_ComponentType");
if (editTypeEl) {
    editTypeEl.addEventListener("change", function () {
        toggleApiConfig(this.value);
    });
}

// ===== EDIT MODAL HANDLERS =====

// Event listener para botones editar
document.addEventListener("click", function (e) {
    const btn = e.target.closest(".edit-btn");
    if (!btn) return;

    const id = btn.dataset.id;
    const name = btn.dataset.name;
    const url = btn.dataset.url;
    const description = btn.dataset.description;
    const type = btn.dataset.type;
    const hasSecret = btn.dataset.secret === "true";
    let apiConfigJson = btn.dataset.json || "";

    // Formatear JSON bonito
    const apiConfigEl = document.getElementById("EditSource_ApiConfigJson");
    if (apiConfigEl) {
        if (apiConfigJson) {
            try {
                apiConfigEl.value = JSON.stringify(JSON.parse(apiConfigJson), null, 2);
            } catch {
                apiConfigEl.value = apiConfigJson;
            }
        } else {
            apiConfigEl.value = '';
        }
    }

    openEditModal(id, name, url, description, type, hasSecret, apiConfigJson);
});

// Abrir modal de edición
function openEditModal(id, name, url, description, type, hasSecret, apiConfigJson) {
    document.getElementById('edit-id').value = id;
    document.getElementById('EditSource_Name').value = name;
    document.getElementById('EditSource_Url').value = url;
    document.getElementById('EditSource_Description').value = description || '';
    document.getElementById('EditSource_ComponentType').value = type;
    console.log("Tipo:", type);
    document.getElementById('edit-secret').checked = hasSecret;
    document.getElementById('EditSource_ApiConfigJson').value = apiConfigJson || '';

    // Mostrar u ocultar JSON según tipo
    toggleApiConfig(type);

    document.getElementById('modal-edit').classList.remove('hidden');
    lucide.createIcons();
}

// Cerrar modal de edición
function closeEditModal() {
    document.getElementById('modal-edit').classList.add('hidden');
}

// ===== DELETE MODAL HANDLERS =====

// Event listener para botones eliminar
document.addEventListener("click", function (e) {
    const btn = e.target.closest(".delete-btn");
    if (!btn) return;

    const id = btn.dataset.id;
    const name = btn.dataset.name;

    openDeleteModal(id, name);
});

// Abrir modal de eliminación
function openDeleteModal(id, name) {
    const idInput = document.getElementById("DeleteSource_Id");
    const nameInput = document.getElementById("DeleteSource_SourceName");
    const nameLabel = document.getElementById("delete-source-name");
    const modal = document.getElementById("modal-delete");

    if (!idInput || !nameInput || !nameLabel || !modal) {
        console.error("Elementos del modal-delete no encontrados");
        return;
    }

    idInput.value = id;
    nameInput.value = name;
    nameLabel.textContent = '"' + name + '"';

    modal.classList.remove("hidden");

    if (window.lucide) {
        lucide.createIcons();
    }
}

// Cerrar modal de eliminación
function closeDeleteModal() {
    const modal = document.getElementById("modal-delete");
    if (modal) {
        modal.classList.add("hidden");
    }
}