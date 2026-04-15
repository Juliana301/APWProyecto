using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Common.Models.Import;
using NewsHub.Application.Interfaces.Services.Source;
using Newtonsoft.Json;

namespace NewsHub.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportApiController : ControllerBase
    {
        private readonly ISourceService _sourceService;
        private readonly ISourceItemService _sourceItemService;

        public ImportExportApiController(
            ISourceService sourceService,
            ISourceItemService sourceItemService)
        {
            _sourceService = sourceService;
            _sourceItemService = sourceItemService;
        }

        // =====================================================
        // EXPORT (FORMATO OFICIAL)
        // =====================================================

        [HttpGet("export/news/{uniqueId}")]
        public async Task<IActionResult> ExportNewsJson(
            string uniqueId)
        {
            var result =
                await _sourceService
                    .ReadAllFeedsAsync();

            if (!result.Success ||
                result.Data == null)
            {
                return NotFound(
                    "No se pudieron obtener noticias.");
            }

            var item =
                result.Data
                    .FirstOrDefault(x =>
                        x.UniqueId == uniqueId);

            if (item == null)
            {
                return NotFound(
                    "Noticia no encontrada.");
            }

            // EXPORT FORMATO OFICIAL
            var exportObject =
                new
                {
                    schemaVersion =
                        "edu.univ.ingest.v1",

                    exportedAt =
                        DateTime.UtcNow,

                    source = new
                    {
                        id =
                            item.SourceId
                                .ToString(),

                        name =
                            item.SourceName,

                        type =
                            item.SourceType
                                .ToString()
                                .ToLower(),

                        url =
                            item.Url,

                        requiresSecret =
                            false
                    },

                    normalized = new
                    {
                        id =
                            item.UniqueId,

                        externalId =
                            item.UniqueId,

                        title =
                            item.Title,

                        content =
                            item.Description,

                        summary =
                            item.Description,

                        publishedAt =
                            item.PublishedAt,

                        url =
                            item.Url,

                        author =
                            (string?)null,

                        language =
                            "es",

                        category = new
                        {
                            primary =
                                item.Category,

                            secondary =
                                new string[] { }
                        }
                    },

                    raw = new
                    {
                        format = "json",

                        data = new
                        {
                            original =
                                item.Description
                        }
                    }
                };

            var json =
                JsonConvert.SerializeObject(
                    exportObject,
                    Formatting.Indented);

            var bytes =
                System.Text.Encoding
                    .UTF8
                    .GetBytes(json);

            var safeTitle =
                item.Title
                    .Replace(" ", "_")
                    .Replace("/", "")
                    .Replace("\\", "");

            var fileName =
                $"news_{safeTitle}_{item.UniqueId}.json";

            return File(
                bytes,
                "application/json",
                fileName);
        }

        // =====================================================
        // IMPORT
        // =====================================================

        [HttpPost("import/news")]
        public async Task<IActionResult> ImportNewsJson(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(
                    "Archivo no válido.");
            }

            string json;

            using (var reader =
                new StreamReader(
                    file.OpenReadStream()))
            {
                json =
                    await reader.ReadToEndAsync();
            }

            try
            {
                // Detectar formato

                if (json.Contains("schemaVersion"))
                {
                    return await ImportOfficialFormat(json);
                }

                // Se pueden agregar más formatos aquí

                return BadRequest(
                    "Formato JSON no reconocido.");
            }
            catch (Exception ex)
            {
                return BadRequest(
                    $"Error procesando archivo: {ex.Message}");
            }
        }

        // =====================================================
        // IMPORT FORMATO OFICIAL
        // =====================================================

        private async Task<IActionResult>
    ImportOfficialFormat(string json)
        {
            var model =
                JsonConvert.DeserializeObject
                    <OfficialImportModel>(json);

            if (model == null)
            {
                return BadRequest(
                    "JSON oficial inválido.");
            }

            if (model.schemaVersion !=
                "edu.univ.ingest.v1")
            {
                return BadRequest(
                    "Versión de schema no soportada.");
            }

            if (model.source == null)
            {
                return BadRequest(
                    "Source inválido.");
            }

            if (model.normalized == null)
            {
                return BadRequest(
                    "Normalized inválido.");
            }

            var importedNews =
                new
                {
                    id =
                        model.normalized.id,

                    source =
                        model.source.name,

                    type =
                        model.source.type,

                    title =
                        model.normalized.title,

                    description =
                        model.normalized.summary,

                    date =
                        model.normalized
                            .publishedAt
                            .ToString("yyyy-MM-dd"),

                    tags =
                        new[]
                        {
                            model.normalized
                                .category.primary
                        }
                        .Concat(
                            model.normalized
                                .category.secondary ?? []
                        )
                        .ToArray()
                };

            var savedJson =
                JsonConvert.SerializeObject(
                    importedNews);

            // SOURCE FIJO (temporal)
            var sourceId = 1;

            var result =
                await _sourceItemService
                    .SaveAsync(
                        sourceId,
                        savedJson);

            if (!result.Success)
            {
                return BadRequest(
                    "Error guardando noticia.");
            }

            return Ok(new
            {
                message =
                    "Importación oficial exitosa.",
                title =
                    model.normalized.title
            });
        }
    }
}