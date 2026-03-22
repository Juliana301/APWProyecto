// ===== DATA =====

// Noticias (mock)
//const mockNews = [
//    { id: 1, source: 'NewsAPI', type: 'api', title: 'OpenAI lanza GPT-5 con capacidades multimodales avanzadas', description: 'La nueva version del modelo de lenguaje promete revolucionar la interaccion humano-maquina con comprension visual y auditiva integrada.', date: '2026-02-12', tags: ['IA', 'Tecnologia'] },
//    { id: 2, source: 'TechCrunch', type: 'feed', title: 'Ciberataque masivo afecta a instituciones bancarias en Europa', description: 'Un grupo de hackers logro vulnerar sistemas de seguridad de multiples bancos, exponiendo datos de millones de usuarios.', date: '2026-02-11', tags: ['Ciberseguridad', 'Finanzas'] },
//    { id: 3, source: 'HackerNews', type: 'widget', title: 'Kubernetes 2.0: La nueva era de la orquestacion de contenedores', description: 'La actualizacion trae mejoras significativas en rendimiento, seguridad y facilidad de uso para despliegues a escala.', date: '2026-02-11', tags: ['Cloud', 'DevOps'] },
//    { id: 4, source: 'NewsAPI', type: 'api', title: 'Apple presenta Vision Pro 2 con nuevo chip M5', description: 'El nuevo dispositivo de realidad mixta ofrece mayor campo de vision y menor peso, junto con un ecosistema de apps ampliado.', date: '2026-02-10', tags: ['Hardware', 'Apple'] },
//    { id: 5, source: 'TechCrunch', type: 'feed', title: 'Startup de energia limpia recauda $2B en Serie D', description: 'SolarGrid Technologies planea expandir su red de paneles solares inteligentes a 50 paises en los proximos dos anos.', date: '2026-02-10', tags: ['Startups', 'Energia'] },
//    { id: 6, source: 'HackerNews', type: 'widget', title: 'Rust supera a Python en el indice TIOBE por primera vez', description: 'El lenguaje de sistemas gana popularidad gracias a su seguridad de memoria y adopcion en proyectos de infraestructura critica.', date: '2026-02-09', tags: ['Desarrollo', 'Open Source'] },
//    { id: 7, source: 'NewsAPI', type: 'api', title: 'La UE aprueba nueva regulacion de inteligencia artificial', description: 'El marco regulatorio establece categorias de riesgo y requisitos de transparencia para sistemas de IA desplegados en la union.', date: '2026-02-09', tags: ['IA', 'Regulacion'] },
//    { id: 8, source: 'TechCrunch', type: 'feed', title: 'GitHub Copilot ahora genera tests automaticos para proyectos completos', description: 'La nueva funcionalidad analiza toda la base de codigo y genera suites de pruebas con cobertura superior al 90%.', date: '2026-02-08', tags: ['Desarrollo', 'IA'] },
//    { id: 9, source: 'HackerNews', type: 'widget', title: 'AWS lanza servicio de computacion cuantica en la nube', description: 'Amazon Braket Pro permite a los desarrolladores ejecutar algoritmos cuanticos complejos sin hardware especializado.', date: '2026-02-08', tags: ['Cloud', 'AWS'] },
//];

let mockNews = [];

async function loadFeed() {
    try {

        const response =
            await fetch('/News/GetFeedJson');

        mockNews =
            await response.json();

        renderFeed();

    } catch (error) {

        console.error(
            'Error cargando feed:',
            error);

    }
}

// Guardados (mock)
const savedItems = [
    { id: 101, source: 'NewsAPI', type: 'api', title: 'OpenAI lanza GPT-5 con capacidades multimodales avanzadas', description: 'La nueva version del modelo de lenguaje promete revolucionar la interaccion humano-maquina con comprension visual y auditiva integrada.', date: '2026-02-12', tags: ['IA', 'Tecnologia'] },
    { id: 102, source: 'HackerNews', type: 'widget', title: 'Rust supera a Python en el indice TIOBE por primera vez', description: 'El lenguaje de sistemas gana popularidad gracias a su seguridad de memoria y adopcion en proyectos de infraestructura critica.', date: '2026-02-09', tags: ['Desarrollo', 'Open Source'] },
];