using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Interfaces.Services.Source;

namespace NewsHub.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SourcesApiController : ControllerBase
    {
        private readonly ISourceService _sourceService;

        public SourcesApiController(
            ISourceService sourceService)
        {
            _sourceService = sourceService;
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeedJson(CancellationToken cancellationToken)
        {
            var result = await _sourceService.ReadAllFeedsAsync(cancellationToken);

            if (!result.Success || result.Data == null)
            {
                return Ok(new List<object>());
            }

            var json = result.Data
                .OrderByDescending(x => x.PublishedAt)
                .Select(item => new
                {
                    id = item.UniqueId,
                    sourceId = item.SourceId,
                    source = item.SourceName ?? "NOTFOUND",
                    type = item.SourceType.ToString(),
                    title = item.Title,
                    description = item.Description,
                    date = item.PublishedAt.ToString("yyyy-MM-dd"),
                    tags = item.Category,
                    url = item.Url
                });

            return Ok(json);
        }
    }
}