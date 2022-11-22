using System;
using System.Net.Sockets;
using System.Text;

namespace WFChatServer
{
    public class ClientObject
    {
        internal string ID { get; private set; }
        internal NetworkStream stream { get; private set; }
        internal string userName { get; private set; }
        internal string[] userNameAndPassword { get; private set; }
        internal string password { get; private set; }
        internal TcpClient client { get; private set; }
        internal ServerObject server { get; private set; }

        internal ClientObject(TcpClient client, ServerObject server)
        {
            ID = Guid.NewGuid().ToString(); //set uniq ID property for each clientOBJ
            this.client = client;
            this.server = server;
            server.AddConnection(this); //is adding cliebtOBJ to collection
        }
        internal void Process()
        {
            try
            {
                string messageSendTime = DateTime.Now.ToString("hh:mm:ss ");
                stream = client.GetStream();
                string message = GetMessage();
                userNameAndPassword = message.Split(' ');
                userName = userNameAndPassword[0];
                password = userNameAndPassword[1];
                if (server.dataBaseHandler.AuthorizeDB(userName, password) == "OK")
                {
                    message = messageSendTime + userName + " entered the chat";
                    server.BrodcastMessage(message, this.ID);
                    Program.fMainReferense.tbChatObs.Invoke(new Action(() => Program.fMainReferense.tbChatObs.Text +=
                            message + Environment.NewLine));
                    server.SendMessage(server.dataBaseHandler.AuthorizeDB(userName, password), this);
                    while (true)
                    {
                        messageSendTime = DateTime.Now.ToString("hh:mm:ss ");
                        try
                        {
                            message = GetMessage();
                            message = String.Format("{0}{1}: {2}", messageSendTime, userName, message);
                            Program.fMainReferense.tbChatObs.Invoke(new Action(() => Program.fMainReferense.tbChatObs.Text +=
                            message + Environment.NewLine));
                            server.BrodcastMessage(message, this.ID);
                        }
                        catch (Exception)
                        {
                            message = String.Format("{0}{1} lived the chat", messageSendTime, userName);
                            Program.fMainReferense.tbChatObs.Invoke(new Action(() => Program.fMainReferense.tbChatObs.Text +=
                            message + Environment.NewLine));
                            server.BrodcastMessage(message, this.ID);
                            break;
                        }
                    }
                }
                else
                {
                    server.SendMessage(server.dataBaseHandler.AuthorizeDB(userName, password), this);
                    server.RemoveConnection(this.ID);
                    Close();
                }

            }
            catch (Exception ex)
            {
                Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                        ex.Message + Environment.NewLine));
            }
            finally
            {
                server.RemoveConnection(this.ID);
                Close();
            }


        }
        private string GetMessage()
        {

            byte[] data = new byte[64]; //buffer
            int lenght;
            StringBuilder builder = new StringBuilder();
            do
            {
                lenght = stream.Read(data, 0, data.Length); //get lenght of data in stream(count of bytes)
                builder.Append(Encoding.Unicode.GetString(data, 0, lenght)); //add to to string while data available 
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }
        internal void Close()
        {
            if (client != null)
            {
                client.Close();
            }
            if (stream != null)
            {
                stream.Close();
            }
        }
    }
}