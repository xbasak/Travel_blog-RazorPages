using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
    [BindProperty]
    public Post Post { get; set; }
    public List<Comment> Comment { get; set; } = new List<Comment>();
    public ApplicationUser PostOwner { get;set; }

    [BindProperty]
    public string Content { get; set; }
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Post = await _context.Posts
                             .Include(p => p.PostImages)
                             .Include(p => p.Comments)
                             .ThenInclude(c => c.User)
                             .FirstOrDefaultAsync(p => p.Id == id);
        GetPostOwner(Post.UserId);
        if (Post == null)
        {
            return NotFound();
        }

        Comment = Post.Comments.Reverse().ToList();

        return Page();
    }

    public void GetPostOwner(string userId)
    {
        PostOwner = _userManager.Users.FirstOrDefault(x => x.Id == userId) ?? null;
    }

    public async Task<IActionResult> OnPostAddCommentAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (!Validation())
        {
            return RedirectToPage(new { id });
        }
            

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

        if (comment == null)
        {
            return NotFound();
        }

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == comment.PostId);
        if (post == null)
        {
            return NotFound();
        }

        if (!Validation())
        {
            return RedirectToPage(new { id = comment.PostId });
        }

        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        bool isPostOwner = post.UserId == currentUserId;
        bool isCommentOwner = comment.UserId == currentUserId;
        bool isAdmin = User.IsInRole("Admin");

        if (!isCommentOwner && !isPostOwner && !isAdmin)
        {
            return Forbid();
        }

        _context.Comment.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = post.Id });
    }

    public async Task<IActionResult> OnPostEditCommentAsync(int commentId, string updatedContent)
    {
        var comment = await _context.Comment.FindAsync(commentId);

        if (comment == null)
        {
            return NotFound();
        }

        if (!Validation())
            return RedirectToPage(new { id = comment.PostId });


        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (comment.UserId != currentUserId)
        {
            return Forbid();
        }

        comment.Content = updatedContent;
        comment.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = comment.PostId });
    }

    public bool Validation()
    {
        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Walidacja b³êdu: {error.ErrorMessage}");
                }
            }
            return false;
        }
        return true;
    }
}
