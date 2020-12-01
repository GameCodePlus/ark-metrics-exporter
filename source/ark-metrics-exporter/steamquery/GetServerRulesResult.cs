using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace com.gamecodeplus.arkmetricsexporter.steamquery
{
    class GetServerRulesResult
    {
        public IReadOnlyDictionary<string, string> Rules { get { return new ReadOnlyDictionary<string, string>(m_rules); } }

        Dictionary<string, string> m_rules = new Dictionary<string, string>();

        private void Add(string key, string val) { m_rules.Add(key, val); }

        internal static GetServerRulesResult Parse(byte[] responseData)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(responseData)))
            {
                GetServerRulesResult result = new GetServerRulesResult();
                var leader = br.ReadBytes(5);
                if (!Enumerable.SequenceEqual(leader, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x45 })) return null;

                var ruleCount = br.ReadUInt16();
                for (ushort i = 0; i < ruleCount; ++i)
                {
                    var key = br.ReadStringNullTerminated();
                    var val = br.ReadStringNullTerminated();
                    result.Add(key, val);
                }

                return result;
            }
        }
    }
}