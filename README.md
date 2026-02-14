# APWProyecto/

## Estructura del Proyecto
```
APWProyecto/
│
├── 📁 .github/
│
├── 📁 NewsHub/                         # Contenedor de la solución
│   │
│   ├── 📁 NewsHub.Web/                 # Web MVC + API integrada
│   │   ├── 📁 Controllers/             # Controladores MVC (HTML)
│   │   │   ├── HomeController.cs
│   │   │   ├── SourcesController.cs
│   │   │   ├── SourceItemsController.cs
│   │   │   ├── AdminController.cs
│   │   │   └── SettingsController.cs
│   │   │
│   │   ├── 📁 Api/                     # Controladores REST (JSON)
│   │   │   ├── SourcesApiController.cs
│   │   │   ├── SourceItemsApiController.cs
│   │   │   ├── UploadApiController.cs
│   │   │   └── DownloadApiController.cs
│   │   │
│   │   ├── 📁 ViewModels/
│   │   ├── 📁 Views/
│   │   ├── 📁 Areas/
│   │   │   └── Identity/
│   │   ├── 📁 wwwroot/
│   │   ├── appsettings.json
│   │   ├── Program.cs
│   │   └── NewsHub.Web.csproj
│   │
│   ├── 📁 NewsHub.Domain/              # Entidades y reglas
│   │   ├── 📁 Entities/
│   │   │   ├── Source.cs
│   │   │   ├── SourceItem.cs
│   │   │   └── Secret.cs
│   │   ├── 📁 Interfaces/
│   │   ├── 📁 Enums/
│   │   └── NewsHub.Domain.csproj
│   │
│   ├── 📁 NewsHub.Services/            # Casos de uso
│   │   ├── 📁 Services/
│   │   │   ├── SourceService.cs
│   │   │   ├── SourceItemService.cs
│   │   │   ├── ParsingService.cs
│   │   │   ├── UploadService.cs
│   │   │   └── DownloadService.cs
│   │   ├── 📁 DTOs/
│   │   ├── 📁 Interfaces/
│   │   └── NewsHub.Services.csproj
│   │
│   ├── 📁 NewsHub.Infrastructure/      # Base de datos + APIs externas
│   │   ├── 📁 Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Configurations/
│   │   ├── 📁 Repositories/
│   │   ├── 📁 External/
│   │   ├── 📁 Parsers/
│   │   └── NewsHub.Infrastructure.csproj
│   │
│   └── NewsHub.slnx
│
├── .gitignore
├── README.md
└── docker-compose.yml (opcional)
```

## Descripción de las Capas

### NewsHub.Web
Web MVC + API integrada con controladores para vistas HTML y endpoints REST JSON.

### NewsHub.Domain
Entidades del dominio y reglas de negocio.

### NewsHub.Services
Capa de casos de uso y lógica de aplicación.

### NewsHub.Infrastructure
Implementación de acceso a datos, repositorios y servicios externos.
