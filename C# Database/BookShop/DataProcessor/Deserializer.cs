namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var xml = new XmlSerializer(typeof(ImportBooksDto[]), new XmlRootAttribute("Books"));

            ImportBooksDto[] importBooksDtos;

            using (var reader = new StringReader(xmlString))
            {
                importBooksDtos = (ImportBooksDto[])xml.Deserialize(reader);
            }

            var books = new List<Book>();

            var sb = new StringBuilder();

            foreach (var dto in importBooksDtos)
            {

                if (Convert.ToInt32(dto.Genre) > 3)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (IsValid(dto))
                {
                    var book = new Book
                    {
                        Name = dto.Name,
                        Pages = dto.Pages,
                        Genre = Enum.Parse<Genre>(dto.Genre),
                        Price = dto.Price,
                        PublishedOn = DateTime.ParseExact(dto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                    };

                    books.Add(book);
                    sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Books.AddRange(books);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportAuthorsDto[]>(jsonString);

            var authorsEmails = new List<string>();

            var authors = new List<Author>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                var notImport = false;
                if (IsValid(dto))
                {
                    if (authorsEmails.Contains(dto.Email))
                    {
                        sb.AppendLine(ErrorMessage);
                        notImport = true;
                    }

                    var author = new Author
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Phone = dto.Phone,
                        Email = dto.Email
                    };

                    var books = new List<AuthorBook>();

                    foreach (var item in dto.Books)
                    {
                        if (IsValid(item))
                        {
                            var book = context.Books.FirstOrDefault(s => s.Id == Convert.ToInt32(item.Id));

                            if (book == null)
                            {
                                continue;
                            }

                            var authorBook = new AuthorBook
                            {
                                AuthorId = author.Id,
                                BookId = book.Id
                            };

                            books.Add(authorBook);
                        }
                    }
                    if (notImport)
                    {
                        context.AuthorsBooks.AddRange(books);
                        continue;
                    }

                    if (books.Count <= 0)
                    {
                        sb.AppendLine(ErrorMessage);
                        context.AuthorsBooks.AddRange(books);
                        continue;
                    }

                    author.AuthorsBooks = books;

                    authors.Add(author);
                    sb.AppendLine(string.Format(SuccessfullyImportedAuthor, author.FirstName + " " + author.LastName, author.AuthorsBooks.Count));

                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Authors.AddRange(authors);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

    }
}