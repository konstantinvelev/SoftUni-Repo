using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
   public class ExportSoldProductsDto
    {
        [XmlElement("count")]
        public int SoldProductCount { get; set; }

        [XmlArray("products")]
        public ExportProductsDto[] ProductsDtos { get; set; }
    }
}
