using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ControllerObject
    {
        protected internal void Controller()
        {
            while (true)
            {
                string command = Console.ReadLine();

                switch (command)
                {
                    case "exit":
                        Program.server.Disconnect();
                        break;
                    case "test":
                        Console.WriteLine("test");
                        break;
                    case "users":
                        clientUserNamesPrint();
                        break;
                    case "help":
                        Help();
                        break;
                    default:
                        Console.WriteLine("Command not found,type help and press enter");
                        break;
                }
            }
        }
        protected internal static void Help()
        {
            Console.WriteLine("{0}: {1}", "exit: ", "Exit");
            Console.WriteLine("{0}: {1}", "users: ", "Print users list");
        }
        protected internal void clientUserNamesPrint()
        {
            Console.Write("Users list ");
            try
            {

                if (Program.server.clients.Count > 0)
                {
                    for (int i = 0; i < Program.server.clients.Count; i++)
                    {
                        Console.Write(Program.server.clients[i].userName + ", ");
                    }
                }
                else
                {
                    Console.Write("is empty");
                }
            }
            catch
            {

            }
        }
    }
}
