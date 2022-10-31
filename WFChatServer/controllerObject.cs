using System;
using System.Windows.Forms;

namespace WFChatServer
{
    class ControllerObject
    {
        internal TextBox tbSendCommand = Application.OpenForms["fMain"].Controls["tbSendCommand"] as TextBox;
        internal void Controller() //handler commands from tbSendCommand
        {
                string command = tbSendCommand.Text;

                switch (command)
                {
                    case "exit":
                        fMain.server.Disconnect();
                        break;
                    case "test":
                        Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                        "Test" + Environment.NewLine));
                        break;
                    case "users":
                        clientUserNamesPrint();
                        break;
                    case "help":
                        help();
                        break;
                    default:
                        Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                        "type 'help' for commands list" + Environment.NewLine));
                        break;
                }
            tbSendCommand.Text = "";
        }
        internal static void help()
        {
            Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
            String.Format("{0}: {1}", "exit: ", "Exit") + Environment.NewLine));
            Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
            String.Format("{0}: {1}", "users: ", "Print users list") + Environment.NewLine)); //workaround(?)
        }
        internal void clientUserNamesPrint()
        {
            Console.Write("Users list ");
            try
            {

                if (fMain.server.clients.Count > 0)
                {
                    for (int i = 0; i < fMain.server.clients.Count; i++)
                    {
                        /* Program.fMainReferense.tbController.Text = fMain.server.clients[i].userName + Environment.NewLine;*/
                        Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                        fMain.server.clients[i].userName + Environment.NewLine));
                    }
                }
                else
                {
                    /*Program.fMainReferense.tbController.Text += "is empty" + Environment.NewLine;*/
                    Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                        "is empty" + Environment.NewLine));
                }
            }
            catch
            {

            }
        }
    }
}
