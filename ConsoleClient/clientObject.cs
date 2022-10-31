using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Server
{
    public class ClientObject
    {
        protected internal string ID;
        protected internal NetworkStream stream;
        public string userName;
        TcpClient client;
        ServerObject server;
        protected internal ClientObject(TcpClient client, ServerObject server)
        {
            ID = Guid.NewGuid().ToString(); //set uniq ID property for each clientOBJ
            this.client = client;
            this.server = server;
            server.AddConnection(this); //add cliebtOBJ to collection
        }
        protected internal void Process()
        {
            try
            {
                stream = client.GetStream();
                string message = GetMessage();
                userName = message;

                message = userName + " entered the chat";
                server.BrodcastMessage(message, this.ID);
                Console.WriteLine(message);
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BrodcastMessage(message, this.ID);
                    }
                    catch (Exception)
                    {
                        message = String.Format("{0} lived the chat", userName);
                        Console.WriteLine(message);
                        server.BrodcastMessage(message, this.ID);
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                builder.Append(Encoding.Unicode.GetString(data, 0, lenght)); //get string from buffer by count of lenght
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }
        protected internal void Close()
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