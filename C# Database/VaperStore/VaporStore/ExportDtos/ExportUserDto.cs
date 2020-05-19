using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace VaporStore.ExportDtos
{
    [XmlType("User")]
    public class ExportUserDto
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        [XmlAttribute("username")]
        public string Username { get; set; }

        [Required]
        [XmlArray("Purchases")]
        public PurcheseDto[] Purchases { get; set; }

        [Required]
        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }
    }
}
