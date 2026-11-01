using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using ChatShared;
using ChatShared.Protocol;

namespace WFChatServer
{
    public partial class MainForm : Form
    {
        readonly ControllerObject _controller = new ControllerObject();
        ChatServer _server;

        public MainForm()
        {
            InitializeComponent();
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
                AppendChatLine("server started on " + _server.LocalEndPoint);
            }
            catch (SocketException ex)
            {
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
            string line = ChatMessageText.Format(message);
            if (line != null)
            {
                RunOnUiThread(() => AppendChatLine(line));
            }
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

        /// <summary>Shows a line in the chat panel and appends it to today's log file.</summary>
        void AppendChatLine(string line)
        {
            tbChatObs.AppendText(line + Environment.NewLine);
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

        private void tbSendCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                bSend_Click(sender, e);
            }
        }

        private void tbChatObs_TextChanged(object sender, EventArgs e) //scroll to the end of the tbChatObs
        {
            tbChatObs.SelectionStart = tbChatObs.Text.Length;
            tbChatObs.ScrollToCaret();
        }

        private void tbController_TextChanged(object sender, EventArgs e) //scroll to the end of the tbController
        {
            tbController.SelectionStart = tbController.Text.Length;
            tbController.ScrollToCaret();
        }

        private void bCloseOpenChatTB_Click(object sender, EventArgs e)
        {

            if (bCloseOpenChatTB.Text == "Hide chat")
            {
                tbChatObs.Hide();
                bCloseOpenChatTB.Text = "Open chat";
                this.Size = new Size(this.Size.Width - tbChatObs.Size.Width,
                    this.Size.Height);
                tbSendCommand.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                pButtons.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                tbController.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;

            }
            else
            {
                tbSendCommand.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                pButtons.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                tbController.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
                tbChatObs.Show();
                bCloseOpenChatTB.Text = "Hide chat";
                this.Size = new Size(this.Size.Width + 562,
                    this.Size.Height);

            }
        }

        private void bSettings_Click(object sender, EventArgs e)
        {
            SettingsForm fsettings = new SettingsForm();
            fsettings.Show();
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
