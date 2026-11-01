using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using ChatShared;
using ChatShared.Protocol;
using WFChat.UI;

namespace WFChatServer
{
    public partial class MainForm : Form
    {
        readonly ControllerObject _controller = new ControllerObject();
        readonly int _consoleWidth;
        int _hiddenChatWidth;
        ChatServer _server;

        public MainForm()
        {
            InitializeComponent();
            ApplyTheme();
            _consoleWidth = pConsole.Width;
        }

        void ApplyTheme()
        {
            Theme.Apply(this);
            Theme.StyleHeader(pHeader, lTitle, lStatus);
            Theme.StylePrimary(bSend);
            Theme.StyleTextFrame(pChatFrame, rtbChatObs);
            Theme.StyleTextFrame(pConsoleFrame, tbController);
            rtbChatObs.Font = Theme.ChatFont;
        }

        static string LogDirectory => Path.Combine(Environment.CurrentDirectory, "logs");

        private void MainForm_Load(object sender, EventArgs e)
        {
            _controller.ReturnCommandEvent += ControllerFunction;
            Directory.CreateDirectory(LogDirectory);

            ServerSettings settings = ServerSettings.Load(ServerSettings.FileName);
            if (settings == null)
            {
                settings = ServerSettings.CreateDefault();
                AppendControllerLine($"Cannot load settings, default was set: {settings.IpAddress}:{settings.Port}");
            }

            _server = new ChatServer(new DBHandler());
            _server.MessageBroadcast += Server_MessageBroadcast;
            _server.Log += Server_Log;
            try
            {
                _server.Start(IPAddress.Parse(settings.IpAddress), settings.Port);
                lStatus.Text = "Listening on " + _server.LocalEndPoint;
                AppendChatNotice("server started on " + _server.LocalEndPoint);
            }
            catch (SocketException ex)
            {
                lStatus.Text = "Not running: check the settings";
                AppendControllerLine($"Cannot start the server on {settings.IpAddress}:{settings.Port}: {ex.Message}. Change the settings and restart.");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _server?.Stop();
            base.OnFormClosed(e);
        }

        #region Server events (raised on thread-pool threads)
        void Server_MessageBroadcast(ChatMessage message)
        {
            RunOnUiThread(() =>
            {
                ChatView.AppendMessage(rtbChatObs, message);
                WriteLog(ChatMessageText.Format(message));
            });
        }

        void Server_Log(string text)
        {
            RunOnUiThread(() => AppendControllerLine(text));
        }

        /// <summary>Controls may only be touched from the UI thread; BeginInvoke queues the action there.</summary>
        void RunOnUiThread(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }
            try
            {
                BeginInvoke(action);
            }
            catch (InvalidOperationException) //the form is closing
            {
            }
        }
        #endregion

        void AppendChatNotice(string text)
        {
            ChatView.AppendNotice(rtbChatObs, text);
            WriteLog(text);
        }

        /// <summary>Appends a line to today's log file.</summary>
        void WriteLog(string line)
        {
            if (line == null)
            {
                return;
            }
            string logFilePath = Path.Combine(LogDirectory, "log-" + DateTime.Now.ToString("yy-MM-dd") + ".txt");
            try
            {
                File.AppendAllText(logFilePath, line + Environment.NewLine, Encoding.Unicode);
            }
            catch (IOException ex)
            {
                AppendControllerLine("Cannot write the log: " + ex.Message);
            }
        }

        void AppendControllerLine(string text)
        {
            tbController.AppendText(text + Environment.NewLine);
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            _controller.Controller(tbSendCommand.Text);
            tbSendCommand.Text = "";
        }

        private void tbChatObsAndChatController_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = tbSendCommand; //workaround for avoid focus at tbChatObs
        }

        private void tbController_TextChanged(object sender, EventArgs e) //scroll to the end of the tbController
        {
            tbController.SelectionStart = tbController.Text.Length;
            tbController.ScrollToCaret();
        }

        private void bCloseOpenChatTB_Click(object sender, EventArgs e)
        {
            if (pChatArea.Visible)
            {
                _hiddenChatWidth = pChatArea.Width + pGap.Width;
                pChatArea.Visible = false;
                pGap.Visible = false;
                pConsole.Dock = DockStyle.Fill;
                this.Width -= _hiddenChatWidth;
                bCloseOpenChatTB.Text = "Show chat";
            }
            else
            {
                pConsole.Dock = DockStyle.Left;
                pConsole.Width = _consoleWidth;
                pGap.Visible = true;
                pChatArea.Visible = true;
                this.Width += _hiddenChatWidth;
                bCloseOpenChatTB.Text = "Hide chat";
            }
        }

        private void bSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm fsettings = new SettingsForm())
            {
                fsettings.ShowDialog(this);
            }
        }

        #region ChatController
        void ControllerFunction(CommandEventArgs e)
        {
            switch (e.Message)
            {
                case "help":
                    Help();
                    break;
                case "users":
                    ClientUserNamesPrint();
                    break;
                case "test":
                    MessageBox.Show("test");
                    break;
                case "disconnect":
                    Close();
                    break;
                default:
                    AppendControllerLine("Enter help to see possible commands list");
                    break;
            }
        }

        internal void Help()
        {
            AppendControllerLine(String.Format("{0}: {1}", "Exit: ", "Close the application"));
            AppendControllerLine(String.Format("{0}: {1}", "Users: ", "Print users list"));
        }

        internal void ClientUserNamesPrint()
        {
            var users = _server?.GetOnlineUsers();
            if (users == null || users.Count == 0)
            {
                AppendControllerLine("no users are connected");
                return;
            }
            foreach (string user in users)
            {
                AppendControllerLine(user);
            }
        }
        #endregion
    }
}
