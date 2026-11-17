using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ChatShared;
using ChatShared.Protocol;
using WFChat.UI;

namespace WFChatServer
{
    public partial class MainForm : Form
    {
        readonly int _consoleWidth;
        int _hiddenChatWidth;
        ChatServer _server;
        LogLevel _consoleLevel = LogLevel.Info;

        public MainForm()
        {
            InitializeComponent();
            cbLogLevel.Items.AddRange(new object[] { "Critical only", "Errors", "All events", "All + chat" }); //item index = (int)LogLevel
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

        static string LogDirectory => Path.Combine(AppContext.BaseDirectory, "logs");

        private void MainForm_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(LogDirectory);

            ServerSettings settings = ServerSettings.Load(ServerSettings.DefaultPath);
            if (settings == null)
            {
                settings = ServerSettings.CreateDefault();
                AppendLog(LogLevel.Error, $"Cannot load settings, default was set: {settings.IpAddress}:{settings.Port}");
            }
            _consoleLevel = settings.ConsoleLogLevel;
            cbLogLevel.SelectedIndex = (int)_consoleLevel;
            cbLogLevel.SelectedIndexChanged += cbLogLevel_SelectedIndexChanged; //subscribed only now, so showing the saved level doesn't save it again

            _server = new ChatServer(new DBHandler());
            _server.MessageBroadcast += Server_MessageBroadcast;
            _server.Log += Server_Log;
            try
            {
                _server.Start(IPAddress.Parse(settings.IpAddress), settings.Port);
                lStatus.Text = "Listening on " + _server.LocalEndPoint;
                AppendChatNotice("server started on " + _server.LocalEndPoint);
                AppendLog(LogLevel.Info, "Server started on " + _server.LocalEndPoint);
            }
            catch (SocketException ex)
            {
                lStatus.Text = "Not running: check the settings";
                AppendLog(LogLevel.Critical, $"Cannot start the server on {settings.IpAddress}:{settings.Port}: {ex.Message}. Change the settings and restart.");
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
                if (message is ChatText text)
                {
                    AppendLog(LogLevel.Message, $"{text.From}: {text.Text}"); //joins and leaves come as Info through Server_Log
                }
            });
        }

        void Server_Log(LogLevel level, string text)
        {
            RunOnUiThread(() => AppendLog(level, text));
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
                File.AppendAllText(logFilePath, line + Environment.NewLine); //UTF-8; log files written before 2026-11-17 are UTF-16
            }
            catch (IOException ex)
            {
                AppendLog(LogLevel.Error, "Cannot write the log: " + ex.Message);
            }
        }

        /// <summary>A console line with the time and the level; skipped when less important than the level chosen above the console.</summary>
        void AppendLog(LogLevel level, string text)
        {
            if (level > _consoleLevel)
            {
                return;
            }
            AppendControllerLine($"{DateTime.Now:HH:mm:ss} {LevelTag(level)} {text}");
        }

        static string LevelTag(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    return "CRIT ";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Info:
                    return "INFO ";
                default:
                    return "MSG  ";
            }
        }

        /// <summary>Command answers (help, users) go here directly: they are shown whatever the level.</summary>
        void AppendControllerLine(string text)
        {
            tbController.AppendText(text + Environment.NewLine);
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            RunCommand(tbSendCommand.Text);
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

        /// <summary>Takes effect at once for new lines and is remembered in settings.xml.</summary>
        private void cbLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _consoleLevel = (LogLevel)cbLogLevel.SelectedIndex;
            try
            {
                //loaded again, not kept from startup: the address may have been changed in the settings window since then
                ServerSettings settings = ServerSettings.Load(ServerSettings.DefaultPath) ?? ServerSettings.CreateDefault();
                settings.ConsoleLogLevel = _consoleLevel;
                settings.Save(ServerSettings.DefaultPath);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                AppendLog(LogLevel.Error, "Cannot save the log level: " + ex.Message);
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
        /// <summary>Commands typed into tbSendCommand.</summary>
        void RunCommand(string command)
        {
            switch (command.Trim().ToLowerInvariant())
            {
                case "users":
                    ClientUserNamesPrint();
                    break;
                case "exit":
                    Close();
                    break;
                case "help":
                    Help();
                    break;
                default:
                    AppendControllerLine("Unknown command. Enter help to see the commands list");
                    break;
            }
        }

        void Help()
        {
            AppendControllerLine("users: print the users list");
            AppendControllerLine("exit: close the application");
            AppendControllerLine("help: this list");
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
