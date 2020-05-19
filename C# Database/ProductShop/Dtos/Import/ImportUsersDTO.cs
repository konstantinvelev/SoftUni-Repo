namespace ProductShop.Dtos
{
    using System.Xml.Serialization;


    [XmlType("User")]
   public class ImportUsersDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }
    }
}
