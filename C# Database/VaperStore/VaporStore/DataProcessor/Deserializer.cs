namespace VaporStore.DataProcessor
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using ImportDtos;
    using VaporStore.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Globalization;
    using VaporStore.Data.Models.Enumerations;
    using System.Xml.Serialization;
    using System.IO;

    public static class Deserializer
    {


        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var games = new List<Game>();
            var dtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            foreach (var dto in dtos)
            {
                if (!IsValid(dto) || dto.Tags.Count <= 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var game = new Game
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    ReleaseDate = DateTime.ParseExact(dto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)

                };

                var developer = GetDeveloper(context, dto.Developer);
                var ganre = GetGanre(context, dto.Genre);
                game.Developer = developer;
                game.Genre = ganre;
                foreach (var currTag in dto.Tags)
                {
                    var tag = GetTag(context, currTag);
                    var gameTag = new GameTag
                    {
                        Game = game,
                        Tag = tag
                    };
                    game.GameTags.Add(gameTag); ;
                }
                games.Add(game);
                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }
            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static Tag GetTag(VaporStoreDbContext context, string currTag)
        {
            var tag = context.Tags.FirstOrDefault(s => s.Name == currTag);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = currTag
                };
                context.Tags.Add(tag);
                context.SaveChanges();
            }
            return tag;
        }

        private static Genre GetGanre(VaporStoreDbContext context, string genre)
        {
            var entity = context.Genres.FirstOrDefault(s => s.Name == genre);
            if (entity == null)
            {
                entity = new Genre
                {
                    Name = genre
                };
                context.Genres.Add(entity);
                context.SaveChanges();
            }
            return entity;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string developer)
        {
            var dev = context.Developers.FirstOrDefault(s => s.Name == developer);
            if (dev == null)
            {
                dev = new Developer
                {
                    Name = developer
                };
                context.Developers.Add(dev);
                context.SaveChanges();
            }
            return dev;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, result, true);

            return isValid;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var dtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);
            var users = new List<User>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto) || !dto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                foreach (var card in dto.Cards)
                {
                    var cardType = Enum.TryParse<CardType>(card.Type, out CardType result);
                    if (!cardType)
                    {
                        sb.AppendLine("Invalid Data");
                        break;
                    }
                }

                var user = new User
                {
                    FullName = dto.FullName,
                    Username = dto.Username,
                    Email = dto.Email,
                    Age = dto.Age
                };

                foreach (var card in dto.Cards)
                {
                    user.Cards.Add(new Card
                    {
                        Number = card.Number,
                        Cvc = card.Cvc,
                        Type =Enum.Parse<CardType>(card.Type)
                    });
                }
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
                users.Add(user);
            }
            context.AddRange(users);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xml = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));

            ImportPurchaseDto[] importPurchaseDtos;

            using (var reader = new StringReader(xmlString))
            {
                importPurchaseDtos = (ImportPurchaseDto[])xml.Deserialize(reader);
            }

            var list = new List<Purchase>();

            foreach (var dto in importPurchaseDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var isValidEnum = Enum.TryParse<PurchaseType>(dto.Type, out PurchaseType result);

                if (!isValidEnum)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var card = context.Cards.FirstOrDefault(s => s.Number == dto.Card);
                var game = context.Games.FirstOrDefault(f => f.Name == dto.Title);

                if (card == null || game == null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var purchasse = new Purchase
                {
                    ProductKey = dto.Key,
                    Date = DateTime.ParseExact(dto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Card = card,
                    Game = game,
                    Type = Enum.Parse<PurchaseType>(dto.Type)
                };
                list.Add(purchasse);
                sb.AppendLine($"Imported {purchasse.Game.Name} for {purchasse.Card.User.Username}");
            }
            context.Purchases.AddRange(list);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
    }
}