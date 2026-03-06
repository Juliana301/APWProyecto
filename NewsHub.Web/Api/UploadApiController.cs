using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Api;

[ApiController]
[Route("api/upload")]
public class UploadApiController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not provided");

        var path = Path.Combine("uploads", file.FileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { message = "File uploaded successfully" });
    }
}