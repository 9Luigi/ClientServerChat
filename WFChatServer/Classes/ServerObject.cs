using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WFChatServer
{
    internal class ServerObjectToMainFormEventArgs : EventArgs
    {
        internal string Context { get; private set; }
        internal string Message { get; private set; }
        public ServerObjectToMainFormEventArgs(string context,string message)
        {
            this.Context = context;
            this.Message = message;
        }
    }
    public class ServerObject
    {
        internal static TcpListener Listener { get; private set; }
        public List<ClientObject> Clients = new List<ClientObject>();
        internal string IpAddress;
        internal int Port;
        internal XmlSerializer XmlSerializer { get; set; }
        internal FileStream FileStream { get; private set; }
        internal DBHandler DataBaseHandler = new DBHandler();

        internal  delegate void ServerObjectToMainFormDelegate(ServerObjectToMainFormEventArgs e);
        internal event ServerObjectToMainFormDelegate ServerObjectToMainFormEvent;

        internal delegate void TransferDelegate(ClientObject sender, ClientObjectEventArgs e);
        internal event TransferDelegate TransferEvent;
        internal void AddConnection(ClientObject client)
        {
            try
            {
                Clients.Add(client);
            }
            catch (Exception ex)
            {
                ServerObjectToMainFormEvent.Invoke(new ServerObjectToMainFormEventArgs("tbController",ex.Message));
            }
        }
        internal void RemoveConnection(string id)
        {
            try
            {
                ClientObject client = Clients.FirstOrDefault(c => c.ID == id);
                if (client != null)
                {
                    Clients.Remove(client);
                }
            }
            catch (Exception ex)
            {
                ServerObjectToMainFormEvent.Invoke(new ServerObjectToMainFormEventArgs("tbController", ex.Message));
            }
        }
        internal void Listen()
        {

            try
            {
                SelectServer(out IpAddress, out Port); //values store in file settings.xml
                Listener = new TcpListener(IPAddress.Parse(IpAddress), Port);
                Listener.Start(); //listen started
                ServerObjectToMainFormEvent.Invoke(new ServerObjectToMainFormEventArgs("tbChatObs", "server started"));
                while (true)
                {
                    TcpClient client = Listener.AcceptTcpClient(); //new tcpClient for each client
                    ClientObject clientObj = new ClientObject(client, this);//added to the collection only after authorization, see Process
                    clientObj.NotificationEvent += TransferDataFromClientObjectToMainForm;
                    Thread clientThread = new Thread(new ThreadStart(clientObj.Process)); //transfer data beetwen server and each client
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                ServerObjectToMainFormEvent.Invoke(new ServerObjectToMainFormEventArgs("tbController", ex.Message));
            }
        }//listen and multi thread accept clients
        internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].ID != id)
                {
                    Clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }
        internal void SendMessage(string message, ClientObject client)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            client.Stream.Write(data, 0, data.Length);

        }
        internal void Disconnect()
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Close();
            }
            Listener.Stop();
            Environment.Exit(0);
        }
        internal void SelectServer(out string ipAddress, out int port)
        {
            XmlSerializer = new XmlSerializer(typeof(SettingsForm.Settings) );
            FileStream = new FileStream("settings.xml", FileMode.OpenOrCreate);
            try
            {
                SettingsForm.Settings desSettings = (SettingsForm.Settings)XmlSerializer.Deserialize(FileStream);

                ipAddress = desSettings.IpAddress;
                port = desSettings.Port;
            }
            catch
            {
                var host = Dns.GetHostName();
                var ip = Dns.GetHostEntry(host).AddressList[2];
                ipAddress = Convert.ToString(ip);
                port = 8888;
                SettingsForm.Settings settings = new SettingsForm.Settings(ipAddress,port);
                XmlSerializer.Serialize(FileStream, settings);
                MessageBox.Show("Cannot load settings, default was set");
            }
            finally
            {
                FileStream.Close();
            }
        }
        void TransferDataFromClientObjectToMainForm(ClientObject sender, ClientObjectEventArgs e)
        {
            TransferEvent.Invoke(sender, e);
        }
    }
}
