using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

namespace WinFormClient
{
    public partial class FClientSettings : Form
    {
        [Serializable]
        public class Settings
        {
            public int port {  get;  set; }
            public string ipAdd { get;  set; }
            public Settings(int port, string ipAdd)
            {
                this.port = port;
                this.ipAdd = ipAdd;
            }
            public Settings() { }
        }
        public FClientSettings()
        {
            InitializeComponent();
        }
        XmlSerializer xmlSerializer;
        FileStream fs;
        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (mtbIPAddress.Text!=null && mtbPort.Text!= null)
            {
                try
                {
                    Settings settings = new Settings(Convert.ToInt32(mtbPort.Text), mtbIPAddress.Text);
                    try
                    {
                        xmlSerializer = new XmlSerializer(typeof(Settings));
                        fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
                        xmlSerializer.Serialize(fs, settings);

                        fs.Close();
                        MessageBox.Show("Succesful");
                        FClientSettings.ActiveForm.Close();
                    }
                    catch
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
            xmlSerializer = new XmlSerializer(typeof(Settings));
            fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
            try
            {
                Settings desSettings = (Settings)xmlSerializer.Deserialize(fs);

                mtbIPAddress.Text = desSettings.ipAdd.ToString();
                mtbPort.Text = desSettings.port.ToString();
            }
            catch
            {
                mtbIPAddress.Text = "192.168.0.5";
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
