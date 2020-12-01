using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.gamecodeplus.arkmetricsexporter.steamquery
{
    public static class BinaryReaderExtensionMethods
    {
        public static string ReadStringNullTerminated(this BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            for(; ; )
            {
                var character = br.ReadChar();
                if (character == 0x00) return sb.ToString();
                else sb.Append(character);
            }
        }
    }
}
