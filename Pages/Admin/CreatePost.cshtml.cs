using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Travel_Blog.Data;
using Travel_Blog.Model;

namespace Travel_Blog.Pages.Admin
{
    [Authorize]
    public class CreatePostModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public IFormFileCollection UploadedImages { get; set; }

        public CreatePostModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _environment = webHostEnvironment;
        }

        [BindProperty]
        public Post Post { get; set; }
        [BindProperty]                
        public string? UserId { get; set; }

        public IActionResult OnGet()
        {
            ViewData["TinyMCEApiKey"] = _configuration["TinyMCE:ApiKey"];
            UserId = _userManager.GetUserId(User);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            Post.UserId = currentUser.Id;
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Walidacja b³êdu: {error.ErrorMessage}");
                    }
                }
                ViewData["TinyMCEApiKey"] = _configuration["TinyMCE:ApiKey"];
                return Page();
            }

            


            Post.CreatedDate = DateTime.Now;
            _context.Posts.Add(Post);

            if (UploadedImages != null && UploadedImages.Count > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                foreach (var image in UploadedImages)
                {
                    //var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    var postImage = new PostImage
                    {
                        ImageUrl = "/uploads/" + uniqueFileName,
                        Post = Post
                    };

                    _context.PostImage.Add(postImage);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
