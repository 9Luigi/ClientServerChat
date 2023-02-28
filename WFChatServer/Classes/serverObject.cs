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
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using WFChatServer.Properties;

namespace WFChatServer
{
    internal class serverObjectToFMainEventArgs : EventArgs
    {
        internal string context { get; private set; }
        internal string message { get; private set; }
        public serverObjectToFMainEventArgs(string context,string message)
        {
            this.context = context;
            this.message = message;
        } 
    }
    public class ServerObject
    {
        internal static TcpListener listener { get; private set; }
        public List<ClientObject> clients = new List<ClientObject>();
        internal string IPAddr;
        internal int port;
        internal XmlSerializer xmlSerializer { get; set; }
        internal FileStream fs { get; private set; }
        internal DBHandler dataBaseHandler = new DBHandler();

        internal  delegate void serverObjectToFMainFormDelegate(serverObjectToFMainEventArgs e);
        internal event serverObjectToFMainFormDelegate serverObjectToFMainFormEvent;

        internal delegate void transferDelegate(ClientObject sender, ClientObjectEventArgs e);
        internal event transferDelegate transferEvent;
        internal void AddConnection(ClientObject client)
        {
            try
            {
                clients.Add(client);
            }
            catch (Exception ex)
            {
                serverObjectToFMainFormEvent.Invoke(new serverObjectToFMainEventArgs("tbController",ex.Message));
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
                serverObjectToFMainFormEvent.Invoke(new serverObjectToFMainEventArgs("tbController", ex.Message));
            }
        }
        internal void Listen()
        {
           
            try
            {
                SelectServer(out IPAddr, out port); //values store in file settings.xml
                listener = new TcpListener(IPAddress.Parse(IPAddr), port);
                listener.Start(); //listen started
                serverObjectToFMainFormEvent.Invoke(new serverObjectToFMainEventArgs("tbChatObs", "server started"));
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient(); //new tcpClient for each client
                    ClientObject clientOBJ = new ClientObject(client, this);//is added connected client to collection
                    clientOBJ.notifacationEvent += TransferDataFromClientObjectToFMain;
                    Thread clientThread = new Thread(new ThreadStart(clientOBJ.Process)); //transfer data beetwen server and each client
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                serverObjectToFMainFormEvent.Invoke(new serverObjectToFMainEventArgs("tbController", ex.Message));
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
        internal void SelectServer(out string IPAddr, out int port)
        {
            xmlSerializer = new XmlSerializer(typeof(FServerSettings.Settings) );
            fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
            try
            {
                FServerSettings.Settings desSettings = (FServerSettings.Settings)xmlSerializer.Deserialize(fs);

                IPAddr = desSettings.ipaddr;
                port = desSettings.port;
            }
            catch
            {
                var host = Dns.GetHostName();
                var ip = Dns.GetHostEntry(host).AddressList[2];
                IPAddr = Convert.ToString(ip);
                port = 8888;
                FServerSettings.Settings settings = new FServerSettings.Settings(IPAddr,port);
                xmlSerializer.Serialize(fs, settings);
                MessageBox.Show("Cannot load settings, default was set");
            }
            finally
            {
                fs.Close();
            }
        }
        void TransferDataFromClientObjectToFMain(ClientObject sender, ClientObjectEventArgs e)
        {
            transferEvent.Invoke(sender, e);
        }
    }
}
