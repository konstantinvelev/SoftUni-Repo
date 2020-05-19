namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.IO;
    using TeisterMask.Data.Models;
    using System.Text;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xml = new XmlSerializer(typeof(ImportProjectsDto[]), new XmlRootAttribute("Projects"));

            var sb = new StringBuilder();

            var projets = new List<Project>();

            ImportProjectsDto[] importProjectsDtos;

            using (var reader = new StringReader(xmlString))
            {
                importProjectsDtos = (ImportProjectsDto[])xml.Deserialize(reader);
            }
            foreach (var dto in importProjectsDtos)
            {
                if (IsValid(dto))
                {
                    var tasks = new List<Task>();

                    foreach (var task in dto.Tasks)
                    {
                        var execType = Enum.Parse<ExecutionType>(task.ExecutionType);
                        var labelType = Enum.Parse<LabelType>(task.LabelType);

                        if (execType == null || labelType == null)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (IsValid(task))
                        {

                            var currTask = new Task
                            {
                                Name = task.Name,
                                DueDate = DateTime.ParseExact(task.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                OpenDate = DateTime.ParseExact(task.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                ExecutionType = Enum.Parse<ExecutionType>(task.ExecutionType),
                                LabelType = Enum.Parse<LabelType>(task.LabelType),
                            };


                            if (string.IsNullOrWhiteSpace(dto.DueDate))
                            {
                                var boool = DateTime.ParseExact(dto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                               < DateTime.ParseExact(task.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                if (!boool)
                                {
                                    context.Tasks.Add(currTask);
                                    sb.AppendLine(ErrorMessage);
                                }
                                else
                                {
                                    tasks.Add(currTask);
                                }
                            }
                            else if (DateTime.ParseExact(dto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                     < DateTime.ParseExact(task.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                     && DateTime.ParseExact(dto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                     > DateTime.ParseExact(task.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                            {
                                tasks.Add(currTask);
                            }
                            else
                            {
                                //context.Tasks.Add(currTask);
                                sb.AppendLine(ErrorMessage);
                            }
                        }
                        else
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }

                    var project = new Project
                    {

                        Name = dto.Name,
                        DueDate = SetDate(dto.DueDate),
                        OpenDate = DateTime.ParseExact(dto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Tasks = tasks
                    };

                    sb.AppendLine($"Successfully imported project - {project.Name} with {project.Tasks.Count} tasks.");
                    projets.Add(project);
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
            }

            return sb.ToString().TrimEnd();
        }

        private static DateTime SetDate(string dueDate)
        {
            if (string.IsNullOrWhiteSpace(dueDate))
            {
                return new DateTime();
            }
            return DateTime.ParseExact(dueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }


        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            var employees = new List<Employee>();

            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                var list = new List<EmployeeTask>();

                if (IsValid(dto))
                {
                    var employe = new Employee
                    {
                        Username = dto.UserName,
                        Email = dto.Email,
                        Phone = dto.Phone,
                    };

                    foreach (var item in dto.Tasks)
                    {
                        var tasks = new List<Task>();

                        var task = context.Tasks.FirstOrDefault(s => s.Id == item);

                        if (task == null)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (!tasks.Contains(task))
                        {
                            tasks.Add(task);

                            var emplyeTask = new EmployeeTask
                            {
                                Employee = employe,
                                Task = task
                            };
                            list.Add(emplyeTask);
                        }

                    }

                    employe.EmployeesTasks = list;

                    employees.Add(employe);
                    sb.AppendLine($"Successfully imported employee - {employe.Username} with {employe.EmployeesTasks.Count} tasks.");

                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
            }
            //context.Employees.AddRange(employees);
            //context.SaveChanges();
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