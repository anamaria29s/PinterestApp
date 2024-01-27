using PinterestApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinterestApp.Models
{
    // tabelul asociativ care face legatura intre Article si Bookmark
    // un articol are mai multe colectii din care face parte
    // iar o colectie contine mai multe articole in cadrul ei
    public class BookmarkCollection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // cheie primara compusa (Id, ArticleId, BookmarkId)
        public int Id { get; set; }
        public int? BookmarkId { get; set; }
        public int? CollectionId { get; set; }

        public virtual Bookmark? Bookmark { get; set; }
        public virtual Collection? Collection { get; set; }

        public DateTime BookmarkDate { get; set; }
    }
}