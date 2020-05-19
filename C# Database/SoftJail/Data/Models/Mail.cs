using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        //•	PrisonerId - integer, foreign key
        //•	Prisoner – the mail's Prisoner (required)

        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"^[0-9A-Za-z\s]+ str\.$")]
        public string Address { get; set; }

        [Required]
        public int PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }

    }
}