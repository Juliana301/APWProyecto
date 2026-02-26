# APWProyecto/

## Estructura del Proyecto
```
APWProyecto/
│
├── 📁 .github/
│
├── 📁 NewsHub/
│   │
│   ├── 📁 NewsHub.Web/                     # Presentación (MVC + API)
│   │   ├── 📁 Controllers/                 # MVC Controllers (HTML)
│   │   │   ├── HomeController.cs
│   │   │   ├── SourcesController.cs
│   │   │   ├── SourceItemsController.cs
│   │   │   ├── AdminController.cs
│   │   │   └── SettingsController.cs
│   │   │
│   │   ├── 📁 Api/                         # API REST (JSON)
│   │   │   ├── SourcesApiController.cs
│   │   │   ├── SourceItemsApiController.cs
│   │   │   └── ImportExportApiController.cs   # Unificado upload/download
│   │   │
│   │   ├── 📁 Filters/                     # Filtros personalizados (Roles, Logs)
│   │   ├── 📁 Middlewares/                 # Manejo global de errores
│   │   ├── 📁 ViewModels/
│   │   ├── 📁 Views/
│   │   ├── 📁 Areas/
│   │   │   └── Identity/
│   │   ├── 📁 wwwroot/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs
│   │   └── NewsHub.Web.csproj
│   │
│   ├── 📁 NewsHub.Domain/                  # Núcleo del negocio (NO depende de nadie)
│   │   ├── 📁 Entities/
│   │   │   ├── Source.cs
│   │   │   ├── SourceItem.cs
│   │   │   ├── Secret.cs
│   │   │   └── UserRole.cs
│   │   │
│   │   ├── 📁 Interfaces/
│   │   │   ├── IRepository.cs
│   │   │   ├── ISourceReader.cs            # Strategy Pattern
│   │   │   ├── ISourceService.cs
│   │   │   └── IUnitOfWork.cs
│   │   │
│   │   ├── 📁 Enums/
│   │   │   ├── SourceType.cs               # Api, Feed, Html, Widget
│   │   │   └── Roles.cs
│   │   │
│   │   └── NewsHub.Domain.csproj
│   │
│   ├── 📁 NewsHub.Application/             # (ANTES Services) Casos de uso limpios
│   │   ├── 📁 DTOs/
│   │   │   ├── SourceDto.cs
│   │   │   ├── SourceItemDto.cs
│   │   │   └── ImportExportDto.cs
│   │   │
│   │   ├── 📁 Interfaces/
│   │   │   ├── ISourceAppService.cs
│   │   │   ├── ISourceItemAppService.cs
│   │   │   └── IImportExportService.cs
│   │   │
│   │   ├── 📁 Services/
│   │   │   ├── SourceAppService.cs
│   │   │   ├── SourceItemAppService.cs
│   │   │   └── ImportExportService.cs
│   │   │
│   │   └── NewsHub.Application.csproj
│   │
│   ├── 📁 NewsHub.Infrastructure/          # Implementaciones técnicas
│   │   ├── 📁 Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── SeedData.cs
│   │   │   └── Configurations/
│   │   │
│   │   ├── 📁 Repositories/
│   │   │   ├── Repository.cs
│   │   │   ├── SourceRepository.cs
│   │   │   ├── SourceItemRepository.cs
│   │   │   └── UnitOfWork.cs
│   │   │
│   │   ├── 📁 External/                    # Consumo APIs externas
│   │   │   ├── ApiSourceReader.cs
│   │   │   ├── XmlFeedSourceReader.cs
│   │   │   ├── HtmlScraperSourceReader.cs
│   │   │   └── SourceReaderFactory.cs      # Factory + Strategy
│   │   │
│   │   ├── 📁 Security/
│   │   │   ├── SecretManager.cs
│   │   │   └── EncryptionService.cs
│   │   │
│   │   ├── 📁 Parsing/
│   │   │   ├── JsonParser.cs
│   │   │   ├── XmlParser.cs
│   │   │   └── HtmlParser.cs
│   │   │
│   │   └── NewsHub.Infrastructure.csproj
│   │
│   └── NewsHub.sln
│
├── .gitignore
├── README.md
└── docker-compose.yml
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
