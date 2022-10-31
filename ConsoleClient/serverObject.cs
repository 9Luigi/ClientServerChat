using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;

namespace Server
{
    public class ServerObject
    {
        static TcpListener listener;
        public List<ClientObject> clients = new List<ClientObject>();
        IPAddress IPAddr = IPAddress.Parse("192.168.1.1");
        int port = 8888;
        protected internal void AddConnection(ClientObject client)
        {
            try
            {
                clients.Add(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        protected internal void RemoveConnection(string id)
        {
            try
            {
                ClientObject client = clients.FirstOrDefault(c => c.ID == id); //??
                if (client != null)
                {
                    clients.Remove(client);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }
        protected internal void Listen()
        {
            try
            {
                listener = new TcpListener(IPAddr, port);
                listener.Start(); //listen started
                Console.WriteLine("Server started, enter help for command list");
                while (true) //each new client is added to collection and create new thread for each TCPClient 
                {
                    TcpClient client = listener.AcceptTcpClient(); //new tcpClient for each client
                    ClientObject clientOBJ = new ClientObject(client, this); //Each client is added to collection of this server
                    Thread clientThread = new Thread(new ThreadStart(clientOBJ.Process)); // data transfer process between server and each client
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }//listen and multi thread accept clients
        protected internal void BrodcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].ID != id)
                {
                    clients[i].stream.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            listener.Stop();
            Environment.Exit(0);
        }

    }
}
