using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace WFChatClient
{
    public partial class MainForm : Form
    {
        TcpClient Client { get; set; }
        NetworkStream Stream { get; set; }
        IPAddress IpAddress { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        Thread ReceiveThread { get; set; }
        public MainForm()
        {
            InitializeComponent();
        }
        XmlSerializer XmlSerializer { get; set; }
        FileStream FileStream { get; set; }
        //TODO space can movecoret ~
        private void bSend_Click(object sender, EventArgs e)
        {
            try
            {
                string message = tbSend.Text;
                tbSend.Text = "";
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                Stream.Write(bytes, 0, bytes.Length);
                tbReceive.Text += Environment.NewLine + message;
            }
            catch (Exception ex)
            {
                tbReceive.Text += ex.Message;
                Disconnect();
                MessageBox.Show("Cannot communicate with server, try again later please");
                Environment.Exit(0);
            }
        }
        private void ReceiveMessage()
        {
            StringBuilder builder;
            int length;
            try
            {
                while (true)
                {
                    try
                    {
                        do
                        {
                            byte[] bytes = new byte[256];
                            builder = new StringBuilder(); //buidler appends buffer bytes from stream until data available in stream
                            length = Stream.Read(bytes, 0, bytes.Length);
                            builder.Append(Encoding.Unicode.GetString(bytes, 0, length));
                        }
                        while (Stream.DataAvailable);
                        tbReceive.Text += Environment.NewLine + builder.ToString();
                    }
                    catch
                    {
                        this.Invoke(new Action(() => tbSend.Enabled = false));
                        this.Invoke(new Action(() => bSend.Enabled = false));

                        Disconnect();
                        MessageBox.Show("Cannot communicate with server, try again later please");
                        Environment.Exit(0);
                    }
                }

            }
            catch
            {
                Disconnect();
                MessageBox.Show("Cannot communicate with server, try again later please");
                Environment.Exit(0);
            }
        }
        private void Disconnect()
        {
            if (Stream != null)
            {
                Stream.Close();
            }
            if (Client != null)
            {
                Client.Close();
            }
        }

        private void tbReceive_TextChanged(object sender, EventArgs e)
        {
            tbReceive.SelectionStart = tbReceive.Text.Length;
            tbReceive.ScrollToCaret();
        }

        internal void bLogin_Click(object sender, EventArgs e)
        {
            UserName = tbUsername.Text;
            Password = tbPassword.Text;
            StringBuilder builder;
            int length;
            try
            {
                #region desirializeSettings
                try
                {
                    XmlSerializer = new XmlSerializer(typeof(SettingsForm.Settings));
                    FileStream = new FileStream("settings.xml", FileMode.OpenOrCreate);
                    SettingsForm.Settings desSettings = (SettingsForm.Settings)XmlSerializer.Deserialize(FileStream);


                    IpAddress = IPAddress.Parse(desSettings.IpAddress);
                    Port = desSettings.Port;
                }
                catch
                {
                    IpAddress = IPAddress.Parse("192.168.0.5");
                    Port = 8888;
                }
                finally
                {
                    FileStream.Close();
                }
                #endregion
                Client = new TcpClient();

                Client.Connect(IpAddress, Port);
                Stream = Client.GetStream();
                string message = UserName + " " + Password;
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                Stream.Write(bytes, 0, bytes.Length);
                do
                {
                    byte[] receiveBytes = new byte[256];
                    builder = new StringBuilder(); //builder appends buffer bytes from stream until data available in stream
                    length = Stream.Read(receiveBytes, 0, receiveBytes.Length);
                    builder.Append(Encoding.Unicode.GetString(receiveBytes, 0, length));
                }
                while (Stream.DataAvailable);
                if (builder.ToString() == "User is not found")
                {
                    Disconnect();
                    this.Invoke(new Action(() => MessageBox.Show("User is not found, try again")));
                }
                else
                if (builder.ToString() == "Incorrect password")
                {
                    Disconnect();
                    this.Invoke(new Action(() => MessageBox.Show("Incorrect password, try again")));
                }
                else if(builder.ToString() == "OK")
                {

                    CloseLoginAndOpenChat();
                    tbReceive.Text = String.Format("Welcome {0}!", tbUsername.Text);

                    ReceiveThread = new Thread(new ThreadStart(ReceiveMessage));
                    ReceiveThread.IsBackground = true;
                    ReceiveThread.Start();

                }
            }
            catch(Exception ex)
            {
                tbSend.Enabled = false;
                bSend.Enabled = false;

                Disconnect();
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Cannot communicate with server, try again later please");
                Environment.Exit(0);
            }
        }

        private void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = true; //avoid new line(key realese event(?))
                    bSend_Click(sender, e);
                    break;
                case Keys.Back:
                    e.SuppressKeyPress = true;
                    if (tbSend.SelectionStart > 0)
                    {
                        SendKeys.Send("+{LEFT}{DEL}"); //delete a word with ctrl+backspace, don't undestand how yet
                    }
                    break;
            }
        }

        private void tbReceive_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = tbSend;
        }

        private void tbSend_TextChanged(object sender, EventArgs e)
        {
            lSymbolLeftCount.Text = (tbSend.MaxLength - tbSend.TextLength).ToString() + " symbol left";
        }

        private void bChangeServer_Click(object sender, EventArgs e)
        {
            SettingsForm fsettings = new SettingsForm();
            fsettings.ShowDialog();
        }
        internal void CloseLoginAndOpenChat()
        {
            bLogin.Visible = false;
            tbPassword.Visible = false;
            tbUsername.Visible = false;
            tbReceive.Visible = true;
            tbSend.Visible = true;
            bSend.Visible = true;
            label1.Visible = false;
            label2.Visible = false;
            lSymbolLeftCount.Visible = true;
            bChangeServer.Visible = false;
            this.Size = new Size(816, 489);
            this.ActiveControl = tbSend;
        }
    }
}
