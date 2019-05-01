using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    class InputPacket
    {
        private byte[] buffer;
        private int len;
        private int cursor;

        public InputPacket(byte[] buf, int offset, int count)
        {
            buffer = new byte[count];
            Buffer.BlockCopy(buf, offset, buffer, 0, count);
            len = BitConverter.ToInt32(buffer, 0);
            cursor = 8;
        }

        public int getProtocol()
        {
            return BitConverter.ToInt32(buffer, 4);
        }

        public int getLength()
        {
            return len;
        }

        public int readInt32() // && protocol != -1 read (int)data
        {
            int ret = -1;
            if (cursor + 4 <= len)
            {

                ret = BitConverter.ToInt32(buffer, cursor);
                cursor += 4;
            }
            return ret;
        }

        public byte[] readBytes() // read (byte)data
        {
            byte[] ret = null;
            int byteLen = readInt32();

            if (byteLen < 0 || byteLen > len - cursor)
            {
                return ret;
            }
            ret = new byte[byteLen];
            Buffer.BlockCopy(buffer, cursor, ret, 0, byteLen);
            cursor += byteLen;

            return ret;
        }

        public string readString() // read(string)data
        {
            string ret = null;
            int encodedLen = readInt32();
            if (encodedLen < 0 || encodedLen > len - cursor)
            {
                return ret;
            }

            ret = Encoding.UTF8.GetString(buffer, cursor, encodedLen);
            cursor += encodedLen;

            return ret;
        }
        public Vector2 readVector2()
        {
            return new Vector2(readFloat(), readFloat());
        }
        public float readFloat()
        {
            float ret = 0.0f;
            if (cursor + 4 <= len)
            {
                ret = BitConverter.ToSingle(buffer, cursor);
                cursor += 4;
            }
            return ret;
        }
    }
}
