using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace com.gamecodeplus.arkmetricsexporter.models
{

    class ArkAppConfig
    {
        internal class ArkAppServerConfig
        {
            [YamlMember(Alias = "ip-address", ApplyNamingConventions = false)]
            public string IPAddress { get; set; }

            [YamlMember(Alias = "port", ApplyNamingConventions = false)]
            public int Port { get; set; }

            [YamlMember(Alias = "query-sec", ApplyNamingConventions = false, DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
            public int QueryTimeSeconds { get; set; }
        }

        [YamlMember(Alias = "prometheus-port", ApplyNamingConventions = false)]
        public int PrometheusPort { get; set; }

        [YamlMember(Alias = "servers", ApplyNamingConventions = false)]
        public List<ArkAppServerConfig> Servers { get; set; }

        internal static ArkAppConfig Load(string configPath)
        {
            var input = File.ReadAllText(configPath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<ArkAppConfig>(input);
        }
    }
}
