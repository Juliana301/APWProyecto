using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Common.Models.Import;
using NewsHub.Application.Interfaces;
using NewsHub.Application.Interfaces.Services;
using Newtonsoft.Json;

namespace NewsHub.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportApiController : ControllerBase
    {
        private readonly ISourceService _sourceService;

        public ImportExportApiController(
            ISourceService sourceService)
        {
            _sourceService = sourceService;
        }

        /// <summary>
        /// Exporta una noticia individual en formato JSON propio
        /// </summary>
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

            // 🔹 Formato JSON propio NewsHub
            var exportObject =
                new
                {
                    newsHub = new
                    {
                        format = "NH-1.0",
                        exportedAt =
                            DateTime.UtcNow
                    },

                    source = new
                    {
                        id = item.SourceId,
                        name = item.SourceName,
                        type =
                            item.SourceType
                                .ToString()
                    },

                    article = new
                    {
                        uniqueId =
                            item.UniqueId,

                        title =
                            item.Title,

                        description =
                            item.Description,

                        url =
                            item.Url,

                        publishedAt =
                            item.PublishedAt
                    },

                    classification = new
                    {
                        categories =
                            item.Category
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
                new StreamReader(file.OpenReadStream()))
            {
                json =
                    await reader.ReadToEndAsync();
            }

            NewsHubImportModel? model;

            try
            {
                model =
                    JsonConvert.DeserializeObject
                    <NewsHubImportModel>(json);
            }
            catch
            {
                return BadRequest(
                    "JSON inválido.");
            }

            if (model == null)
            {
                return BadRequest(
                    "No se pudo leer el archivo.");
            }

            // 🔹 Validar formato NewsHub
            if (model.newsHub.format != "NH-1.0")
            {
                return BadRequest(
                    "Formato no compatible.");
            }

            // 🔹 Aquí puedes guardar en BD
            // EJEMPLO base:

            var importedNews =
                new
                {
                    SourceName =
                        model.source.name,

                    Title =
                        model.article.title,

                    Description =
                        model.article.description,

                    Url =
                        model.article.url,

                    PublishedAt =
                        model.article.publishedAt,

                    Categories =
                        model.classification.categories
                };

            // ⚠️ Aquí debes llamar tu servicio real
            // Ejemplo:

            /*
            await _savedNewsService
                .SaveImportedNewsAsync(
                    importedNews);
            */

            return Ok(new
            {
                message =
                    "Noticia importada correctamente.",
                title =
                    model.article.title
            });
        }
    }
}