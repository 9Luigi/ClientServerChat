using System;
using System.Windows.Forms;

namespace WFChatServer
{
    internal class CommandEventArgs : EventArgs
    {
        internal string Message { get; private set; }
        public CommandEventArgs(string message)
        {
            this.Message = message;
        }
    }
    class ControllerObject
    {
        internal delegate void ReturnCommandDelegate(CommandEventArgs e);
        internal event ReturnCommandDelegate ReturnCommandEvent;
        internal void Controller(string command) //handler commands from tbSendCommand
        {
            switch (command.ToLower())
            {
                case "exit":
                    ReturnCommandEvent.Invoke(new CommandEventArgs("disconnect"));
                    break;
                case "test":
                    ReturnCommandEvent.Invoke(new CommandEventArgs("test"));
                    break;
                case "users":
                    ReturnCommandEvent.Invoke(new CommandEventArgs("users"));
                    break;
                case "help":
                    ReturnCommandEvent.Invoke(new CommandEventArgs("help"));
                    break;
                default:
                    ReturnCommandEvent.Invoke(new CommandEventArgs("help"));
                    break;
            }
        }
    }
}
