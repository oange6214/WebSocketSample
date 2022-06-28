using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CA_Client
{
    internal class Program
    {
        //建立1個客戶端套接字和1個負責監聽服務端請求的執行緒 
        static Thread ThreadClient = null;
        static Socket SocketClient = null;

        static void Main(string[] args)
        {
            try
            {
                int port = 11000;
                string host = "127.0.0.1";//伺服器端ip地址
                //string host = "192.168.3.249";//伺服器端ip地址

                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);

                //定義一個套接字監聽 
                SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //客戶端套接字連線到網路節點上，用的是Connect 
                    SocketClient.Connect(ipe);
                }
                catch (Exception)
                {
                    Console.WriteLine("連線失敗！\r\n");
                    Console.ReadLine();
                    return;
                }

                //ThreadClient = new Thread(Recv);
                //ThreadClient.IsBackground = true;
                //ThreadClient.Start();

                Thread.Sleep(1000);
                Console.WriteLine("請輸入內容<按Enter鍵傳送>：\r\n");
                while (true)
                {
                    string sendStr = Console.ReadLine();
                    ClientSendMsg(sendStr);
                }

                //int i = 1;
                //while (true)
                //{
                //  Console.Write("請輸入內容：");
                //  string sendStr = Console.ReadLine();

                //  Socket clientSocket = new Socket(AddressFamily.InterNetwork,ProtocolType.Tcp);
                //  clientSocket.Connect(ipe);
                //  //send message
                //  //byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                //  byte[] sendBytes = Encoding.GetEncoding("utf-8").GetBytes(sendStr);

                //  //Thread.Sleep(4000);

                //  clientSocket.Send(sendBytes);

                //  //receive message
                //  string recStr = ""; 
                //  byte[] recBytes = new byte[4096];
                //  int bytes = clientSocket.Receive(recBytes,recBytes.Length,0);
                //  //recStr += Encoding.ASCII.GetString(recBytes,bytes);
                //  recStr += Encoding.GetEncoding("utf-8").GetString(recBytes,bytes);
                //  Console.WriteLine(recStr);

                //  clientSocket.Close();
                //  if (i >= 100)
                //  {
                //    break;
                //  }
                //  i++;
                //}

                //Console.ReadLine();
                //return;

                //string result = String.Empty;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        //接收服務端發來資訊的方法  
        public static void Recv()
        {
            int x = 0;
            //持續監聽服務端發來的訊息 
            while (true)
            {
                try
                {
                    //定義一個1M的記憶體緩衝區，用於臨時性儲存接收到的訊息 
                    byte[] arrRecvmsg = new byte[1024 * 1024];

                    //將客戶端套接字接收到的資料存入記憶體緩衝區，並獲取長度 
                    int length = SocketClient.Receive(arrRecvmsg);

                    //將套接字獲取到的字元陣列轉換為人可以看懂的字串 
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);
                    if (x == 1)
                    {
                        Console.WriteLine("\r\n伺服器：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + strRevMsg + "\r\n");

                    }
                    else
                    {
                        Console.WriteLine(strRevMsg + "\r\n");
                        x = 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("遠端伺服器已經中斷連線！" + ex.Message + "\r\n");
                    break;
                }
            }
        }

        //傳送字元資訊到服務端的方法 
        public static void ClientSendMsg(string sendMsg)
        {
            //將輸入的內容字串轉換為機器可以識別的位元組陣列   
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            //呼叫客戶端套接字傳送位元組陣列   
            SocketClient.Send(arrClientSendMsg);
        }
    }
}
