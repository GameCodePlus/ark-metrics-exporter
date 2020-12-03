using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace com.gamecodeplus.arkmetricsexporter.steamquery
{
    class GetServerPlayersResult
    {
        internal class PlayerInfo
        {
            public string PlayerName { get; set; }
            public uint Score { get; set; }
            public float Duration { get; set; }
        }

        List<PlayerInfo> m_playerInfo = new List<PlayerInfo>();

        internal IReadOnlyCollection<PlayerInfo> Players
        {
            get { return new ReadOnlyCollection<PlayerInfo>(m_playerInfo); }
        }

        internal static GetServerPlayersResult Parse(byte[] responseData)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(responseData)))
            {
                GetServerPlayersResult result = new GetServerPlayersResult();
                var leader = br.ReadBytes(5);
                if (!Enumerable.SequenceEqual(leader, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x44 })) return null;

                var playerCount = br.ReadByte();
                for (byte i = 0; i < playerCount; ++i)
                {
                    PlayerInfo p = new PlayerInfo();
                    br.ReadByte(); // Index??
                    p.PlayerName = br.ReadStringNullTerminated();
                    p.Score = br.ReadUInt32();
                    p.Duration = br.ReadSingle();
                    if (!String.IsNullOrWhiteSpace(p.PlayerName))
                        result.m_playerInfo.Add(p);
                }

                return result;
            }
        }
    }
}