using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Blog.Model
{
    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relacja z użytkownikiem
        public string UserId { get; set; }
        [NotMapped]
        public ApplicationUser User { get; set; }

        // Relacja z postem
        public int PostId { get; set; }
        public Post Post { get; set; }
    }

}
