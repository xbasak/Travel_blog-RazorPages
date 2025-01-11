using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Travel_Blog.Data;
using Travel_Blog.Model;

namespace Travel_Blog.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Post> Posts { get; set; }
        public async Task OnGetAsync()
        {
            Posts = await _context.Posts
                                  .Include(p => p.PostImages)
                                  .Include(c => c.User)
                                  .ToListAsync();
            Posts.Reverse();

        }
    }
}
