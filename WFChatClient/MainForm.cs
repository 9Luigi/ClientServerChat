using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Drawing2D;
using System.Reflection.Emit;

namespace WinFormClient
{
    public partial class mainForm : Form
    {
        TcpClient client;
        NetworkStream stream;
        IPAddress ipAddr;
        int port;
        BinaryFormatter bf;
        FileStream fs;
        string Username, Password;
        public mainForm()
        {
            InitializeComponent();
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            try
            {
                string message = tbSend.Text;
                tbSend.Text = "";
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
                tbRecieve.Text += Environment.NewLine + message;
            }
            catch (Exception ex)
            {
                tbRecieve.Text += ex.Message;
                Disconnect();
            }
        }
        private void recieveMessage() //TODO here
        {
            
            StringBuilder builder; 
            int lenght;
            try
            {
                do
                {
                    byte[] bytes = new byte[256];
                    builder = new StringBuilder(); //buidler appends buffer bytes from stream until data available in stream 
                    lenght = stream.Read(bytes, 0, bytes.Length);
                    builder.Append(Encoding.Unicode.GetString(bytes, 0, lenght));
                }
                while (stream.DataAvailable);
                if (builder.ToString() == "User is not found")
                {
                    Disconnect();
                    mainForm.ActiveForm.Invoke(new Action(() => MessageBox.Show("User is not found, try again")));
                }
                if (builder.ToString() == "Incorrect password")
                {
                    Disconnect();
                    mainForm.ActiveForm.Invoke(new Action(() => MessageBox.Show("Incorrect password, try again")));
                }
                else
                {
                    mainForm.ActiveForm.Invoke(new Action(() => closeLoginAndOpenChat()));
                    mainForm.ActiveForm.Invoke(new Action(() => tbRecieve.Text = String.Format("Welcome {0}!", tbUsername.Text)));
                    while (true)
                    {
                        try
                        {
                            do
                            {
                                byte[] bytes = new byte[256];
                                builder = new StringBuilder(); //buidler appends buffer bytes from stream until data available in stream 
                                lenght = stream.Read(bytes, 0, bytes.Length);
                                builder.Append(Encoding.Unicode.GetString(bytes, 0, lenght));
                            }
                            while (stream.DataAvailable);
                            tbRecieve.Text += Environment.NewLine + builder.ToString();
                        }
                        catch
                        {
                            tbSend.Invoke(new Action(() => tbSend.Enabled = false));
                            bSend.Invoke(new Action(() => bSend.Enabled = false));

                            Disconnect();
                            MessageBox.Show("Cannot communicate with server, try again later please");
                            Environment.Exit(0);
                        }
                    }
                }

            }
            catch
            {
                tbSend.Invoke(new Action(() => tbSend.Enabled = false));
                bSend.Invoke(new Action(() => bSend.Enabled = false));

                Disconnect();
                MessageBox.Show("Cannot communicate with server, try again later please");
                Environment.Exit(0);
            }
        }
        private void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (client != null)
            {
                client.Close();
            }
        }

        private void tbRecieve_TextChanged(object sender, EventArgs e)
        {
            tbRecieve.SelectionStart = tbRecieve.Text.Length;
            tbRecieve.ScrollToCaret();
        }

        internal void bLogin_Click(object sender, EventArgs e)
        {
            Username = tbUsername.Text;
            Password = tbPassword.Text;
            /*string check = DataBaseSQL.AuthorizeDB();
            if (check == "OK")
            {*/ //TODO change login messageboxes
            try
            {

                try
                {
                    try
                    {
                        bf = new BinaryFormatter();
                        fs = new FileStream("settings.dat", FileMode.OpenOrCreate);
                        fClientSettings.Settings desSettings = (fClientSettings.Settings)bf.Deserialize(fs);


                        ipAddr = desSettings.ipAdd;
                        port = desSettings.port;
                    }
                    catch
                    {
                        ipAddr = IPAddress.Parse("192.168.0.5");
                        port = 8888;
                    }
                    finally
                    {
                        fs.Close();
                    }

                    client = new TcpClient();

                    client.Connect(ipAddr, port);
                    stream = client.GetStream();
                    string message = Username + " " + Password;
                    byte[] bytes = Encoding.Unicode.GetBytes(message);
                    stream.Write(bytes, 0, bytes.Length);

                    Thread recieveThread = new Thread(new ThreadStart(recieveMessage));
                    recieveThread.IsBackground = true;
                    recieveThread.Start();

                }
                catch
                {
                    tbSend.Enabled = false;
                    bSend.Enabled = false;

                    Disconnect();
                    MessageBox.Show("Cannot communicate with server, try again later please");
                    Environment.Exit(0);
                }
            }
            catch
            {
                tbSend.Enabled = false;
                tbRecieve.Text += "" + Environment.NewLine;
                bSend.Enabled = false;

                Disconnect();
                MessageBox.Show("Login is OK, but server is down, try again later please");
                Environment.Exit(0);
            }


            //}
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

        private void tbRecieve_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = tbSend;
        }

        private void tbSend_TextChanged(object sender, EventArgs e)
        {
            lSymbolLeftCount.Text = (tbSend.MaxLength - tbSend.TextLength).ToString() + " symbol left";
        }

        private void bChangeServer_Click(object sender, EventArgs e)
        {
            fClientSettings fsettings = new fClientSettings();
            fsettings.ShowDialog();
        }
        internal void closeLoginAndOpenChat()
        {
            bLogin.Visible = false;
            tbPassword.Visible = false;
            tbUsername.Visible = false;
            tbRecieve.Visible = true;
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
