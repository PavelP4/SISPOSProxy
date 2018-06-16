using MySql.Data.MySqlClient;
using System.Configuration;

namespace SISPOSProxy.Core.Helpers
{
    public class DbConnection
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["posdb"].ConnectionString;

        public static MySqlConnection NewInstance()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
