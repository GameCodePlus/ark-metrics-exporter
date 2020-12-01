using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace com.gamecodeplus.arkmetricsexporter.steamquery
{
    sealed class SteamServerQuery
    {
        sealed public class ServerQuerySettings
        {
            public const int DEFAULT_TIMEOUT_MSEC = 2000;

            public int SendTimeout = DEFAULT_TIMEOUT_MSEC;
            public int ReceiveTimeout = DEFAULT_TIMEOUT_MSEC;
        }

        public IPEndPoint ServerEndpoint { get; private set; }

        public SteamServerQuery(IPEndPoint serverEndpoint)
        {
            ServerEndpoint = serverEndpoint;
        }

        /// <summary>
        /// Connects to the Ark: Survival Evolved UDP Query port and requests the players using 
        /// the steam server query protocol.
        /// <see cref="https://developer.valvesoftware.com/wiki/Server_queries#A2S_PLAYER"/>
        /// </summary>
        /// <param name="settings">Connection settings used for communication with the server.</param>
        /// <returns>Results of the server query communication with the server.</returns>
        /// <exception cref="SocketException">An error occured when accessing the socket.</exception>
        public GetServerPlayersResult GetPlayers(ServerQuerySettings settings)
        {
            if (settings == null) settings = new ServerQuerySettings();

            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndpoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;
                client.Connect(ServerEndpoint);

                // Challenge Request/Response
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x55 });
                requestPacket.AddRange(BitConverter.GetBytes(-1));
                client.Send(requestPacket.ToArray(), requestPacket.Count);
                byte[] responseData = client.Receive(ref localEndpoint);

                // Get Players Request/Response
                requestPacket.Clear();
                requestPacket.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x55 });
                requestPacket.AddRange(responseData.Skip(5).Take(4));
                client.Send(requestPacket.ToArray(), requestPacket.Count);
                responseData = client.Receive(ref localEndpoint);
                return GetServerPlayersResult.Parse(responseData);
            }
        }

        /// <summary>
        /// Connects to the Ark: Survival Evolved UDP Query port and requests the server rules using 
        /// the steam server query protocol.
        /// <see cref="https://developer.valvesoftware.com/wiki/Server_queries#A2S_RULES"/>
        /// </summary>
        /// <param name="settings">Connection settings used for communication with the server.</param>
        /// <returns>Results of the server query communication with the server.</returns>
        /// <exception cref="SocketException">An error occured when accessing the socket.</exception>
        public GetServerRulesResult GetRules(ServerQuerySettings settings)
        {
            if (settings == null) settings = new ServerQuerySettings();

            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndpoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;
                client.Connect(ServerEndpoint);

                // Challenge Request/Response
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x56 });
                requestPacket.AddRange(BitConverter.GetBytes(-1));
                client.Send(requestPacket.ToArray(), requestPacket.Count);
                byte[] responseData = client.Receive(ref localEndpoint);

                // Get Rules Request/Response
                requestPacket.Clear();
                requestPacket.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x56 });
                requestPacket.AddRange(responseData.Skip(5).Take(4));
                client.Send(requestPacket.ToArray(), requestPacket.Count);
                responseData = client.Receive(ref localEndpoint);
                return GetServerRulesResult.Parse(responseData);
            }
        }

        /// <summary>
        /// Connects to the Ark: Survival Evolved UDP Query port and requests the server info using 
        /// the steam server query protocol.
        /// <see cref="https://developer.valvesoftware.com/wiki/Server_queries#A2S_INFO"/>
        /// </summary>
        /// <param name="settings">Connection settings used for communication with the server.</param>
        /// <returns>Results of the server query communication with the server.</returns>
        /// <exception cref="SocketException">An error occured when accessing the socket.</exception>
        public GetServerInfoResult GetInfo(ServerQuerySettings settings)
        {
            if (settings == null) settings = new ServerQuerySettings();

            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndpoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;
                client.Connect(ServerEndpoint);

                // Challenge Request/Response
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x54 });
                requestPacket.AddRange(Encoding.ASCII.GetBytes("Source Engine Query"));
                requestPacket.Add(0x00);
                client.Send(requestPacket.ToArray(), requestPacket.Count);
                byte[] responseData = client.Receive(ref localEndpoint);
                return GetServerInfoResult.Parse(responseData);
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                SteamServerQuery q = (SteamServerQuery)obj;
                if ((q.ServerEndpoint == null && ServerEndpoint != null) ||
                    (q.ServerEndpoint != null && ServerEndpoint == null))
                    return false;
                else if (q.ServerEndpoint == null && ServerEndpoint == null) return true;
                else return (q.ServerEndpoint.Equals(ServerEndpoint));
            }
        }

        public override int GetHashCode()
        {
            return ServerEndpoint == null ? 0 : ServerEndpoint.GetHashCode();
        }

        public override string ToString()
        {
            return $"SteamServerQuery Connection to {ServerEndpoint}";
        }
    }
}
