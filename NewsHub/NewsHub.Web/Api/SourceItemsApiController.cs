using Microsoft.AspNetCore.Mvc;
using NewsHub.Services.Services;

namespace NewsHub.Web.Api;

[ApiController]
[Route("api/news")]
public class SourceItemsApiController : ControllerBase
{
    private readonly SourceItemService _sourceItemService;

    public SourceItemsApiController(SourceItemService sourceItemService)
    {
        _sourceItemService = sourceItemService;
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
}