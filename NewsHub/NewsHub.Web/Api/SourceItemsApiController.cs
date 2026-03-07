using Microsoft.AspNetCore.Mvc;
using NewsHub.Services.Services;

namespace NewsHub.Web.Api;

[ApiController]
[Route("api/news")]
public class SourceItemsApiController : ControllerBase
{
    private readonly SourceItemService _sourceItemService;
    private readonly ParsingService _parsingService;

    public SourceItemsApiController(
        SourceItemService sourceItemService,
        ParsingService parsingService)
    {
        _sourceItemService = sourceItemService;
        _parsingService = parsingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNews()
    {
        var news = await _sourceItemService.GetAllAsync();
        return Ok(news);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsById(int id)
    {
        var item = await _sourceItemService.GetByIdAsync(id);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost("fetch")]
    public async Task<IActionResult> FetchNews()
    {
        await _parsingService.FetchAllSourcesAsync();

        return Ok(new
        {
            message = "News fetched successfully"
        });
    }
}