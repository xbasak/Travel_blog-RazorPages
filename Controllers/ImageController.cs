using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_Blog.Data;

[Authorize]
[ApiController]
[Route("ImageController/[action]")]
public class RemoveImageController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RemoveImageController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveImage(int id)
    {
        var image = await _context.PostImage.FindAsync(id);

        if (image == null)
        {
            return NotFound();
        }

        _context.PostImage.Remove(image);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
