using System;
using System.Threading;

namespace Server
{
    class Program
    {
        protected internal static ServerObject server;
        static ControllerObject controller;
        static Thread listenThread;
        static Thread controllerThread;
        static void Main(string[] args)
        {
           
            try
            {
                server = new ServerObject();
                controller = new ControllerObject();

                listenThread = new Thread(new ThreadStart(server.Listen)); 
                controllerThread = new Thread(new ThreadStart(controller.Controller));

                listenThread.Start();
                controllerThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ", click any button to continue");
                Console.ReadLine();
                server.Disconnect();

            }
        }
    }
}
