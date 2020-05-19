using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models.Enumerations;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Purchases = new List<Purchase>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression("[0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9]")]
        public string Number { get; set; }

        [Required]
        [RegularExpression("[0-9]{3}")]
        public string Cvc { get; set; }

        [Required]
        public CardType Type { get; set; }

        public User User { get; set; }
        [Required]
        public int UserId { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }
}
