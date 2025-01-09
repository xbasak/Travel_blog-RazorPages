namespace Travel_Blog.Model
{
    public class PostImage
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? ImageUrl { get; set; }
        public Post? Post { get; set; }
    }
}
