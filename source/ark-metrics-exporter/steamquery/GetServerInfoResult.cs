using System;
using System.IO;
using System.Linq;

namespace com.gamecodeplus.arkmetricsexporter.steamquery
{
    class GetServerInfoResult
    {
        public enum ServerTypes
        {
            DedicatedServer,
            NonDedicatedServer,
            SourceTVRelay,
            Unknown
        }

        public enum EnvironmentTypes
        {
            Linux,
            Windows,
            Mac,
            Unknown
        }

        public enum ServerVisibilities
        {
            Public,
            Private
        }

        public enum VACModes
        {
            Unsecured,
            Secured
        }


        public byte ProtocolVersion { get; private set; }
        public string Name { get; private set; }
        public string Map { get; private set; }
        public string Folder { get; private set; }
        public string Game { get; private set; }
        public short ID { get; private set; }
        public byte Players { get; private set; }
        public byte MaxPlayers { get; private set; }
        public byte Bots { get; private set; }
        public ServerTypes ServerType { get; private set; }
        public EnvironmentTypes Environment { get; private set; }
        public ServerVisibilities Visibility { get; private set; }
        public VACModes VAC { get; private set; }
        public string Version { get; private set; }
        public ushort? Port { get; set; }
        public ulong? SteamID { get; private set; }
        public ushort? SpectatorPort { get; private set; }
        public string SpectatorServer { get; private set; }
        public string Keywords { get; private set; }
        public ulong? GameID { get; private set; }

        internal static GetServerInfoResult Parse(byte[] responseData)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(responseData)))
            {
                GetServerInfoResult result = new GetServerInfoResult();
                var leader = br.ReadBytes(5);
                if (!Enumerable.SequenceEqual(leader, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x49 })) return null;

                result.ProtocolVersion = br.ReadByte();
                result.Name = br.ReadStringNullTerminated();
                result.Map = br.ReadStringNullTerminated();
                result.Folder = br.ReadStringNullTerminated();
                result.Game = br.ReadStringNullTerminated();
                result.ID = br.ReadInt16();
                result.Players = br.ReadByte();
                result.MaxPlayers = br.ReadByte();
                result.Bots = br.ReadByte();
                switch(br.ReadChar())
                {
                    case 'd': result.ServerType = ServerTypes.DedicatedServer; break;
                    case 'l': result.ServerType = ServerTypes.NonDedicatedServer; break;
                    case 'p': result.ServerType = ServerTypes.SourceTVRelay; break;
                    default: result.ServerType = ServerTypes.Unknown; break;
                }
                switch(br.ReadChar())
                {
                    case 'l': result.Environment = EnvironmentTypes.Linux; break;
                    case 'w': result.Environment = EnvironmentTypes.Windows; break;
                    case 'm':
                    case 'o': result.Environment = EnvironmentTypes.Mac; break;
                    default: result.Environment = EnvironmentTypes.Unknown; break;
                }
                switch (br.ReadByte())
                {
                    case 0: result.Visibility = ServerVisibilities.Public; break;
                    default: result.Visibility = ServerVisibilities.Private; break;
                }
                switch (br.ReadByte())
                {
                    case 0: result.VAC = VACModes.Unsecured; break;
                    default: result.VAC = VACModes.Secured; break;
                }
                result.Version = br.ReadStringNullTerminated();
                var edf = br.ReadByte();
                if ((edf & 0x80) != 0)
                {
                    result.Port = br.ReadUInt16();
                }
                else result.Port = null;
                if ((edf & 0x10) != 0)
                {
                    result.SteamID = br.ReadUInt64();
                }
                else result.SteamID = null;
                if ((edf & 0x40) != 0)
                {
                    result.SpectatorPort = br.ReadUInt16();
                    result.SpectatorServer = br.ReadStringNullTerminated();
                }
                else 
                { 
                    result.SpectatorPort = null;
                    result.SpectatorServer = null;
                }
                if ((edf & 0x20) != 0)
                {
                    result.Keywords = br.ReadStringNullTerminated();
                }
                else
                {
                    result.Keywords = null;
                }
                if ((edf & 0x01) != 0)
                {
                    result.GameID = br.ReadUInt64();
                }
                else
                {
                    result.GameID = null;
                }

                return result;
            }
        }
    }
}