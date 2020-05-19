namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new ExportAuthorsDto
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks.Select(b => new BooksDto
                    {
                        BookName = b.Book.Name,
                        BookPrice = b.Book.Price.ToString("F2")
                    })
                    .OrderByDescending(s => decimal.Parse(s.BookPrice))
                    .ToArray()
                })
                .ToArray()
                .OrderByDescending(p => p.Books.Length)
                .ThenBy(z => z.AuthorName);

            var json = JsonConvert.SerializeObject(authors, Formatting.Indented);

            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var xml = new XmlSerializer(typeof(ExportBookDto[]), new XmlRootAttribute("Books"));

            var books = context.Books
                .Where(b => b.PublishedOn < date && b.Genre.ToString() == "Science")
                .OrderByDescending(s => s.Pages)
                .ThenByDescending(a => a.PublishedOn)
                .Take(10)
                .ToArray()
                .Select(e => new ExportBookDto
                 {
                     Pages = e.Pages,
                     Name = e.Name,
                     Date = e.PublishedOn.ToString("d", CultureInfo.InvariantCulture)
                 })
                .ToArray();

            var sb = new StringBuilder();

            var nm = new XmlSerializerNamespaces();
            nm.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xml.Serialize(writer, books, nm);
            }

            return sb.ToString();
        }
    }
}