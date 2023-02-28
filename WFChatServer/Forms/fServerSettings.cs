using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Xml.Serialization;

namespace WFChatServer
{
    public partial class FServerSettings : Form
    {
        public FServerSettings()
        {
            InitializeComponent();
        }
        [Serializable]
        public class Settings
        {
            public string ipaddr { get; set; }
            public int port { get; set; }
            public Settings(string ipaddr, int port)
            {
                this.ipaddr = ipaddr;
                this.port = port;
            }
            public Settings() { }
        }
        XmlSerializer xmlSerializer;
        FileStream fs;
        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (mtbIPAddress.Text != "" && mtbPort.Text != "")
            {
                try
                {
                    Settings settings = new Settings(mtbIPAddress.Text, Convert.ToInt32(mtbPort.Text));
                    try
                    {
                        xmlSerializer = new XmlSerializer(typeof(FServerSettings.Settings) );
                        fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
                        xmlSerializer.Serialize(fs, settings);
                        fs.Close();
                        
                        MessageBox.Show("Succesful, reload the application for the changes to take effect");
                        FServerSettings.ActiveForm.Close();
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
            xmlSerializer = new XmlSerializer(typeof(FServerSettings.Settings) );
            fs = new FileStream("settings.xml", FileMode.OpenOrCreate);
            try
            {
                Settings desSettings = (Settings)xmlSerializer.Deserialize(fs);

                
                mtbIPAddress.Text = desSettings.ipaddr.ToString();
                mtbPort.Text = desSettings.port.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
