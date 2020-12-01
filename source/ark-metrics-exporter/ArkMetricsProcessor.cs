using com.gamecodeplus.arkmetricsexporter.models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.gamecodeplus.arkmetricsexporter
{
    class ArkMetricsProcessor
    {
        List<ArkServer> servers = new List<ArkServer>();

        internal void Run()
        {
            ArkAppConfig appConfig = null;
            try
            {
                appConfig = ArkAppConfig.Load("config.yaml");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to load config file config.yaml: {ex}");
                return;
            }

            try
            {
                PrometheusMgr.BeginHosting((ushort)appConfig.PrometheusPort);
            }
            catch(Exception ex)
            {
                Console.Write($"Failed to start prometheus host on port {appConfig.PrometheusPort}: {ex}");
                return;
            }

            foreach (var configItem in appConfig.Servers)
            {
                servers.Add(new ArkServer(configItem));
            }

            for(; ;)
            {
                foreach (var server in servers)
                {
                    if (server.IsExecutionReady)
                    {
                        ThreadPool.QueueUserWorkItem((a) => { server.Execute(); });                        
                    }
                }
            }
        }
    }
}
