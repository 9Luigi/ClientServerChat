using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
namespace WFChatServer
{
    public partial class fServerSettings : Form
    {
        public fServerSettings()
        {
            InitializeComponent();
        }
        [Serializable]
        internal class Settings
        {
            public IPAddress ipaddr;
            public int port;
            public Settings(IPAddress ipaddr, int port)
            {
                this.ipaddr = ipaddr;
                this.port = port;
            }
        }
        BinaryFormatter bf;
        FileStream fs;
        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (mtbIPAddress.Text != "" && mtbPort.Text != "")
            {
                try
                {
                    Settings settings = new Settings( IPAddress.Parse(mtbIPAddress.Text), Convert.ToInt32(mtbPort.Text));
                    try
                    {
                        bf = new BinaryFormatter();
                        fs = new FileStream("settings.dat", FileMode.OpenOrCreate);
                        bf.Serialize(fs, settings);
                        fs.Close();
                        
                        MessageBox.Show("Succesful, reload the application for the changes to take effect");
                        fServerSettings.ActiveForm.Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch
                {
                    MessageBox.Show("Incorrect port or ip address");
                }

            }
            else
            {
                MessageBox.Show("Fill in all the fields");
            }
        }
    private void fSettings_Load(object sender, EventArgs e)
        {
            try
            {
                bf = new BinaryFormatter();
                fs = new FileStream("settings.dat", FileMode.OpenOrCreate);
                Settings desSettings = (Settings)bf.Deserialize(fs);

                
                mtbIPAddress.Text = desSettings.ipaddr.ToString();
                mtbPort.Text = desSettings.port.ToString();
            }
            catch
            {
                mtbIPAddress.Text = "192.168.1.1";
                mtbPort.Text = "8888";
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
