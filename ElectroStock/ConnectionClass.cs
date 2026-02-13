using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studium
{
    internal class ConnectionClass
    {
        public static string Conexion
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ConnectionStringMio"]?.ConnectionString;

            }
        }
    }
}
