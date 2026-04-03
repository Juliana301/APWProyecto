using Microsoft.AspNetCore.Mvc;
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

            var fileName =
                $"news_{item.UniqueId}.json";

            return File(
                bytes,
                "application/json",
                fileName);
        }
    }
}