using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.DTOs.Source;
using NewsHub.Application.Interfaces.Services.Source;
using Newtonsoft.Json;

namespace NewsHub.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SourcesItemsApiController : ControllerBase
    {
        private readonly ISourceItemService _sourceItemService;

        public SourcesItemsApiController(
            ISourceItemService sourceItemService)
        {
            _sourceItemService =
                sourceItemService;
        }

        [HttpGet("savedfeed")]
        public async Task<IActionResult>
            GetSavedFeedJson()
        {
            var result =
                await _sourceItemService
                    .GetSavedAsync();

            if (!result.Success ||
                result.Data == null)
            {
                return Ok(new List<object>());
            }

            var json =
                result.Data
                    .OrderByDescending(
                        x => x.CreatedAt)
                    .Select(item =>
                    {
                        if (string.IsNullOrWhiteSpace(
                            item.Json))
                        {
                            return null;
                        }

                        try
                        {
                            dynamic? obj =
                                JsonConvert
                                    .DeserializeObject(
                                        item.Json);

                            if (obj == null)
                            {
                                return null;
                            }

                            // ARREGLAR TAGS (string o array)

                            List<string> tags;

                            if (obj.tags is Newtonsoft.Json.Linq.JArray)
                            {
                                tags =
                                    obj.tags
                                        .ToObject<List<string>>();
                            }
                            else
                            {
                                tags =
                                    new List<string>
                                    {
                                obj.tags?.ToString()
                                    };
                            }

                            return new
                            {
                                id =
                                    (string)obj.id,

                                source =
                                    (string?)obj.source
                                    ?? "NOTFOUND",

                                type =
                                    (string?)obj.type
                                    ?? "unknown",

                                title =
                                    (string)obj.title,

                                description =
                                    (string)obj.description,

                                date =
                                    (string)obj.date,

                                tags =
                                    tags
                            };
                        }
                        catch
                        {
                            // Si un JSON está malo,
                            // simplemente lo ignora
                            return null;
                        }
                    })
                    .Where(x => x != null);

            return Ok(json);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync(
            [FromBody] SavedSourceDto dto)
        {
            if (dto == null)
            {
                return BadRequest("DTO inválido.");
            }

            if (dto.SourceId <= 0)
            {
                return BadRequest("SourceId inválido.");
            }

            var json =
                JsonConvert.SerializeObject(dto);

            var result =
                await _sourceItemService
                    .SaveAsync(
                        dto.SourceId,
                        json);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new
            {
                message = "Noticia guardada.",
                title = dto.title
            });
        }
    }
}