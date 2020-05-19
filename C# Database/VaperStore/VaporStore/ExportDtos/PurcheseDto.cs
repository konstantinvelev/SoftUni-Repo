using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace VaporStore.ExportDtos
{
    [XmlType("Purchase")]
    public class PurcheseDto
    {
        [Required]
        [RegularExpression("[0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9] [0-9][0-9][0-9][0-9]")]
        [XmlElement("Card")]
        public string Card { get; set; }

        [Required]
        [RegularExpression("[0-9]{3}")]
        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date { get; set; }

        [Required]
        [XmlElement("Game")]
        public GameDto Game { get; set; }
    }
}