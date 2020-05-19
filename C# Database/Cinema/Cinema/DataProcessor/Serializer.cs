namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {

        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            //var movies = context.Movies
            //    .Where(s => s.Rating >= rating && s.Projections.Where(a=>a.Tickets.Count >=1))
            //    .Select(x => new
            //    {
            //        MovieName = x.Title,
            //        Rating = x.Rating.ToString("F2"),
            //        TotalIncomes = (x.Projections.Sum(s => s.Tickets.Sum(e => e.Price))).ToString("F2"),
            //        Customers = x.Projections
            //                .Select(c => c.Tickets
            //                    .Select(q=> new
            //                    {
            //                        FirstName = q.Customer.FirstName,
            //                        LastName = q.Customer.LastName,
            //                        Balance = q.Customer.Balance.ToString("f2")
            //                    })
            //                     .OrderByDescending(b =>decimal.Parse(b.Balance))
            //                     .ThenBy(f=>f.FirstName)
            //                     .ThenBy(l=>l.LastName)
            //                     .ToArray()
            //                 )

            //    })
            //    .OrderByDescending(b => double.Parse(b.Rating))
            //    .ThenByDescending(t=>decimal.Parse(t.TotalIncomes))
            //    .Take(10)
            //    .ToArray();

            //var json = JsonConvert.SerializeObject(movies,Formatting.Indented);

            //return json;
            return "";
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var xml = new XmlSerializer(typeof(ExportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customers = context.Customers
                .Where(s => s.Age >= age)
                .Select(e => new ExportCustomerDto
                {
                    FirstName=e.FirstName,
                    LastName=e.LastName,
                    SpentMoney = e.Tickets.Sum(p=>p.Price).ToString("F2"),
                    SpentTime = SumTime(e.Tickets)
                })
                .OrderByDescending(m=>decimal.Parse(m.SpentMoney))
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();

            var nm = new XmlSerializerNamespaces();
            nm.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xml.Serialize(writer,customers,nm);
            }

            return sb.ToString().TrimEnd();

        }

        private static string SumTime(ICollection<Ticket> tickets)
        {
            var time = new TimeSpan();

            foreach (var item in tickets)
            {
                time += item.Projection.Movie.Duration;
            }
           return time.ToString(@"hh\:mm\:ss");
        }
    }
}