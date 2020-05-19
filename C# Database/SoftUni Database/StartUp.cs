using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var conntext = new SoftUniContext();
            Console.WriteLine(GetAddressesByTown(conntext));
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var newAdress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(newAdress);

            var nakov = context
                .Employees
                .First(e => e.LastName == "Nakov");

            nakov.Address = newAdress;

            context.SaveChanges();

            var addressText = context
                .Employees
                .OrderByDescending(E => E.AddressId)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .Take(10)
                .ToList();

            foreach (var text in addressText)
            {
                sb.AppendLine($"{text.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                 .Select(e => new
                 {
                     FirstName = e.FirstName,
                     LastName = e.LastName,
                     ManagerFirstName = e.Manager.FirstName,
                     ManagerLastName = e.Manager.LastName,
                     Projects = e.EmployeesProjects.Select(ep => new
                     {
                         ProjectName = ep.Project.Name,
                         ProjectStartDate = ep.Project.StartDate,
                         ProjectEndDate = ep.Project.EndDate
                     })
                 }).Take(10);

            StringBuilder employeeManagerResult = new StringBuilder();

            foreach (var employee in employees)
            {
                employeeManagerResult.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    var startDate = project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt");
                    var endDate = project.ProjectEndDate.HasValue ? project.ProjectEndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";

                    employeeManagerResult.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }
            return employeeManagerResult.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .OrderByDescending(e => e.Employees.Count)
                .ThenBy(e => e.Town.Name)
                .Take(10)
                .Select(a => new
                {
                    TownName = a.Town.Name,
                    a.AddressText,
                    EmployeeCount = a.Employees.Count
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in addresses)
            {
                sb.AppendLine($"{item.AddressText}, {item.TownName} - {item.EmployeeCount} employees");
            }
            return sb.ToString().TrimEnd();
        }
    }
}

