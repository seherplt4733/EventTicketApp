using System.ComponentModel.DataAnnotations;

namespace EventTicketApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Adınız zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen bir yorum yazın.")]
        [Display(Name = "Yorumunuz")]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        [Display(Name = "Puan")]
        public int Rating { get; set; } = 5;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}