using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Atom.Services
{
    public static class ConnectionService
    {
        public static string GetConnectionString()
        {
            string value = ConfigurationManager.AppSettings["appPath"];

            XDocument document = XDocument.Load(value ?? @"D:\SAM\Sources\SAM-NET40\SAM Web\Web.config");
            string result = document.XPathSelectElement("//connectionStrings/add[@name = 'SAM_CS']").Attribute("connectionString").Value;
            return result;
        }

        public static SqlConnection GetConnection()
        {
            string connection = GetConnectionString();
            return new SqlConnection(connection);
        }
    }
}
