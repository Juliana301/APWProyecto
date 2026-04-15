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

        // 🔧 FIX
        renderFeed();

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

        // 🔧 FIX
        renderSaved();

    } catch (error) {

        console.error(
            'Error cargando guardados:',
            error);
    }
}


// ===== DOWNLOAD JSON =====

function downloadNewsJson(uniqueId) {

    window.location.href =
        `/api/ImportExportApi/export/news/${uniqueId}`;

}