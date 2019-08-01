// 간단 2D 탱크 게임 (서버쪽) 예시

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameEngine
{
    class Program
    {
        static List<User> usrList = new List<User>();
        static List<User> tempList = new List<User>();
        static long lastUpdateTime; // Time.DeletaTime;
        static void Main(string[] args)
        {
            lastUpdateTime = DateTime.Now.Ticks;
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint( IPAddress.Any, 25565));
            listenSocket.Listen(10);

            Thread th = new Thread(ServerProcess);
            th.Start();

            while (true)
            {
                Socket usr = listenSocket.Accept();
                Console.WriteLine("CONNECT");
                lock (tempList)
                {
                    tempList.Add(new User(usr));
                }
            }
        }

        static void ServerProcess()
        {
            while (true)
            {
                SweepTmpList();
                ReadPackets();
                Update();
                DetectCollision();
            }
        }

        static void SweepTmpList()
        {
            lock (tempList)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    User u = tempList[i];
                    InputPacket ip = u.readPacket();
                    if (ip != null)
                    {
                        if (ip.getProtocol() == Protocol.REQUEST_ENTER)
                        {
                            handleRequestEnter(ip, u);
                        }
                        else
                        {
                            u.close();
                        }
                        tempList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        static void handleRequestEnter(InputPacket ip,User u)
        {
            Console.WriteLine("handlerE");
            OutputPacket op1 = new OutputPacket(512, Protocol.RESPONSE_ENTER);
            u.setUserName(ip.readBytes());
            op1.writeUser(u);

            op1.writeInt32(usrList.Count);
            
            foreach(User otherUsr in usrList)
            {
                op1.writeUser(otherUsr);
            }
            u.sendPacketAsync(op1);

            OutputPacket op2 = new OutputPacket(512, Protocol.BROADCAST_ENTER);

            op2.writeUser(u);
            foreach(User otherUsr in usrList)
            {
                otherUsr.sendPacketAsync(op2);
            }

            usrList.Add(u);
            //Console.WriteLine("HEOS");
        }
        
        static void ReadPackets()
        {
            for(int i=0;i<usrList.Count;i++)
            {
                User u = usrList[i];
                InputPacket ip = u.readPacket();
                while(ip != null)
                {
                    switch(ip.getProtocol())
                    {
                        case Protocol.REQUEST_CHANGE_DESTINATION:
                            handleRequestChangeDestination(ip, u);
                            break;
                    }
                    ip = u.readPacket();
                }
            }
        }

        static void Update()
        {
            long currentTime = DateTime.Now.Ticks;
            float deltaTime = (currentTime - lastUpdateTime) / 10000000.0f;

            //Console.WriteLine(deltaTime);
            foreach (User u in usrList)
            {
                u.Update(deltaTime);
            }

            lastUpdateTime = currentTime;
        }

        static void DetectCollision()
        {

        }

        static void handleRequestChangeDestination(InputPacket ip , User u)
        {
            OutputPacket op = new OutputPacket(512, Protocol.BROADCAST_CHANGE_DESTINATION);
            int key = u.getKey();
            op.writeInt32(key);
            op.writeVector2(u.posVec);
            Vector2 newDestinaton = ip.readVector2();
            op.writeVector2(newDestinaton);
            u.destinationVec = newDestinaton;            

            //Console.WriteLine(key + u.posVec.x + ", " + u.posVec.y + " " + newDestinaton.x + "," + newDestinaton.y);
            foreach(User otherUsr in usrList)
            {
                otherUsr.sendPacketAsync(op);
            }
        }
    }
}
