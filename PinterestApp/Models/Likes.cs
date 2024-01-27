namespace PinterestApp.Models
{
    public class Likes
    {
        public int Id { get; set; }
        public int? BookmarkId { get; set; }
        public string? UserId { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}