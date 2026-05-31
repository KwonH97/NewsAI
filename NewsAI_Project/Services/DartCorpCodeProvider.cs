using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NewsAI_Project.Services
{
    public class DartCorpCodeProvider
    {
        private readonly string _xmlPath;

        public DartCorpCodeProvider(string xmlPath)
        {
            _xmlPath = xmlPath;
        }

        public string? FindCorpCode(string companyName)
        {
            if (!File.Exists(_xmlPath))
                return null;

            XDocument doc = XDocument.Load(_xmlPath);

            var company =
                doc.Descendants("list")
                   .FirstOrDefault(x =>
                       (string?)x.Element("corp_name")
                       == companyName);

            return company?.Element("corp_code")?.Value;
        }
    }
}