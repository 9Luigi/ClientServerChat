using System;
using System.IO;
using System.Windows.Forms;
using WFChat.UI;

namespace WFChatClient
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
            if (!int.TryParse(mtbPort.Text, out int port) || !ClientSettings.IsValid(mtbIPAddress.Text, port))
            {
                MessageBox.Show("Incorrect port or ip address");
                return;
            }
            try
            {
                new ClientSettings { IpAddress = mtbIPAddress.Text, Port = port }.Save(ClientSettings.FileName);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Cannot save settings: " + ex.Message);
                return;
            }
            MessageBox.Show("Succesful");
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ClientSettings settings = ClientSettings.Load(ClientSettings.FileName);
            if (settings == null)
            {
                settings = ClientSettings.CreateDefault();
                MessageBox.Show("Cannot load settings, default values were set");
            }
            mtbIPAddress.Text = settings.IpAddress;
            mtbPort.Text = settings.Port.ToString();
        }
    }
}
