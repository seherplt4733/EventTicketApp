using System.ComponentModel.DataAnnotations;

namespace EventTicketApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Tek seferde en fazla 10 bilet alabilirsiniz.")]
        [Display(Name = "Bilet Adedi")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Toplam Tutar")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Satın Alma Tarihi")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Display(Name = "Bilet Kodu")]
        public string TicketCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    }
}