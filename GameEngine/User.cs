using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace GameEngine
{
    class User
    {
        private static int _nextkey = 1;
        private int key;
        private Socket usrSocket;
        private byte[] name;
        private byte[] receiveBuffer;
        private int end;
        private int cursor;
        private bool connected;
        public float speed;
        public Vector2 destinationVec;
        public Vector2 posVec;
        private ConcurrentQueue<InputPacket> inputPacketQ;
        public int getKey()
        {
            return key;
        }
        public byte[] getUserName()
        {
            return name.Clone() as byte[];
        }
        public void setUserName(byte[] newName)
        {
            name = newName.Clone() as byte[];
        }
        public User(Socket sock)
        {
            speed = 10.0f;
            posVec = new Vector2(0, 0);
            destinationVec = new Vector2(0, 0);
            usrSocket = sock;
            key = _nextkey++;
            receiveBuffer = new byte[Global.MAX_INPUT_PACKET_LEN * 2];
            cursor = 0;
            end = 0;
            connected = true;
            inputPacketQ = new ConcurrentQueue<InputPacket>();
            usrSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, receivePacketComplete, null);
        }

        public bool isClosed()
        {
            return connected == false;
        }

        public void sendPacketAsync(OutputPacket op)
        {
            if (op.getLength() > Global.MAX_OUTPUT_PACKET_LEN)
            {
                Console.WriteLine("sendPacket length is greater than MAX_PACKET_LEN");
                return;
            }
            try
            {
                usrSocket.BeginSend(op.wrapPacket(), 0, op.getLength(), SocketFlags.None, sendPacketComplete, null);
            }
            catch (Exception e)
            {
                close();
            }
        }

        private void sendPacketComplete(IAsyncResult ar)
        {
            try
            {
                usrSocket.EndSend(ar);
            }
            catch (Exception e)
            {
                close();
            }
        }

        private void receivePacketComplete(IAsyncResult ar)
        {
            try
            {
                int len = usrSocket.EndReceive(ar);
                if (len == 0)
                {
                    close();
                    return;
                }
                else
                {
                    end += len;
                }
            }
            catch(Exception e)
            {
                close();
                return;
            }

            while (true)
            {
                if (4 > end - cursor)
                    break;
                int packetLen = BitConverter.ToInt32(receiveBuffer, cursor);
                if (packetLen < 0 || packetLen > Global.MAX_INPUT_PACKET_LEN)
                {
                    close();
                    return;
                }
                if (end - cursor < packetLen)
                    break;

                InputPacket ip = new InputPacket(receiveBuffer, cursor, packetLen);
                inputPacketQ.Enqueue(ip);
                cursor += packetLen;
            }
            if (cursor >= Global.MAX_INPUT_PACKET_LEN)
            {
                Buffer.BlockCopy(receiveBuffer, cursor, receiveBuffer, 0, end - cursor);
            }
            try
            {
                usrSocket.BeginReceive(receiveBuffer, end, receiveBuffer.Length - end, SocketFlags.None, receivePacketComplete, null);
            }
            catch (Exception e)
            {
                close();
            }
        }

        public InputPacket readPacket()
        {
            if (inputPacketQ.Count == 0)
                return null;
            InputPacket ip = null;
            inputPacketQ.TryDequeue(out ip);

            return ip;
        }
        public void close()
        {
            connected = false;
            usrSocket.Close();
        }

        public void Update(float deltaTime)
        {
            // Console.Write(posVec);
            float moveDistance = speed * deltaTime;
            Vector2 dirVec = destinationVec - posVec;
            if(dirVec.GetLength() < moveDistance)
            {
                posVec = destinationVec;
            }
            else
            {
                dirVec.Normalize();
                posVec += dirVec * moveDistance;
            }

            //Console.WriteLine(posVec);
        }
    }
}
