using System;
using System.IO;

namespace ARCExtractor
{
    public static class IOUtility
    {
        public static string ReadString(BinaryReader reader, int length)
        {
            string str = "";
            bool reachedNullTerminator = false;

            for (int i = 0; i < length; i++)
            {
                byte b = reader.ReadByte();

                if (!reachedNullTerminator)
                {
                    if (b == 0)
                    {
                        reachedNullTerminator = true;
                    }
                    else
                    {
                        str += (char)b;
                    }
                }
            }

            return str;
        }

        public static string ReadTString(BinaryReader reader)
        {
            string str = "";
            bool reachedNullTerminator = false;

            while (!reachedNullTerminator)
            {
                byte b = reader.ReadByte();

                if (b == 0)
                {
                    reachedNullTerminator = true;
                }
                else
                {
                    str += (char)b;
                }
            }

            return str;
        }
    }
}
