using PinterestApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


// PASUL 3 - useri si roluri

namespace PinterestApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<BookmarkCollection> BookmarkCollections { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // definirea relatiei many-to-many dintre Article si Bookmark

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<BookmarkCollection>()
                .HasKey(bm => new { bm.Id, bm.BookmarkId, bm.CollectionId });


            // definire relatii cu modelele Bookmark si Article (FK)

            modelBuilder.Entity<BookmarkCollection>()
                .HasOne(bm => bm.Bookmark)
                .WithMany(bm => bm.BookmarkCollections)
                .HasForeignKey(bm => bm.BookmarkId);

            modelBuilder.Entity<BookmarkCollection>()
                .HasOne(bm => bm.Collection)
                .WithMany(bm => bm.BookmarkCollections)
                .HasForeignKey(bm => bm.CollectionId);
        }
    }
}