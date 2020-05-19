using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
   public class ExportUserDto
    {
        [XmlElement("count")]
        public int UsersCount { get; set; }

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }

        [XmlElement("SoldProducts")]
        public ExportSoldProductsDto SoldProducts { get; set; }

    }
}
