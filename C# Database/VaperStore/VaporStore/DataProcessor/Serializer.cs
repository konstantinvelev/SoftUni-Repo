namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enumerations;
    using VaporStore.ExportDtos;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var geners = context.Genres
                .Where(s => genreNames.Contains(s.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games
                    .Where(o=>o.Purchases.Count >=1)
                    .Select(s => new
                    {
                        Id = s.Id,
                        Title = s.Name,
                        Developer = s.Developer.Name,
                        Tags = string.Join(", ", s.GameTags.Select(e => e.Tag.Name).ToArray()),
                        Players = s.Purchases.Count
                    })
                    .OrderByDescending(s => s.Players)
                    .ThenBy(s => s.Id)
                    .ToArray(),

                    TotalPlayers = x.Games.Sum(g => g.Purchases.Count)
                })
                .OrderByDescending(s => s.TotalPlayers)
                .ThenBy(s => s.Id)
                .ToArray();

            var json = JsonConvert.SerializeObject(geners, Formatting.Indented);

            return json;

        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var type = Enum.Parse<PurchaseType>(storeType);

            var user = context.Users
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Purchases = u.Cards
                    .SelectMany(x => x.Purchases)
                    .Where(j=>j.Type == type)
                    .Select(c=> new PurcheseDto
                    {
                        Card = c.Card.Number,
                        Cvc = c.Card.Cvc,
                        Date = c.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new GameDto
                        {
                            Genre = c.Game.Genre.Name,
                            Title = c.Game.Name,
                            Price = c.Game.Price
                        }
                    })
                    .OrderBy(g=>g.Date)
                    .ToArray(),
                    TotalSpent = u.Cards.SelectMany(h=>h.Purchases)
                    .Where(l=>l.Type == type)
                    .Sum(i=>i.Game.Price)
                })
                .Where(d=>d.Purchases.Any())
                .OrderByDescending(j=>j.TotalSpent)
                .ThenBy(k=>k.Username)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportUserDto[]),new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xml.Serialize(writer, user, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}