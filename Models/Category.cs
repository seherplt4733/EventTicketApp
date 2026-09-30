using System.ComponentModel.DataAnnotations;

namespace EventTicketApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;

        // Bire Çok İlişki (One-to-Many)
        public List<Event>? Events { get; set; }
    }
}