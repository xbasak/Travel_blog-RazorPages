using Microsoft.AspNetCore.Authentication;
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

        public IActionResult OnGet()
        {
            ViewData["TinyMCEApiKey"] = _configuration["TinyMCE:ApiKey"];
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            var currentUser = await _userManager.GetUserAsync(User);


            Post.CreatedDate = DateTime.Now;
            Post.UserId = currentUser.Id; // Przypisanie u¿ytkownika tworz¹cego post
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

            return RedirectToPage("/Index"); // Przekierowanie na stronê g³ówn¹
        }
    }
}
