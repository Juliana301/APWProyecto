using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Api;

[ApiController]
[Route("api/download")]
public class DownloadApiController : ControllerBase
{
    [HttpGet("{filename}")]
    public IActionResult DownloadFile(string filename)
    {
        var path = Path.Combine("uploads", filename);

        if (!System.IO.File.Exists(path))
            return NotFound();

        var bytes = System.IO.File.ReadAllBytes(path);

        return File(bytes, "application/octet-stream", filename);
    }
}