using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace Client
{
    class Program
    {
        static string userName;
        static int port;
        static NetworkStream stream;
        static TcpClient client;
        static IPAddress IPaddr;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter username");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                IPaddr = IPAddress.Parse("192.168.1.1");
                port = 8888;

                client.Connect(IPaddr, port);
                stream = client.GetStream();

                Console.WriteLine("Hello {0} !", userName);

                string message = userName;
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
                
                    try
                    {
                        Thread recieveThread = new Thread(new ThreadStart(recieveMessage));
                        recieveThread.Start();
                        sendMessage();

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        Disconnect();
                    }
            }
            catch(Exception ex)
            {
                //server is down, please try again later, press any key to exit
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Disconnect();
            }
            
        }
        static private void sendMessage()
        {
            Console.WriteLine("Now you can send messages!");
            while (true)
            {
                string message = Console.ReadLine();
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        static private void recieveMessage()
        {
            while (true)
            {
                byte[] bytes = new byte[256];
                StringBuilder builder = new StringBuilder(); //buidler appends buffer bytes from stream until data available in stream
                int lenght;
                try
                {
                     do
                    {
                         lenght = stream.Read(bytes, 0, bytes.Length);
                         builder.Append(Encoding.Unicode.GetString(bytes, 0, lenght));
                    } 
                while (stream.DataAvailable);
                Console.WriteLine(builder.ToString());
            }
                catch (Exception ex)
                 {
                    Console.WriteLine(ex.Message + " try to connect later please");
                    Console.ReadLine();
                    Disconnect();
                }
            }

        } 
        static private void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (client != null)
            {
                client.Close();
            }
            Environment.Exit(0);

        }
    }
}