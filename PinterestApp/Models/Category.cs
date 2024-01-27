using System.ComponentModel.DataAnnotations;

namespace PinterestApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string Name { get; set; }

        public virtual ICollection<Bookmark>? Bookmarks { get; set; }
    }
}
