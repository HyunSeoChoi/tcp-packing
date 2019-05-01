using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    class OutputPacket
    {
        private byte[] buffer;
        private int len;

        public OutputPacket(int messageSize, int protocol) // init size
        {
            buffer = new byte[messageSize + 8];
            len = 8; // protocol(int) + messegeSize(int) = (int) + (int)
            setProtocol(protocol);
        }

        private void expandBuffer() // buffer resize
        {
            byte[] newbuf = new byte[buffer.Length * 2 + 1];
            Buffer.BlockCopy(buffer, 0, newbuf, 0, buffer.Length);
            buffer = newbuf;
        }

        public int getLength()
        {
            return len;
        }

        public void setProtocol(int protocol) // write buffer <- protocol
        {
            Buffer.BlockCopy(BitConverter.GetBytes(protocol), 0, buffer, 4, 4);
        }

        public void writeInt32(int data) // write buffer <- (int)data
        {
            if (len + 4 > buffer.Length)
            {
                expandBuffer();
                writeInt32(data);
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(data), 0, buffer, len, 4);
                len += 4;
            }
        }

        public void writeUser(User u)
        {
            writeInt32(u.getKey());
            writeBytes(u.getUserName());
            writeFloat(u.speed);
            writeVector2(u.posVec);
            writeVector2(u.destinationVec);
        }

        public void writeBytes(byte[] data) // write buffer <- (byte[])data
        {
            if (len + data.Length + 4 > buffer.Length)
            {
                expandBuffer();
                writeBytes(data);
            }
            else
            {
                writeInt32(data.Length);
                Buffer.BlockCopy(data, 0, buffer, len, data.Length);
                len += data.Length;
            }
        }
        public void writeFloat(float data)
        {
            if (len + 4 > buffer.Length)
            {
                expandBuffer();
                writeFloat(data);
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(data), 0, buffer, len, 4);
                len += 4;
            }
        }
        public void writeVector2(Vector2 vec)
        {
            writeFloat(vec.x);
            writeFloat(vec.y);
        }
        public void writeString(string data) // write buffer <- (string )data
        {
            byte[] encodedData = Encoding.UTF8.GetBytes(data);
            writeBytes(encodedData);
        }

        public byte[] wrapPacket() // write buffer <- (int)len 
        {
            Buffer.BlockCopy(BitConverter.GetBytes(len), 0, buffer, 0, 4);
            return buffer;
        }
    }
}