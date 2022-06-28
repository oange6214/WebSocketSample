using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CA_Server_IP6
{
    internal class Program
    {
        //建立一個和客戶端通訊的套接字
        static Socket SocketWatch = null;
        //定義一個集合，儲存客戶端資訊
        static Dictionary<string, Socket> ClientConnectionItems = new Dictionary<string, Socket> { };

        static IPHostEntry ipHostInfo;
        static IPAddress ipAddress;
        static private IPEndPoint ipe;

        static void Main(string[] args)
        {
            //埠號（用來監聽的）
            int port = 11000;

            //string host = "127.0.0.1";
            //IPAddress ip = IPAddress.Parse(host);
            IPAddress ip = IPAddress.Any;


            //ipHostInfo = Dns.GetHostEntry("192.168.3.249");
            //ipAddress = ipHostInfo.AddressList[0];
            //ipe = new IPEndPoint(IPAddress.Any, 11000);



            //將IP地址和埠號繫結到網路節點point上 
            IPEndPoint ipe = new IPEndPoint(ip, port);

            //定義一個套接字用於監聽客戶端發來的訊息，包含三個引數（IP4定址協議，流式連線，Tcp協議） 
            //SocketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketWatch = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            //監聽繫結的網路節點 
            SocketWatch.Bind(ipe);
            //將套接字的監聽佇列長度限制為20 
            SocketWatch.Listen(20);


            //負責監聽客戶端的執行緒:建立一個監聽執行緒 
            Thread threadwatch = new Thread(WatchConnecting);
            //將窗體執行緒設定為與後臺同步，隨著主執行緒結束而結束 
            threadwatch.IsBackground = true;
            //啟動執行緒   
            threadwatch.Start();

            Console.WriteLine("開啟監聽......");
            Console.WriteLine("點選輸入任意資料回車退出程式......");
            Console.ReadKey();

            SocketWatch.Close();

            //Socket serverSocket = null;

            //int i=1;
            //while (true)
            //{
            //  //receive message
            //  serverSocket = SocketWatch.Accept();
            //  Console.WriteLine("連線已經建立！");
            //  string recStr = "";
            //  byte[] recByte = new byte[4096];
            //  int bytes = serverSocket.Receive(recByte,recByte.Length,0);
            //  //recStr += Encoding.ASCII.GetString(recByte,bytes);
            //  recStr += Encoding.GetEncoding("utf-8").GetString(recByte,bytes);

            //  //send message
            //  Console.WriteLine(recStr);

            //  Console.Write("請輸入內容：");
            //  string sendStr = Console.ReadLine();

            //  //byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
            //  byte[] sendByte = Encoding.GetEncoding("utf-8").GetBytes(sendStr);

            //  //Thread.Sleep(4000);

            //  serverSocket.Send(sendByte,sendByte.Length,0);
            //  serverSocket.Close();
            //  if (i >= 100)
            //  {
            //    break;
            //  }
            //  i++;
            //}

            //sSocket.Close();
            //Console.WriteLine("連線關閉！");


            //Console.ReadLine();
        }

        //監聽客戶端發來的請求 
        static void WatchConnecting()
        {
            Socket connection = null;

            //持續不斷監聽客戶端發來的請求   
            while (true)
            {
                try
                {
                    connection = SocketWatch.Accept();
                }
                catch (Exception ex)
                {
                    //提示套接字監聽異常   
                    Console.WriteLine(ex.Message);
                    break;
                }

                //客戶端網路結點號 
                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                //新增客戶端資訊 
                ClientConnectionItems.Add(remoteEndPoint, connection);
                //顯示與客戶端連線情況
                Console.WriteLine("\r\n[客戶端\"" + remoteEndPoint + "\"建立連線成功！ 客戶端數量：" + ClientConnectionItems.Count + "]");

                //獲取客戶端的IP和埠號 
                IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

                //讓客戶顯示"連線成功的"的資訊 
                string sendmsg = "[" + "本地IP：" + clientIP + " 本地埠：" + clientPort.ToString() + " 連線服務端成功！]";
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                connection.Send(arrSendMsg);

                //建立一個通訊執行緒   
                Thread thread = new Thread(recv);
                //設定為後臺執行緒，隨著主執行緒退出而退出 
                thread.IsBackground = true;
                //啟動執行緒   
                thread.Start(connection);
            }
        }

        /// <summary>
        /// 接收客戶端發來的資訊，客戶端套接字物件
        /// </summary>
        /// <param name="socketclientpara"></param>  
        static void recv(object socketclientpara)
        {
            Socket socketServer = socketclientpara as Socket;

            while (true)
            {
                //建立一個記憶體緩衝區，其大小為1024*1024位元組 即1M   
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                //將接收到的資訊存入到記憶體緩衝區，並返回其位元組陣列的長度  
                try
                {
                    int length = socketServer.Receive(arrServerRecMsg);

                    //將機器接受到的位元組陣列轉換為人可以讀懂的字串   
                    string strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);

                    //將傳送的字串資訊附加到文字框txtMsg上   
                    Console.WriteLine("\r\n[客戶端：" + socketServer.RemoteEndPoint + " 時間：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "]\r\n" + strSRecMsg);

                    //Thread.Sleep(3000);
                    //socketServer.Send(Encoding.UTF8.GetBytes("[" + socketServer.RemoteEndPoint + "]："+strSRecMsg));
                    //傳送客戶端資料
                    if (ClientConnectionItems.Count > 0)
                    {
                        foreach (var socketTemp in ClientConnectionItems)
                        {
                            socketTemp.Value.Send(Encoding.UTF8.GetBytes("[" + socketServer.RemoteEndPoint + "]：" + strSRecMsg));
                        }
                    }
                }
                catch (Exception)
                {
                    ClientConnectionItems.Remove(socketServer.RemoteEndPoint.ToString());
                    //提示套接字監聽異常 
                    Console.WriteLine("\r\n[客戶端\"" + socketServer.RemoteEndPoint + "\"已經中斷連線！ 客戶端數量：" + ClientConnectionItems.Count + "]");
                    //關閉之前accept出來的和客戶端進行通訊的套接字 
                    socketServer.Close();
                    break;
                }
            }
        }
    }
}
