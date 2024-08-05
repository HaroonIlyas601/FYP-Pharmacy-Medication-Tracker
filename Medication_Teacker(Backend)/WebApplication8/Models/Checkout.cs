using System.ComponentModel.DataAnnotations;

namespace WebApplication8.Models
{
    public class Checkout
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(15)]
        public string FirstName { get; set; }

        [Required, MaxLength(15)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(10), MaxLength(15)]
        public string Mobile { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public string PaymentMethod { get; set; }
        public string VisaCardNumber { get; set; }
        public string JazzCashNumber { get; set; }
    }
}
