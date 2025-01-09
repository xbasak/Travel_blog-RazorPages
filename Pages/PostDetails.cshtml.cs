using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Travel_Blog.Data;
using Travel_Blog.Model;

public class PostDetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostDetailsModel(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Post Post { get; set; }
    public List<Comment> Comment { get; set; } = new List<Comment>();

    [BindProperty]
    public string Content { get; set; }
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Post = await _context.Posts
                             .Include(p => p.PostImages)
                             .Include(p => p.Comments)
                             .ThenInclude(c => c.User)
                             .FirstOrDefaultAsync(p => p.Id == id);

        if (Post == null)
        {
            return NotFound();
        }

        Comment = Post.Comments.ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAddCommentAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var comment = new Comment
        {
            Content = Content,
            CreatedAt = DateTime.Now,
            UserId = currentUser.Id,
            PostId = id
        };

        _context.Comment.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteCommentAsync(int commentId, int id)
    {
        var comment = await _context.Comment.FindAsync(commentId);

        if (comment == null || (comment.UserId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value && !User.IsInRole("Admin") && Post.UserId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value))
        {
            return Forbid();
        }

        _context.Comment.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    //public async Task<IActionResult> OnPostEditCommentAsync(int commentId, string updatedContent, int id)
    //{
    //    var comment = await _context.Comment.FindAsync(commentId);

    //    if (comment == null || comment.UserId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
    //    {
    //        return Forbid();
    //    }

    //    comment.Content = updatedContent;
    //    comment.UpdatedAt = DateTime.Now;

    //    await _context.SaveChangesAsync();

    //    return RedirectToPage(new { id });
    //}

    public async Task<IActionResult> OnPostEditCommentAsync(int commentId, string updatedContent)
    {
        var comment = await _context.Comment.FindAsync(commentId);

        if (comment == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (comment.UserId != currentUserId)
        {
            return Forbid(); // Tylko autor komentarza mo¿e edytowaæ
        }

        comment.Content = updatedContent;
        comment.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = comment.PostId });
    }


}
