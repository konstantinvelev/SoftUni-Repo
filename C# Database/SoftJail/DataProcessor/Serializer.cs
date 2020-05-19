namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(a => ids.Contains(a.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(p => new
                    {
                        OfficerName = p.Officer.FullName,
                        Department = p.Officer.Department.Name
                    })
                    .OrderBy(a => a.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = x.PrisonerOfficers.Sum(p => p.Officer.Salary).ToString("F2")
                })
                .OrderBy(a => a.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var json = JsonConvert.SerializeObject(prisoners, Formatting.Indented);

            return json;

        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var xml = new XmlSerializer(typeof(ExportPrisonerDto[]), new XmlRootAttribute("Prisoners"));

            string[] names = prisonersNames.Split(",");

            var prisoners = context.Prisoners
                .Where(p => names.Contains(p.FullName))
                .Select(e => new ExportPrisonerDto
                {
                    Id = e.Id,
                    Name = e.FullName,
                    IncarcerationDate = e.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = e.Mails.Select(m => new EncryptedMessagesDto
                    {
                        Description = ReverseDescription(m.Description)
                    })
                    .ToArray()
                })
                .OrderBy(s=>s.Name)
                .ThenBy(i=>i.Id)
                .ToArray();

            var sb = new StringBuilder();

            var nm = new XmlSerializerNamespaces();
            nm.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xml.Serialize(writer, prisoners, nm);
            }

            return sb.ToString().TrimEnd();
        }

        private static string ReverseDescription(string description)
        {
            var list = description.ToList();
             list.Reverse();

            return string.Join("",list);
        }
    }
}