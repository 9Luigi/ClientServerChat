using System;
using System.Net.Sockets;
using System.Text;

namespace WFChatServer
{
    public class ClientObjectEventArgs : EventArgs
    {
        internal string Message { get; private set; }
        internal string Target { get; private set; }
        public ClientObjectEventArgs(string message, string target)
        {
            this.Message = message;
            this.Target = target;
        }
    }
    public class ClientObject
    {
        internal string ID { get; private set; }
        internal NetworkStream Stream { get; private set; }
        internal string UserName { get; private set; }
        internal string[] UserNameAndPassword { get; private set; }
        internal string Password { get; private set; }
        internal TcpClient Client { get; private set; }
        internal ServerObject Server { get; private set; }

        internal delegate void NotificationDelegate(ClientObject sender, ClientObjectEventArgs e);
        internal event NotificationDelegate NotificationEvent;
        internal ClientObject(TcpClient client, ServerObject server)
        {
            ID = Guid.NewGuid().ToString(); //set uniq ID property for each clientOBJ
            this.Client = client;
            this.Server = server;
        }
        internal void Process()
        {
            try
            {
                string messageSendTime = DateTime.Now.ToString("hh:mm:ss ");
                Stream = Client.GetStream();
                string message = GetMessage();
                UserNameAndPassword = message.Split(' ');
                UserName = UserNameAndPassword[0];
                Password = UserNameAndPassword[1];
                string authorizeResult = Server.DataBaseHandler.AuthorizeDB(UserName, Password);
                if (authorizeResult == "OK")
                {
                    Server.SendMessage(authorizeResult, this); //before AddConnection so the "OK" reply can't be mixed with a broadcast
                    Server.AddConnection(this); //only authorized clients receive chat messages
                    message = messageSendTime + UserName + " entered the chat";
                    Server.BroadcastMessage(message, this.ID);
                    NotificationEvent.Invoke(this, new ClientObjectEventArgs(message, "tbChatObs"));
                    while (true)
                    {
                        messageSendTime = DateTime.Now.ToString("hh:mm:ss ");
                        try
                        {
                            message = GetMessage();
                            message = String.Format("{0}{1}: {2}", messageSendTime, UserName, message);
                            NotificationEvent.Invoke(this, new ClientObjectEventArgs(message, "tbChatObs"));
                            Server.BroadcastMessage(message, this.ID);
                        }
                        catch (Exception)
                        {
                            message = String.Format("{0}{1} lived the chat", messageSendTime, UserName);
                            NotificationEvent.Invoke(this, new ClientObjectEventArgs(message, "tbChatObs"));
                            Server.BroadcastMessage(message, this.ID);
                            break;
                        }
                    }
                }
                else
                {
                    Server.SendMessage(authorizeResult, this);
                    Close();
                }

            }
            catch (Exception ex)
            {
                NotificationEvent.Invoke(this, new ClientObjectEventArgs(ex.Message, "tbController"));
            }
            finally
            {
                Server.RemoveConnection(this.ID);
                Close();
            }


        }
        private string GetMessage()
        {

            byte[] data = new byte[64]; //buffer
            int length;
            StringBuilder builder = new StringBuilder();
            do
            {
                length = Stream.Read(data, 0, data.Length); //get lenght of data in stream(count of bytes)
                builder.Append(Encoding.Unicode.GetString(data, 0, length)); //add to to string while data available
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }
        internal void Close()
        {
            if (Client != null)
            {
                Client.Close();
            }
            if (Stream != null)
            {
                Stream.Close();
            }
        }
    }
}
