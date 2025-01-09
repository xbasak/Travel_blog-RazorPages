using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Travel_Blog.Model;

namespace Travel_Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImage { get; set; }
        public DbSet<Comment> Comment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Wywołanie bazy (wymagane dla Identity)
            base.OnModelCreating(modelBuilder);

            // Konfiguracja relacji między Post a ApplicationUser
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User) // Nawigacja do ApplicationUser
                .WithMany() // Jeśli użytkownik może mieć wiele postów
                .HasForeignKey(p => p.UserId) // Klucz obcy w tabeli Post
                .OnDelete(DeleteBehavior.Cascade); // Usunięcie postów użytkownika przy jego usunięciu

            modelBuilder.Entity<PostImage>()
                .HasOne(i => i.Post)
                .WithMany(p => p.PostImages)
                .HasForeignKey(i => i.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja Comment → Post
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Usuwanie komentarzy przy usunięciu posta

            // Relacja Comment → User 
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Zatrzymaj usuwanie użytkownika, jeśli istnieją komentarze
        }
    }
}