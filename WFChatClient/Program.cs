using System;
using System.Windows.Forms;
using WFChat.UI;

namespace WFChatClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetColorMode(SystemColorMode.Dark); //dark scroll bars and title bars to match the Matrix theme
            using (var splash = new SplashForm("WFCHAT // CLIENT", "Wake up, Neo...", "The Matrix has you...", "Follow the white rabbit.", "Knock, knock."))
            {
                splash.ShowDialog();
            }
            Application.Run(new MainForm());
        }

    }
}
