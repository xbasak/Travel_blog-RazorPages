using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Blog.Data;
using Travel_Blog.Model;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class EditPostModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditPostModel(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _environment = environment;
        _configuration = configuration;
        _userManager = userManager;
    }

    [BindProperty]
    public Post Post { get; set; }

    [BindProperty]
    public IFormFileCollection UploadedImages { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Post = await _context.Posts
            .Include(p => p.PostImages)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (Post == null)
        {
            return NotFound();
        }

        ViewData["TinyMCEApiKey"] = _configuration["TinyMCE:ApiKey"];

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var postToUpdate = await _context.Posts
            .Include(p => p.PostImages)
            .FirstOrDefaultAsync(p => p.Id == Post.Id);
        if (postToUpdate == null)
        {
            return NotFound();
        }
        postToUpdate.UserId = currentUser.Id;
        //if (!ModelState.IsValid)
        //{
        //    foreach (var modelState in ModelState.Values)
        //    {
        //        foreach (var error in modelState.Errors)
        //        {
        //            Console.WriteLine($"Walidacja b³êdu: {error.ErrorMessage}");
        //        }
        //    }
        //    return Page();
        //}

        // Aktualizacja pól posta
        postToUpdate.Title = Post.Title;
        postToUpdate.Content = Post.Content;
        postToUpdate.MapLink = Post.MapLink;
        postToUpdate.VideoLink = Post.VideoLink;

        // Przetwarzanie nowych zdjêæ
        if (UploadedImages != null && UploadedImages.Count > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var image in UploadedImages)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                postToUpdate.PostImages.Add(new PostImage
                {
                    ImageUrl = "/uploads/" + uniqueFileName,
                    Post = postToUpdate
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("/PostDetails", new { id = Post.Id });
    }
}
