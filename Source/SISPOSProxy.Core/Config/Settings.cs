using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SISPOSProxy.Core.Helpers;
using MySql.Data.MySqlClient;

namespace SISPOSProxy.Core.Config
{
    public class Settings
    {
        #region udp settings

        public int? ListenPort { get; set; }
        public IList<IPEndPoint> TransmitIpEndPoints { get; set; }

        #endregion udp settings

        #region db udf namedpipe settings

        public string Udf2ProxyNamedPipeName { get; set; }
        public int Udf2ProxyNamedPipeMaxServerInstances { get; set; } = 4;

        #endregion


        public async Task InitAsync()
        {
            var localIpAddress = NetHelper.GetLocalIPv4();

            ListenPort = await GetListenPortAsync(localIpAddress);
            TransmitIpEndPoints = await GetTransmitIpEndPointsAsync();

            await InitFromProxySettingsAsync();
        }

        private async Task<int?> GetListenPortAsync(IPAddress ipaddress)
        {
            using (var conn = DbConnection.NewInstance())
            {
                var sql = "SELECT port FROM ilasst WHERE address = @address";
                var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@address", ipaddress.ToString());

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        if (int.TryParse(reader.GetString(0), out int port))
                        {
                            return port;
                        }
                    }
                }

                await conn.CloseAsync();
            }

            return null;
        }

        private async Task<IList<IPEndPoint>> GetTransmitIpEndPointsAsync()
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

        private async Task InitFromProxySettingsAsync()
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
                            case "udf2proxy_namedpipe_name":
                                Udf2ProxyNamedPipeName = value;
                                break;
                            case "udf2proxy_namedpipe_maxserverinstances":
                                Udf2ProxyNamedPipeMaxServerInstances = int.Parse(value);
                                break;
                        }
                    }
                }

                await conn.CloseAsync();
            }
        }
    }
}
