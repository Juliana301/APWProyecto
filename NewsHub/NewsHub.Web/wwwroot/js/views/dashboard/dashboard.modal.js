// ===== MOSTRAR / OCULTAR API CONFIG =====

function toggleApiConfig(type) {

    const container =
        document.getElementById("api-config-container");

    if (!container) return;

    // 0 = Api
    if (type === "0") {

        container.classList.remove("hidden");

    } else {

        container.classList.add("hidden");

    }

}


// Escuchar cambio manual del tipo

const editTypeEl =
    document.getElementById("EditSource_ComponentType");

if (editTypeEl) {

    editTypeEl.addEventListener("change", function () {

        toggleApiConfig(this.value);

    });

}


// ===== BOTONES EDITAR =====

document.addEventListener("click", function (e) {

    const btn = e.target.closest(".edit-btn");

    if (!btn) return;

    const id = btn.dataset.id;
    const name = btn.dataset.name;
    const url = btn.dataset.url;
    const description = btn.dataset.description;
    const type = btn.dataset.type;

    const hasSecret =
        btn.dataset.secret === "true";

    let apiConfigJson =
        btn.dataset.json || "";

    // Formatear JSON bonito

    const apiConfigEl =
        document.getElementById("EditSource_ApiConfigJson");

    if (apiConfigEl) {

        if (apiConfigJson) {

            try {

                apiConfigEl.value =
                    JSON.stringify(
                        JSON.parse(apiConfigJson),
                        null,
                        2
                    );

            }
            catch {

                apiConfigEl.value =
                    apiConfigJson;

            }

        }
        else {

            apiConfigEl.value = '';

        }

    }

    openEditModal(
        id,
        name,
        url,
        description,
        type,
        hasSecret,
        apiConfigJson
    );

});


// ===== MODAL: EDITAR FUENTE =====

function openEditModal(
    id,
    name,
    url,
    description,
    type,
    hasSecret,
    apiConfigJson
) {

    document.getElementById('edit-id').value = id;

    document.getElementById('EditSource_Name').value =
        name;

    document.getElementById('EditSource_Url').value =
        url;

    document.getElementById('EditSource_Description').value =
        description || '';

    document.getElementById('EditSource_ComponentType').value =
        type;

    document.getElementById('edit-secret').checked =
        hasSecret;

    document.getElementById('EditSource_ApiConfigJson').value =
        apiConfigJson || '';

    // Mostrar u ocultar JSON según tipo

    toggleApiConfig(type);

    document.getElementById('modal-edit')
        .classList.remove('hidden');

    lucide.createIcons();

}


// ===== CERRAR MODAL =====

function closeEditModal() {

    document.getElementById('modal-edit')
        .classList.add('hidden');

}

// ===== BOTONES ELIMINAR =====

document.addEventListener("click", function (e) {

    const btn = e.target.closest(".delete-btn");

    if (!btn) return;

    const id = btn.dataset.id;
    const name = btn.dataset.name;

    openDeleteModal(id, name);

});


// ===== MODAL: ELIMINAR FUENTE =====

function openDeleteModal(id, name) {

    const idInput =
        document.getElementById("DeleteSource_Id");
    const nameInput =
        document.getElementById("DeleteSource_SourceName");

    const nameLabel =
        document.getElementById("delete-source-name");

    const modal =
        document.getElementById("modal-delete");

    if (!idInput || !nameInput || !nameLabel || !modal) {

        console.error(
            "Elementos del modal-delete no encontrados"
        );

        return;

    }

    idInput.value = id;
    nameInput.value = name;

    nameLabel.textContent =
        '"' + name + '"';

    modal.classList.remove("hidden");

    if (window.lucide) {
        lucide.createIcons();
    }

}


// ===== CERRAR MODAL =====

function closeDeleteModal() {

    const modal =
        document.getElementById("modal-delete");

    if (modal) {

        modal.classList.add("hidden");

    }

}