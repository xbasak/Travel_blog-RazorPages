using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Blog.Model
{
    public class Post
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(40, ErrorMessage = "Tytuł może mieć maksymalnie 40 znaków.")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Treść jest wymagana.")]
        public string? Content { get; set; }
        public string? Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public string? Category { get; set; }
        public string? MapLink { get; set; }
        public string? VideoLink { get; set; }
        [NotMapped]
        [ValidateNever]
        public ApplicationUser User { get; set; }
        public ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
    }
}
