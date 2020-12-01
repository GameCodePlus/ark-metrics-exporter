using Prometheus;

namespace com.gamecodeplus.arkmetricsexporter.models
{
    static class PrometheusMgr
    {
        private static MetricServer metricServer = null;

        public static readonly Gauge IsUpGague =
            Metrics.CreateGauge("is_server_running", "Returns 1 if the server is currently running, 0 if it is not",
                new GaugeConfiguration
                {
                    LabelNames = new[] { "ip_address", "port", "ip_and_port", "server_name" }
                });

        public static readonly Gauge PlayerCountGague =
            Metrics.CreateGauge("player_count", "Returns the player count of the server",
                new GaugeConfiguration
                {
                    LabelNames = new[] { "ip_address", "port", "ip_and_port", "server_name" }
                });

        public static readonly Gauge PlayerOnlineGague =
            Metrics.CreateGauge("player_online", "Returns the player online status of each user the server",
                new GaugeConfiguration
                {
                    LabelNames = new[] { "ip_address", "port", "ip_and_port", "server_name", "player_name" }
                });

        public static void BeginHosting(ushort port)
        {
            metricServer = new MetricServer(port: port);
            metricServer.Start();
        }
    }
}
