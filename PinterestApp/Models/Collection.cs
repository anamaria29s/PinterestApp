using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PinterestApp.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele colectiei este obligatoriu")]
        public string Name { get; set; }

        // o colectie este creata de catre un user
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // relatia many-to-many dintre Article si Bookmark
        public virtual ICollection<BookmarkCollection>? BookmarkCollections { get; set; }

    }
}