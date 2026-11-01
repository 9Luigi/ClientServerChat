using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ChatShared;
using ChatShared.Protocol;

namespace WFChatClient
{
    public partial class MainForm : Form
    {
        ChatClient _chat; //null while the login screen is shown
        readonly Size _loginSize;

        public MainForm()
        {
            InitializeComponent();
            _loginSize = this.Size;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _chat?.Dispose();
            base.OnFormClosed(e);
        }

        internal async void bLogin_Click(object sender, EventArgs e)
        {
            ClientSettings settings = ClientSettings.Load(ClientSettings.FileName) ?? ClientSettings.CreateDefault();
            var chat = new ChatClient();
            chat.MessageReceived += Chat_MessageReceived;
            chat.Disconnected += Chat_Disconnected;

            bLogin.Enabled = false;
            try
            {
                LoginResult result = await chat.ConnectAsync(IPAddress.Parse(settings.IpAddress), settings.Port, tbUsername.Text, tbPassword.Text);
                if (!result.Success)
                {
                    MessageBox.Show(LoginErrorText(result.Error));
                    return;
                }
                _chat = chat;
                CloseLoginAndOpenChat();
                tbReceive.Text = String.Format("Welcome {0}!", tbUsername.Text);
                chat.StartReceiving(); //after the chat view is ready, so no message lands on the login screen
            }
            catch (Exception ex) when (ex is SocketException || ex is IOException || ex is InvalidDataException || ex is OperationCanceledException)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Cannot communicate with server, try again later please");
            }
            finally
            {
                bLogin.Enabled = true;
            }
        }

        static string LoginErrorText(LoginError? error)
        {
            switch (error)
            {
                case LoginError.UserNotFound:
                    return "User is not found, try again";
                case LoginError.IncorrectPassword:
                    return "Incorrect password, try again";
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
            tbSend.Text = "";
            try
            {
                await _chat.SendTextAsync(message); //the server sends it back to everyone, including us
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException || ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                Debug.WriteLine(ex); //a lost connection is reported by Chat_Disconnected
            }
        }

        #region Chat events (raised on thread-pool threads)
        void Chat_MessageReceived(ChatMessage message)
        {
            string line = ChatMessageText.Format(message);
            if (line != null)
            {
                RunOnUiThread(() => tbReceive.AppendText(Environment.NewLine + line));
            }
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
                MessageBox.Show("Connection to the server was lost");
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

        private void tbReceive_TextChanged(object sender, EventArgs e)
        {
            tbReceive.SelectionStart = tbReceive.Text.Length;
            tbReceive.ScrollToCaret();
        }

        private void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = true; //avoid new line(key realese event(?))
                    bSend_Click(sender, e);
                    break;
                case Keys.Back:
                    e.SuppressKeyPress = true;
                    if (tbSend.SelectionStart > 0)
                    {
                        SendKeys.Send("+{LEFT}{DEL}"); //delete a word with ctrl+backspace, don't undestand how yet
                    }
                    break;
            }
        }

        private void tbReceive_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = tbSend;
        }

        private void tbSend_TextChanged(object sender, EventArgs e)
        {
            lSymbolLeftCount.Text = (tbSend.MaxLength - tbSend.TextLength).ToString() + " symbol left";
        }

        private void bChangeServer_Click(object sender, EventArgs e)
        {
            SettingsForm fsettings = new SettingsForm();
            fsettings.ShowDialog();
        }

        internal void CloseLoginAndOpenChat()
        {
            SetChatVisible(true);
            this.Size = new Size(816, 489);
            this.ActiveControl = tbSend;
        }

        void ShowLogin()
        {
            SetChatVisible(false);
            tbReceive.Clear();
            tbSend.Clear();
            this.Size = _loginSize;
            this.ActiveControl = tbUsername;
        }

        void SetChatVisible(bool chat)
        {
            bLogin.Visible = !chat;
            tbPassword.Visible = !chat;
            tbUsername.Visible = !chat;
            label1.Visible = !chat;
            label2.Visible = !chat;
            bChangeServer.Visible = !chat;
            tbReceive.Visible = chat;
            tbSend.Visible = chat;
            bSend.Visible = chat;
            lSymbolLeftCount.Visible = chat;
        }
    }
}
