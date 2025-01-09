using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Travel_Blog.Data;
using Travel_Blog.Model;

[Authorize]
public class DeletePostModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public DeletePostModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    [BindProperty]
    public Post Post { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Post = await _context.Posts.Include(p => p.PostImages).FirstOrDefaultAsync(p => p.Id == id);

        if (Post == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (Post.UserId != currentUser.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var post = await _context.Posts.Include(p => p.PostImages).FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (post.UserId != currentUser.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // Usuñ powi¹zane obrazy
        foreach (var image in post.PostImages)
        {
            var imagePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}
