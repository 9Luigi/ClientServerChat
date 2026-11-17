using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ChatShared.Protocol;
using WFChat.UI;

namespace WFChatClient
{
    public partial class MainForm : Form
    {
        static readonly Size ChatSize = new Size(820, 560);

        ChatClient _chat; //null while the login screen is shown
        string _userName;
        readonly Size _loginSize;

        public MainForm()
        {
            InitializeComponent();
            ApplyTheme();
            tbSend.MaxLength = SendText.MaxLength; //the server cuts longer texts anyway
            _loginSize = this.ClientSize;
            ShowServerAddress();
        }

        void ApplyTheme()
        {
            Theme.Apply(this);
            Theme.StyleHeader(pHeader, lTitle, lSubtitle);
            Theme.StyleCard(pLoginCard);
            pLogin.Interval = 65; //the rain behind the login card falls a little slower than on the splash screen
            Theme.StylePrimary(bLogin);
            Theme.StylePrimary(bSend);
            Theme.StyleMuted(lServer);
            Theme.StyleMuted(lSymbolLeftCount);
            Theme.StyleTextFrame(pReceiveFrame, rtbReceive);
            rtbReceive.Font = Theme.ChatFont;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _chat?.Dispose();
            base.OnFormClosed(e);
        }

        static ClientSettings LoadSettings()
        {
            return ClientSettings.Load(ClientSettings.DefaultPath) ?? ClientSettings.CreateDefault();
        }

        void ShowServerAddress()
        {
            ClientSettings settings = LoadSettings();
            lServer.Text = $"Server: {settings.IpAddress}:{settings.Port}";
        }

        internal async void bLogin_Click(object sender, EventArgs e)
        {
            ClientSettings settings = LoadSettings();
            var chat = new ChatClient();
            chat.MessageReceived += Chat_MessageReceived;
            chat.Disconnected += Chat_Disconnected;

            bLogin.Enabled = false;
            bLogin.Text = "Connecting...";
            try
            {
                LoginResult result = await chat.ConnectAsync(IPAddress.Parse(settings.IpAddress), settings.Port, tbUsername.Text, tbPassword.Text);
                if (!result.Success)
                {
                    MessageBox.Show(LoginErrorText(result.Error), "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _chat = chat;
                _userName = tbUsername.Text;
                ShowChat(settings);
                ChatView.AppendNotice(rtbReceive, $"Welcome {_userName}!");
                chat.StartReceiving(); //after the chat view is ready, so no message lands on the login screen
            }
            catch (Exception ex) when (ex is SocketException || ex is IOException || ex is InvalidDataException || ex is OperationCanceledException)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Cannot communicate with server, try again later please", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bLogin.Enabled = true;
                bLogin.Text = "Log in";
            }
        }

        static string LoginErrorText(LoginError? error)
        {
            switch (error)
            {
                case LoginError.InvalidCredentials:
                    return "Incorrect user name or password, try again";
                case LoginError.AlreadyLoggedIn:
                    return "This user is already in the chat";
                default:
                    return "Server error, try again later please";
            }
        }

        private async void bSend_Click(object sender, EventArgs e)
        {
            string message = tbSend.Text;
            if (_chat == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            try
            {
                await _chat.SendTextAsync(message); //the server sends it back to everyone, including us
                tbSend.Text = ""; //only after a successful send, so a failed message isn't lost
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException || ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                Debug.WriteLine(ex); //a lost connection is reported by Chat_Disconnected
            }
        }

        #region Chat events (raised on thread-pool threads)
        void Chat_MessageReceived(ChatMessage message)
        {
            RunOnUiThread(() => ChatView.AppendMessage(rtbReceive, message, _userName));
        }

        void Chat_Disconnected(Exception error)
        {
            if (error != null)
            {
                Debug.WriteLine(error);
            }
            RunOnUiThread(() =>
            {
                _chat = null;
                ShowLogin();
                MessageBox.Show("Connection to the server was lost", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
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

        private void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            //Enter sends through AcceptButton (bSend) while the chat is shown
            if (e.Control && e.KeyCode == Keys.Back)
            {
                e.SuppressKeyPress = true; //otherwise TextBox inserts a square symbol instead of deleting
                DeleteWordBeforeCaret(tbSend);
            }
        }

        /// <summary>Ctrl+Backspace: deletes the selection, or the word before the caret with the spaces after it.</summary>
        static void DeleteWordBeforeCaret(TextBox box)
        {
            if (box.SelectionLength == 0)
            {
                int end = box.SelectionStart;
                int start = end;
                while (start > 0 && char.IsWhiteSpace(box.Text[start - 1]))
                {
                    start--;
                }
                while (start > 0 && !char.IsWhiteSpace(box.Text[start - 1]))
                {
                    start--;
                }
                box.Select(start, end - start);
            }
            box.SelectedText = "";
        }

        private void rtbReceive_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = tbSend;
        }

        private void tbSend_TextChanged(object sender, EventArgs e)
        {
            lSymbolLeftCount.Text = (tbSend.MaxLength - tbSend.TextLength).ToString() + " symbols left";
        }

        private void bChangeServer_Click(object sender, EventArgs e)
        {
            using (SettingsForm fsettings = new SettingsForm())
            {
                fsettings.ShowDialog(this);
            }
            ShowServerAddress();
        }

        void ShowChat(ClientSettings settings)
        {
            pLogin.Visible = false;
            pChat.Visible = true;
            lSubtitle.Text = $"{_userName}  ·  {settings.IpAddress}:{settings.Port}";
            this.AcceptButton = bSend;
            this.ClientSize = new Size(Math.Max(ClientSize.Width, ChatSize.Width), Math.Max(ClientSize.Height, ChatSize.Height));
            this.ActiveControl = tbSend;
        }

        void ShowLogin()
        {
            pChat.Visible = false;
            pLogin.Visible = true;
            rtbReceive.Clear();
            tbSend.Clear();
            lSubtitle.Text = "";
            _userName = null;
            this.AcceptButton = bLogin;
            this.ClientSize = _loginSize;
            this.ActiveControl = tbUsername;
        }
    }
}
