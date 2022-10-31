using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace WFChatServer
{
    public class ServerObject
    {
        static TcpListener listener;
        public List<ClientObject> clients = new List<ClientObject>();
        IPAddress IPAddr;
        int port;
        BinaryFormatter bf;
        FileStream fs;

        internal void AddConnection(ClientObject client)
        {
            try
            {
                clients.Add(client);
            }
            catch (Exception ex)
            {
                Program.fMainReferense.tbController.Text += ex.Message + Environment.NewLine;
            }
        }
        internal void RemoveConnection(string id)
        {
            try
            {
                ClientObject client = clients.FirstOrDefault(c => c.ID == id);
                if (client != null)
                {
                    clients.Remove(client);
                }
            }
            catch (Exception ex)
            {
                Program.fMainReferense.tbController.Text += ex.Message + Environment.NewLine;
            }
        }
        internal void Listen()
        {
           
            try
            {
                SelectServer(out IPAddr, out port); //values store in file settings.dat
                listener = new TcpListener(IPAddr, port);
                listener.Start(); //listen started
                Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                        "Server started" + Environment.NewLine));
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient(); //new tcpClient for each client
                    ClientObject clientOBJ = new ClientObject(client, this);//is added connected client to collection
                    Thread clientThread = new Thread(new ThreadStart(clientOBJ.Process)); //transfer data beetwen server and each client
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Program.fMainReferense.tbController.Invoke(new Action(() => Program.fMainReferense.tbController.Text +=
                ex.Message + Environment.NewLine));
            }
        }//listen and multi thread accept clients
        internal void BrodcastMessage(string message, string id)
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

        internal void Disconnect()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            listener.Stop();
            Environment.Exit(0);
        }
        internal void SelectServer(out IPAddress IPAddr, out int port)
        {
            try
            {
                bf = new BinaryFormatter();
                fs = new FileStream("settings.dat", FileMode.OpenOrCreate);
                fServerSettings.Settings desSettings = (fServerSettings.Settings)bf.Deserialize(fs);
                

                IPAddr = desSettings.ipaddr;
                port = desSettings.port;
            }
            catch
            {
                IPAddr = IPAddress.Parse("192.168.1.1");
                port  = 8888;
                
            }
            finally
            {
                fs.Close();
            }
        }

    }
}
