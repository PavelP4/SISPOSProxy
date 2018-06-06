using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using SISPOSProxy.Core.Helpers;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SISPOSProxy.Core.Config
{
    class Settings
    {
        #region udp settings

        public static IList<IPEndPoint> ListenIpEndPoints;
        public static IList<IPEndPoint> TransmitIpEndPoints;

        #endregion udp settings

        #region db udf namedpipe settings

        public static string Udf2ProxyNamedPipeName;

        #endregion


        public static async Task Init()
        {
            var localIpAddress = NetHelper.GetLocalIPv4(NetworkInterfaceType.Ethernet);

            ListenIpEndPoints = await GetListenIpEndPointsAsync(localIpAddress);
            TransmitIpEndPoints = await GetTransmitIpEndPointsAsync();

            await InitFromProxySettingsAsync();
        }

        public static async Task<IList<IPEndPoint>> GetListenIpEndPointsAsync(IPAddress ipaddress)
        {
            var result = new List<IPEndPoint>();

            using (var conn = DbConnection.NewInstance())
            {
                var sql = "SELECT port FROM ilasst";
                var cmd = new MySqlCommand(sql, conn);
                
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        if (int.TryParse(reader.GetString(0), out int port))
                        {
                            result.Add(new IPEndPoint(ipaddress, port));
                        }
                    }
                }

                await conn.CloseAsync();
            }

            return result;
        }

        public static async Task<IList<IPEndPoint>> GetTransmitIpEndPointsAsync()
        {
            var result = new List<IPEndPoint>();

            using (var conn = DbConnection.NewInstance())
            {
                var sql = "SELECT address, port FROM real_ilasst";
                var cmd = new MySqlCommand(sql, conn);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        if (IPAddress.TryParse(reader.GetString(0), out IPAddress address)
                            && int.TryParse(reader.GetString(1), out int port))
                        {
                            result.Add(new IPEndPoint(address, port));
                        }
                    }
                }

                await conn.CloseAsync();
            }

            return result;
        }

        public static async Task InitFromProxySettingsAsync()
        {
            using (var conn = DbConnection.NewInstance())
            {
                var sql = "SELECT name, value FROM proxy_settings";
                var cmd = new MySqlCommand(sql, conn);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var value = reader.GetString(1);

                        switch (name)
                        {
                            case "udf2proxy_namedpipe_name": Udf2ProxyNamedPipeName = value;
                                break;
                        }
                    }
                }

                await conn.CloseAsync();
            }
        }
    }
}
