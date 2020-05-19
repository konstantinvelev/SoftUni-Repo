using System.ComponentModel.DataAnnotations;

namespace VaporStore.ImportDtos
{
    public class CardsDto
    {
        [Required]
        [RegularExpression("[0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9]")]
        public string Number { get; set; }

        [Required]
        [RegularExpression("[0-9]{3}")]
        public string Cvc { get; set; }

        [Required]
        public string Type { get; set; }
    }
}