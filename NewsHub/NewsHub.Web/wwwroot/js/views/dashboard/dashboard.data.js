// ===== DATA =====

let mockNews = [];

let savedItems = [];


// ===== LOAD FEED =====

async function loadFeed() {

    try {

        const response =
            await fetch(
                '/api/SourcesApi/feed',
                {
                    credentials: "include"
                });

        if (!response.ok) {

            if (response.status === 401) {

                // No logueado → redirigir
                window.location.href =
                    '/Authentication/Login';

                return;
            }

            throw new Error(
                'Error cargando feed');
        }

        mockNews =
            await response.json();

        renderFeed();

        getTotalItems();

    } catch (error) {

        console.error(
            'Error cargando feed:',
            error);
    }
}


// ===== LOAD SAVED =====

async function loadSavedItems() {

    try {

        const response =
            await fetch(
                '/api/SourcesItemsApi/savedfeed',
                {
                    credentials: "include"
                });

        if (!response.ok) {

            if (response.status === 401) {

                // No logueado → redirigir
                window.location.href =
                    '/Authentication/Login';

                return;
            }

            throw new Error(
                'Error cargando guardados');
        }

        savedItems =
            await response.json();

        renderSaved();

        renderFeed();

    } catch (error) {

        console.error(
            'Error cargando guardados:',
            error);
    }
}

// ===== GET TOTAL ITEMS =====

function getTotalItems() {
    const total = mockNews.length;
    document.getElementById('total-items').textContent = `${total} items`;
}

// ===== DOWNLOAD JSON =====

function downloadNewsJson(uniqueId) {

    window.location.href =
        `/api/ImportExportApi/export/news/${uniqueId}`;

}

// ===== SAVE NEWS =====
async function saveNews(uniqueId) {

    try {

        // Buscar noticia en feed
        const item =
            mockNews.find(
                x => x.id === uniqueId);

        if (!item) {

            console.error(
                "Noticia no encontrada");

            return;
        }

        const response =
            await fetch(
                '/api/SourcesItemsApi/save',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: "include",
                    body: JSON.stringify(item)
                });

        if (!response.ok) {

            const text =
                await response.text();

            throw new Error(text);
        }

        // Recargar guardados
        await loadSavedItems();

    } catch (error) {

        console.error(
            'Error guardando:',
            error);
    }
}