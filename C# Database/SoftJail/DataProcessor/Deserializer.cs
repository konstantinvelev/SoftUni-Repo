namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportDepartmentsDto[]>(jsonString);

            var sb = new StringBuilder();

            var departs = new List<Department>();

            foreach (var dto in dtos)
            {
                var cells = new List<Cell>();

                if (IsValid(dto))
                {
                    bool isError = false;
                    foreach (var item in dto.Cells)
                    {
                        if (IsValid(item))
                        {
                            var cell = new Cell
                            {
                                CellNumber = item.CellNumber,
                                HasWindow = item.HasWindow,
                            };
                            cells.Add(cell);
                        }
                        else
                        {
                            isError = true;
                            sb.AppendLine("Invalid Data");
                            break;
                        }
                    }

                    if (isError)
                    {
                        continue;
                    }

                    var depart = new Department
                    {
                        Name = dto.Name,
                        Cells = cells
                    };

                    departs.Add(depart);
                    sb.AppendLine($"Imported {depart.Name} with {cells.Count} cells");
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                }
            }

            context.Departments.AddRange(departs);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportPrisonersDto[]>(jsonString);

            var sb = new StringBuilder();

            var prisoners = new List<Prisoner>();

            foreach (var dto in dtos)
            {
                bool isErorr = false;
                if (IsValid(dto))
                {
                    var mails = new List<Mail>();

                    var prisoner = new Prisoner
                    {
                        FullName = dto.FullName,
                        Bail = dto.Bail,
                        Age = dto.Age,
                        CellId = dto.CellId,
                        IncarcerationDate = DateTime.ParseExact(dto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Nickname = dto.Nickname,
                        ReleaseDate = SetRealeseDate(dto.ReleaseDate),

                    };

                    foreach (var item in dto.Mails)
                    {
                        if (IsValid(item))
                        {
                            var mail = new Mail
                            {
                                Address = item.Address,
                                Description = item.Description,
                                Sender = item.Sender,
                                Prisoner = prisoner
                            };
                            mails.Add(mail);
                        }
                        else
                        {
                            isErorr = true;
                            sb.AppendLine("Invalid Data");
                            break;
                        }
                    }
                    if (isErorr)
                    {
                        continue;
                    }

                    prisoner.Mails = mails;

                    prisoners.Add(prisoner);
                    sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                }



                else
                {
                    sb.AppendLine("Invalid Data");
                }
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static DateTime SetRealeseDate(string releaseDate)
        {
            if (releaseDate == null)
            {
                return new DateTime();
            }
            return DateTime.ParseExact(releaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var xml = new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));

            var sb = new StringBuilder();

            ImportOfficerDto[] importOfficerDtos;

            using (var reader = new StringReader(xmlString))
            {
                importOfficerDtos = (ImportOfficerDto[])xml.Deserialize(reader);
            }

             var officers = new List<Officer>();

            foreach (var dto in importOfficerDtos)
            {
                var officerPrisoners = new List<OfficerPrisoner>();

                var position = Enum.TryParse(dto.Position.ToString(), out Position pos);
                var weapon = Enum.TryParse(dto.Weapon.ToString(), out Weapon weap);

                if (position == false || weapon == false)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (IsValid(dto))
                {
                    var officer = new Officer
                    {
                        FullName = dto.FullName,
                        Salary = dto.Salary,
                        Position = Enum.Parse<Position>(dto.Position),
                        Weapon = Enum.Parse<Weapon>(dto.Weapon),
                        DepartmentId = dto.DepartmentId
                    };

                    foreach (var item in dto.Prisoners)
                    {
                        var prisoner = context.Prisoners.FirstOrDefault(s => s.Id == item.Id);

                        if (prisoner == null)
                        {
                            sb.AppendLine("Invalid Data");
                            continue;
                        }

                        var oficerPrisoner = new OfficerPrisoner
                        {
                            Prisoner = prisoner,
                            Officer = officer
                        };

                        officerPrisoners.Add(oficerPrisoner);
                    }

                    officer.OfficerPrisoners = officerPrisoners;

                    officers.Add(officer);
                    sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                }
            }

            context.Officers.AddRange(officers);
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