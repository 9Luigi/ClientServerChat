using System;
using System.Windows.Forms;

namespace WFChatServer
{
    internal class commandEventArgs : EventArgs
    {
        internal string message { get; private set; }
        public commandEventArgs(string message)
        {
            this.message = message;
        }
    }
    class ControllerObject
    {
        internal delegate void returnCommandDelegate(commandEventArgs e);
        internal event returnCommandDelegate returnCommandEvent;
        internal void Controller(string command) //handler commands from tbSendCommand
        {
            switch (command.ToLower())
            {
                case "exit":
                    returnCommandEvent.Invoke(new commandEventArgs("disconnect"));
                    break;
                case "test":
                    returnCommandEvent.Invoke(new commandEventArgs("test"));
                    break;
                case "users":
                    returnCommandEvent.Invoke(new commandEventArgs("users"));
                    break;
                case "help":
                    returnCommandEvent.Invoke(new commandEventArgs("help"));
                    break;
                default:
                    returnCommandEvent.Invoke(new commandEventArgs("help"));
                    break;
            }
        }
    }
}
