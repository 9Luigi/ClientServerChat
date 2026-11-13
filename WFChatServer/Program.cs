using System;
using System.Windows.Forms;
using WFChat.UI;

namespace WFChatServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetColorMode(SystemColorMode.Dark); //dark scroll bars and title bars to match the Matrix theme
            using (var splash = new SplashForm("WFCHAT // SERVER", "> booting construct...", "> loading operator console...", "> opening a port into the Matrix...", "Welcome to the real world."))
            {
                splash.ShowDialog();
            }
            Application.Run(new MainForm());
        }
    }
}
