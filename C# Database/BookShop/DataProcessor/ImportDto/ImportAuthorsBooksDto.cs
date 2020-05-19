using System.ComponentModel.DataAnnotations;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorsBooksDto
    {
        [Required]
        public string Id { get; set; }
    }
}