namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportMoviesDto[]>(jsonString);

            var list = new List<Movie>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                var isGener = Enum.TryParse<Genre>(dto.Genre, out Genre gen);

                if (IsValid(dto) && isGener)
                {
                    var movie = new Movie
                    {
                        Title = dto.Title,
                        Genre = Enum.Parse<Genre>(dto.Genre),
                        Director = dto.Director,
                        Duration = TimeSpan.ParseExact(dto.Duration, @"hh\:mm\:ss", CultureInfo.InvariantCulture),
                        Rating = dto.Rating
                    };

                    if (list.Contains(movie))
                    {
                        sb.AppendLine(ErrorMessage);
                    }
                    else
                    {
                        list.Add(movie);
                        sb.AppendLine($"Successfully imported {movie.Title} with genre {movie.Genre} and rating {movie.Rating:F2}!");
                    }
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Movies.AddRange(list);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);

            var halls = new List<Hall>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if (IsValid(dto))
                {
                    var hall = new Hall
                    {
                        Is3D = dto.Is3D,
                        Is4Dx = dto.Is4Dx,
                        Name = dto.Name
                    };

                    var seats = new List<Seat>();

                    for (int i = 0; i < dto.Seats; i++)
                    {
                        var seat = new Seat
                        {
                            Hall = hall
                        };
                        seats.Add(seat);
                    }
                    hall.Seats = seats;

                    halls.Add(hall);

                    if (hall.Is4Dx && hall.Is3D)
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(4Dx/3D) with {hall.Seats.Count} seats!");
                    }
                    else if (hall.Is3D)
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(3D) with {hall.Seats.Count} seats!");
                    }
                    else if (hall.Is4Dx)
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(4Dx) with {hall.Seats.Count} seats!");
                    }
                    else
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(Normal) with {hall.Seats.Count} seats!");
                    }
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var xml = new XmlSerializer(typeof(ImportProjectsDto[]), new XmlRootAttribute("Projections"));

            ImportProjectsDto[] importProjectsDtos;

            using (var reader = new StringReader(xmlString))
            {
                importProjectsDtos = (ImportProjectsDto[])xml.Deserialize(reader);
            }

            var projections = new List<Projection>();

            var sb = new StringBuilder();

            foreach (var dto in importProjectsDtos)
            {
                var movie = context.Movies.FirstOrDefault(s => s.Id == dto.MovieId);
                var hall = context.Halls.FirstOrDefault(s => s.Id == dto.HallId);

                if (IsValid(dto) && movie != null && hall != null)
                {
                    var profection = new Projection
                    {
                        MovieId = dto.MovieId,
                        HallId = dto.HallId,
                        DateTime = DateTime.ParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture)
                    };

                    projections.Add(profection);
                    sb.AppendLine($"Successfully imported projection {movie.Title} on {profection.DateTime.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture)}!");
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Projections.AddRange(projections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xml = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            ImportCustomerDto[] importCustomerDtos;

            using (var reader = new StringReader(xmlString))
            {
                importCustomerDtos = (ImportCustomerDto[])xml.Deserialize(reader);
            }

            var sb = new StringBuilder();
            var customers = new List<Customer>();

            foreach (var dto in importCustomerDtos)
            {
                if (IsValid(dto))
                {
                    bool isError = false;
                    var tickets = new List<Ticket>();

                    foreach (var item in dto.Tickets)
                    {
                        if (IsValid(item))
                        {
                            var tiket = new Ticket
                            {
                                ProjectionId = item.ProjectionId,
                                Price = item.Price
                            };
                            tickets.Add(tiket);
                        }
                        else
                        {
                            isError = true;
                            sb.AppendLine(ErrorMessage);
                            break;
                        }
                    }

                    if (isError)
                    {
                        continue;
                    }

                    var customer = new Customer
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Age = dto.Age,
                        Balance = dto.Balance,
                        Tickets = tickets
                    };

                    customers.Add(customer);
                    sb.AppendLine
                    ($"Successfully imported customer {customer.FirstName} {customer.LastName} with bought tickets: {customer.Tickets.Count}!");
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, result, true);

            return isValid;
        }
    }
}