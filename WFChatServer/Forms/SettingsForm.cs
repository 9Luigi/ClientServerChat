using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Xml.Serialization;

namespace WFChatServer
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        [Serializable]
        public class Settings
        {
            [XmlElement("ipaddr")] //keeps the element names of existing settings.xml files
            public string IpAddress { get; set; }
            [XmlElement("port")]
            public int Port { get; set; }
            public Settings(string ipAddress, int port)
            {
                this.IpAddress = ipAddress;
                this.Port = port;
            }
            public Settings() { }
        }
        XmlSerializer _xmlSerializer;
        FileStream _fs;
        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (mtbIPAddress.Text != "" && mtbPort.Text != "")
            {
                try
                {
                    Settings settings = new Settings(mtbIPAddress.Text, Convert.ToInt32(mtbPort.Text));
                    try
                    {
                        _xmlSerializer = new XmlSerializer(typeof(SettingsForm.Settings) );
                        _fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
                        _xmlSerializer.Serialize(_fs, settings);
                        _fs.Close();

                        MessageBox.Show("Succesful, reload the application for the changes to take effect");
                        SettingsForm.ActiveForm.Close();
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
    private void SettingsForm_Load(object sender, EventArgs e)
        {
            _xmlSerializer = new XmlSerializer(typeof(SettingsForm.Settings) );
            _fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
            try
            {
                Settings desSettings = (Settings)_xmlSerializer.Deserialize(_fs);


                mtbIPAddress.Text = desSettings.IpAddress.ToString();
                mtbPort.Text = desSettings.Port.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _fs.Close();
            }
        }
    }
}
