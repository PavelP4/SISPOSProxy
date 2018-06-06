using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SISPOSProxy.Core.Helpers
{
    class DbConnection
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["posdb"].ConnectionString;

        public static MySqlConnection NewInstance()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
