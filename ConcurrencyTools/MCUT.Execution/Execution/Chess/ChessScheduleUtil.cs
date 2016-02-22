using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{
    public static class ChessScheduleUtil
    {
        //      <schedule format="hex">
        //        01000000 01000000 19000000 01000000 FF010000 01000000 01000000 FF010000 12000000 01000000 0200000
        //        ...
        //      </schedule>

        private const string hexchars = "0123456789ABCDEF";

        public static XElement CreateXScheduleFromFile(string filename)
        {
            string scheduleHex = ReadScheduleFromFile(filename);
            if (scheduleHex == null)
                return null;

            return new XElement(XChessNames.Schedule
                , new XAttribute(XChessNames.AFormat, XChessNames.VHex)
                , scheduleHex
                );
        }

        /// <summary>
        /// Reads a schedule file and returns its hex representation as a string.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ReadScheduleFromFile(string filename)
        {
            if (!File.Exists(filename))
                return null;

            StringBuilder b = new StringBuilder();
            using (FileStream stream = File.Open(filename, FileMode.Open))
            {
                uint pos = 0;
                int curbyteInt;
                while ((curbyteInt = stream.ReadByte()) != -1)
                {
                    byte curbyte = (byte)curbyteInt;

                    if (pos % 64 == 0)
                        b.Append("\n  ");
                    if (pos % 4 == 0)
                        b.Append(" ");

                    b.Append(hexchars[(curbyte >> 4) & 0xF]);
                    b.Append(hexchars[curbyte & 0xF]);

                    pos++;
                }
            }

            return b.Length == 0 ? null : b.ToString();
        }

        public static void SaveXScheduleToFile(XElement xschedule, string filePath)
        {
            if (xschedule == null)
                throw new Exception("Can not write schedule: none supplied");
            if (xschedule.Attribute("format").Value != "hex")
                throw new Exception("Unsupported xml schedule format");
            string encoding = (xschedule.FirstNode as XText).Value;

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                for (int pos = 0; pos < encoding.Length; )
                {
                    if (Char.IsWhiteSpace(encoding[pos]))
                    {
                        pos++;
                    }
                    else
                    {
                        uint upper = DecodeHexDigit(encoding[pos++]);
                        uint lower = DecodeHexDigit(encoding[pos++]);
                        byte combined = (byte)((upper << 4) | lower);
                        stream.WriteByte(combined);
                    }
                }
            }
        }

        private static uint DecodeHexDigit(char digit)
        {
            if (Char.IsDigit(digit))
                return ((uint)(digit - '0')) & 0xFu;
            else
                return ((uint)(10u + Char.ToUpper(digit) - 'A')) & 0xFu;
        }
    }
}
