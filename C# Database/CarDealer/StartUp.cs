using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                //foreach (var item in db.Cars)
                //{
                //    db.Cars.Remove(item);
                //}
                
                //var k = File.ReadAllText("./../../../Datasets/cars.json");
                Console.WriteLine(GetCarsWithTheirListOfParts(db));
            }
        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var objects = JsonConvert.DeserializeObject<Part[]>(inputJson);

         
            foreach (var item in objects)
            {
                var suppler = context.Suppliers.FirstOrDefault(s => s.Id == item.SupplierId);

                if ((suppler == null))
                {
                    continue;
                }

                if (!suppler.IsImporter)
                {
                    continue;
                }
                context.Parts.Add(item);

            }

            return $"Successfully imported {objects.Length}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var objects = JsonConvert.DeserializeObject<Car[]>(inputJson);
            int i = 1;

            foreach (var item in objects)
            {
                if (context.Cars.Contains(item))
                {
                    continue;
                }

                item.Id = 1;

                context.Cars.Add(item);
            }

            context.SaveChanges();

            return $"Successfully imported {objects.Length}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var obj = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            foreach (var item in obj)
            {
                context.Customers.Add(item);
            }

            context.SaveChanges();

            return $"Successfully imported {obj.Length}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var kk = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            foreach (var item in kk)
            {
                context.Sales.Add(item);
            }

            context.SaveChanges();

            return $"Successfully imported {kk.Length}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(o => o.BirthDate)
                .ThenByDescending(t => t.IsYoungDriver)
                .Select(e => new
                {
                    Name = e.Name,
                    BirthDate = $"{e.BirthDate.Day}/{e.BirthDate.Month}/{e.BirthDate.Year}",
                    IsYoungDriver = e.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(o => o.Model)
                .OrderByDescending(p => p.TravelledDistance)
                .Select(e => new
                {
                    e.Id,
                    e.Make,
                    e.Model,
                    e.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supplier = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(e => new
                {
                    Id = e.Id,
                    Name = e.Name,
                    PartsCount = e.Parts.Count
                })
                .ToArray();

            var jspn = JsonConvert.SerializeObject(supplier, Formatting.Indented);

            return jspn;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Include(c => c.PartCars)
                .ThenInclude(c => c.Part)
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },

                    parts = c.PartCars
                    .Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = $"{p.Part.Price:F2}"
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }
    }
}