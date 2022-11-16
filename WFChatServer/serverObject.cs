using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace WFChatServer
{
    public class ServerObject
    {
        internal static TcpListener listener { get; private set; }
        public List<ClientObject> clients = new List<ClientObject>();
        internal IPAddress IPAddr;
        internal int port;
        internal BinaryFormatter bf { get; private set; }
        internal FileStream fs { get; private set; }

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
        internal void SendMessage(string message, ClientObject client)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            client.stream.Write(data, 0, data.Length);

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
                var host = Dns.GetHostName();
                var ip = Dns.GetHostEntry(host).AddressList[1];
                IPAddr = ip;
                port = 8888;
                fServerSettings.Settings settings = new fServerSettings.Settings(IPAddr,port);
                bf.Serialize(fs, settings);
                MessageBox.Show("Cannot load settings, default was set");
            }
            finally
            {
                fs.Close();
            }
        }

    }
}
