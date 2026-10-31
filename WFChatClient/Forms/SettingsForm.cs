using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

namespace WFChatClient
{
    public partial class SettingsForm : Form
    {
        [Serializable]
        public class Settings
        {
            [XmlElement("port")] //keeps the element names of existing settings.xml files
            public int Port {  get;  set; }
            [XmlElement("ipAdd")]
            public string IpAddress { get;  set; }
            public Settings(int port, string ipAddress)
            {
                this.Port = port;
                this.IpAddress = ipAddress;
            }
            public Settings() { }
        }
        public SettingsForm()
        {
            InitializeComponent();
        }
        XmlSerializer _xmlSerializer;
        FileStream _fs;
        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (mtbIPAddress.Text!=null && mtbPort.Text!= null)
            {
                try
                {
                    Settings settings = new Settings(Convert.ToInt32(mtbPort.Text), mtbIPAddress.Text);
                    try
                    {
                        _xmlSerializer = new XmlSerializer(typeof(Settings));
                        _fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
                        _xmlSerializer.Serialize(_fs, settings);

                        _fs.Close();
                        MessageBox.Show("Succesful");
                        SettingsForm.ActiveForm.Close();
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

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _xmlSerializer = new XmlSerializer(typeof(Settings));
            _fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
            try
            {
                Settings desSettings = (Settings)_xmlSerializer.Deserialize(_fs);

                mtbIPAddress.Text = desSettings.IpAddress.ToString();
                mtbPort.Text = desSettings.Port.ToString();
            }
            catch
            {
                mtbIPAddress.Text = "192.168.0.5";
                mtbPort.Text = "8888";
                MessageBox.Show("Cannot load settings, default values was set");
            }
            finally
            {
                _fs.Close();
            }
        }
    }
}
