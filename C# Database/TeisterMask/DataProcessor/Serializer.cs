namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            throw new NotImplementedException();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(s => s.EmployeesTasks.Select(a => a.Task).All(g => g.OpenDate >= date))
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks.Select(w => new
                    {
                        TaskName = w.Task.Name,
                        OpenDate = w.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = w.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = w.Task.LabelType.ToString(),
                        ExecutionType = w.Task.ExecutionType.ToString()
                    })
                    .OrderByDescending(d=>d.DueDate)
                    .ThenBy(n=>n.TaskName)
                    .ToArray()
                })
                .OrderByDescending(k=>k.Tasks.Length)
                .ThenBy(u=>u.Username)
                .Take(10)
                .ToArray();

            var json = JsonConvert.SerializeObject(employees);

            return json;
        }
    }
}