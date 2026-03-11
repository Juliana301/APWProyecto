using Microsoft.AspNetCore.Mvc;
using NewsHub.Services.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Web.Api;

[ApiController]
[Route("api/sources")]
public class SourcesApiController : ControllerBase
{
    private readonly SourceService _sourceService;

    public SourcesApiController(SourceService sourceService)
    {
        _sourceService = sourceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSources()
    {
        var sources = await _sourceService.GetAllSourcesAsync();
        return Ok(sources);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSource(int id)
    {
        var source = await _sourceService.GetSourceByIdAsync(id);

        if (source == null)
            return NotFound();

        return Ok(source);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSource([FromBody] Source source)
    {
        await _sourceService.CreateSourceAsync(source);
        return Ok(source);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSource(int id)
    {
        await _sourceService.DeleteSourceAsync(id);
        return Ok();
    }
}