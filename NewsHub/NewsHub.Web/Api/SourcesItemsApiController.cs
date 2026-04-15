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

                        var data =
                            JsonConvert
                                .DeserializeObject
                                <SavedSourceDto>(
                                    item.Json);

                        if (data == null)
                        {
                            return null;
                        }

                        return new
                        {
                            id =
                                data.id,

                            source =
                                data.source
                                ?? "NOTFOUND",

                            type =
                                data.type
                                ?? "unknown",

                            title =
                                data.title,

                            description =
                                data.description,

                            date =
                                data.date,

                            tags =
                                data.tags
                        };
                    })
                    .Where(x => x != null);

            return Ok(json);
        }
    }
}