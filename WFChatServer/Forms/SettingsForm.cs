using System;
using System.IO;
using System.Windows.Forms;
using WFChat.UI;

namespace WFChatServer
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            Theme.StylePrimary(bSaveSettings);
        }

        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(mtbPort.Text, out int port) || !ServerSettings.IsValid(mtbIPAddress.Text, port))
            {
                MessageBox.Show("Incorrect port or ip address");
                return;
            }
            try
            {
                ServerSettings settings = ServerSettings.Load(ServerSettings.DefaultPath) ?? ServerSettings.CreateDefault(); //keeps the log level
                settings.IpAddress = mtbIPAddress.Text;
                settings.Port = port;
                settings.Save(ServerSettings.DefaultPath);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Succesful, reload the application for the changes to take effect");
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ServerSettings settings = ServerSettings.Load(ServerSettings.DefaultPath);
            if (settings == null)
            {
                settings = ServerSettings.CreateDefault();
                MessageBox.Show("Cannot load settings, default values were set");
            }
            mtbIPAddress.Text = settings.IpAddress;
            mtbPort.Text = settings.Port.ToString();
        }
    }
}
