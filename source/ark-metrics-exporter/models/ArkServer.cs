using com.gamecodeplus.arkmetricsexporter.steamquery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;

namespace com.gamecodeplus.arkmetricsexporter.models
{
    class ArkServer
    {
        Timer readyTimer;

        private ArkAppConfig.ArkAppServerConfig configItem;
        private SteamServerQuery serverQuery;

        private string serverName = null;

        private HashSet<string> knownPlayers = new HashSet<string>();

        public ArkServer(ArkAppConfig.ArkAppServerConfig configItem)
        {
            IsExecutionReady = false;
            if (configItem.QueryTimeSeconds < 1)
            {
                configItem.QueryTimeSeconds = 30;
            }

            serverQuery = new SteamServerQuery(new System.Net.IPEndPoint(IPAddress.Parse(configItem.IPAddress), configItem.Port));

            this.configItem = configItem;
            readyTimer = new Timer((double)configItem.QueryTimeSeconds * 1000);
            readyTimer.AutoReset = true;
            readyTimer.Elapsed += ReadyTimer_Elapsed;
            readyTimer.Start();
        }

        private void ReadyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IsExecutionReady = true;
        }

        public bool IsExecutionReady
        {
            get { bool r = isExecutionReady; if (r) isExecutionReady = false; return r; }
            private set { isExecutionReady = value; }
        }
        private bool isExecutionReady;

        private bool ServerNameChanged { get; set; }

        internal void Execute()
        {
            try
            {
                ProcessServerInfo();
                ProcessPlayerInfo();
            }
            catch (Exception) { }
        }

        private void ProcessPlayerInfo()
        {
            try
            {
                var playerInfo = serverQuery.GetPlayers(new SteamServerQuery.ServerQuerySettings() { ReceiveTimeout = 30000, SendTimeout = 30000 });

                if (ServerNameChanged)
                {
                    var curLabels = PrometheusMgr.PlayerOnlineGague.GetAllLabelValues();
                    if (curLabels != null)
                    {
                        foreach (var item in curLabels)
                        {
                            PrometheusMgr.PlayerOnlineGague.RemoveLabelled(item);
                        }
                    }

                    curLabels = PrometheusMgr.PlayerCountGague.GetAllLabelValues();
                    if (curLabels != null)
                    {
                        foreach (var item in curLabels)
                        {
                            PrometheusMgr.PlayerCountGague.RemoveLabelled(item);
                        }
                    }
                }

                PrometheusMgr.PlayerCountGague.WithLabels(new string[] { configItem.IPAddress, configItem.Port.ToString(), $"{configItem.IPAddress}:{configItem.Port}", serverName }).Set(playerInfo.Players.Count);

                foreach (var player in playerInfo.Players)
                {
                    if (!String.IsNullOrWhiteSpace(player.PlayerName))
                    {
                        PrometheusMgr.PlayerOnlineGague.WithLabels(new string[]
                        {
                            configItem.IPAddress,
                            configItem.Port.ToString(),
                            $"{configItem.IPAddress}:{configItem.Port}",
                            serverName,
                            player.PlayerName
                        }).Set(1);
                        knownPlayers.Add(player.PlayerName);
                    }
                }

                foreach (var existingPlayer in knownPlayers)
                {
                    if (!playerInfo.Players.Any((x) => x.PlayerName.Equals(existingPlayer)))
                    {
                        PrometheusMgr.PlayerOnlineGague.WithLabels(new string[]
                        {
                            configItem.IPAddress,
                            configItem.Port.ToString(),
                            $"{configItem.IPAddress}:{configItem.Port}",
                            serverName,
                            existingPlayer
                        }).Set(0);
                    }
                }
            }
            catch (Exception) 
            {
                PrometheusMgr.PlayerCountGague.WithLabels(new string[] { configItem.IPAddress, configItem.Port.ToString(), $"{configItem.IPAddress}:{configItem.Port}", serverName }).Set(0);
            }
        }

        private void ProcessServerInfo()
        {
            try
            {
                var serverInfo = serverQuery.GetInfo(new SteamServerQuery.ServerQuerySettings() { ReceiveTimeout = 30000, SendTimeout = 30000 });

                // Remove the "Old" server name if the same server just changed names (since version number
                // is in the server name, this is common on updates).
                ServerNameChanged = false;
                if (serverName == null || (serverName != null && serverName != serverInfo.Name))
                {
                    ServerNameChanged = true;
                    var curLabels = PrometheusMgr.IsUpGague.GetAllLabelValues();
                    if (curLabels != null)
                    {
                        foreach (var item in curLabels)
                        {
                            PrometheusMgr.IsUpGague.RemoveLabelled(item);
                        }
                    }

                    //curLabels = PrometheusMgr.PlayerCountGague.GetAllLabelValues();
                    //if (curLabels != null)
                    //{
                    //    foreach (var item in curLabels)
                    //    {
                    //        PrometheusMgr.PlayerCountGague.RemoveLabelled(item);
                    //    }
                    //}
                }
                serverName = serverInfo.Name;

                // Update the "IsUp" gague setting to "true"
                PrometheusMgr.IsUpGague.WithLabels(new string[] { configItem.IPAddress, configItem.Port.ToString(), $"{configItem.IPAddress}:{configItem.Port}", serverName }).Set(1);
                //PrometheusMgr.PlayerCountGague.WithLabels(new string[] { configItem.IPAddress, configItem.Port.ToString(), $"{configItem.IPAddress}:{configItem.Port}", serverName }).Set(serverInfo.Players);
            }
            catch (Exception)
            {
                var curName = serverName ?? String.Empty;

                // Update the "IsUp" gague setting to "false"
                PrometheusMgr.IsUpGague.WithLabels(new string[] { configItem.IPAddress, configItem.Port.ToString(), $"{configItem.IPAddress}:{configItem.Port}", curName }).Set(0);
                //PrometheusMgr.PlayerCountGague.WithLabels(new string[] { configItem.IPAddress, configItem.Port.ToString(), $"{configItem.IPAddress}:{configItem.Port}", serverName }).Set(0);
            }
        }
    }
}
