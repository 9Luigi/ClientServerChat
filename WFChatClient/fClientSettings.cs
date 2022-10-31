using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace WinFormClient
{
    public partial class fClientSettings : Form
    {
        [Serializable]
        public class Settings
        {
            public int port { get; private set; }
            public IPAddress ipAdd { get; private set; }
            public Settings(int port, IPAddress ipAdd)
            {
                this.port = port;
                this.ipAdd = ipAdd;
            }
        }
        public fClientSettings()
        {
            InitializeComponent();
        }
        BinaryFormatter bf;
        FileStream fs;
        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (mtbIPAddress.Text!=null && mtbPort.Text!= null)
            {
                try
                {
                    Settings settings = new Settings(Convert.ToInt32(mtbPort.Text), IPAddress.Parse(mtbIPAddress.Text));
                    try
                    {
                        bf = new BinaryFormatter();
                        fs = new FileStream("settings.dat", FileMode.OpenOrCreate);
                        bf.Serialize(fs, settings);
                        
                        fs.Close();
                        MessageBox.Show("Succesful");
                        fClientSettings.ActiveForm.Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Cannot save settings, try to reload application");
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

                
                mtbIPAddress.Text = desSettings.ipAdd.ToString();
                mtbPort.Text = desSettings.port.ToString();
            }
            catch
            {
                mtbIPAddress.Text = "192.168.1.1";
                mtbPort.Text = "8888";
                MessageBox.Show("Cannot load settings, default values was set");
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
