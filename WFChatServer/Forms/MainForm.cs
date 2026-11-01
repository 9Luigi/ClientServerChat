using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text;

namespace WFChatServer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        internal static ServerObject Server;
        static ControllerObject _controller;
        static Thread _listenThread;
        StreamWriter _logWriter;//Log file
        private void MainForm_Load(object sender, EventArgs e)
        {

            try
            {
                _controller = new ControllerObject();
                Server = new ServerObject();
                Server.DataBaseHandler.ErrorEvent += ShowEventMessage;
                Server.ServerObjectToMainFormEvent += NotifyFromClientToMainForm;
                Server.TransferEvent += NotifyFromClientToMainForm;
                _controller.ReturnCommandEvent += ControllerFunction;

                _listenThread = new Thread(new ThreadStart(Server.Listen)); //thread for listen
                _listenThread.IsBackground = true; //thread will close after close main(entry point?) thread

                _listenThread.Start();
            }
            catch (Exception ex)
            {
                tbController.Text = ex.Message + ", click any button to continue" + Environment.NewLine;
                Server.Disconnect();
            }
            string path = Environment.CurrentDirectory + "\\logs";
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\logs");
            }
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

        private void tbChatObs_TextChanged(object sender, EventArgs e)
        {
            string logFilePath = Environment.CurrentDirectory + "\\logs\\log-" + DateTime.Now.ToString("yy-MM-dd") + ".txt";
            _logWriter = new StreamWriter(logFilePath, true, Encoding.Unicode);
            tbChatObs.SelectionStart = tbChatObs.Text.Length;
            tbChatObs.ScrollToCaret();
            _logWriter.WriteLine(tbChatObs.Lines[tbChatObs.Lines.Length - 2]);//-2 cause of NewLine
            _logWriter.Close();
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
        #region EventFunctions
        private void ShowEventMessage(string message)
        {
            this.Invoke(new Action(() => MessageBox.Show(message)));
        }



        //TODO bring the top functions (events) in line with notifySenderToMainForm(wich is below)
        void NotifyFromClientToMainForm(ClientObject sender, ClientObjectEventArgs e)
        {
            switch (e.Target)
            {
                case "tbChatObs":
                    this.Invoke(new Action(() => tbChatObs.Text += e.Message + Environment.NewLine));
                    break;
                case "tbController":
                    this.Invoke(new Action(() => tbController.Text += e.Message + Environment.NewLine));
                    break;
                default:
                    break;
            }
        }
        void NotifyFromClientToMainForm(ServerObjectToMainFormEventArgs e)
        {
            switch (e.Context)
            {
                case "tbChatObs":
                    this.Invoke(new Action(() => tbChatObs.Text += e.Message + Environment.NewLine));
                    break;
                case "tbController":
                    this.Invoke(new Action(() => tbController.Text += e.Message + Environment.NewLine));
                    break;
                default:
                    break;
            }
        }
        #endregion
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
                    Environment.Exit(0);
                    break;
                default:
                    tbController.Text += "Enter help to see possible commands list";
                    break;
            }
        }
        internal void Help()
        {
            this.Invoke(new Action(() => tbController.Text += String.Format("{0}: {1}", "Exit: ", "Close the application") + Environment.NewLine));
            this.Invoke(new Action(() => tbController.Text += String.Format("{0}: {1}", "Users: ", "Print users list") + Environment.NewLine));
        }
        internal void ClientUserNamesPrint()
        {
            try
            {

                if (Server.Clients.Count > 0)
                {
                    for (int i = 0; i < Server.Clients.Count; i++)
                    {
                        /* Program.fMainReferense.tbController.Text = fMain.server.clients[i].userName + Environment.NewLine;*/
                        this.Invoke(new Action(() => tbController.Text += Server.Clients[i].UserName + Environment.NewLine));
                    }
                }
                else
                {
                    /*Program.fMainReferense.tbController.Text += "is empty" + Environment.NewLine;*/
                    this.Invoke(new Action(() => tbController.Text += "no users are connected" + Environment.NewLine));
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}
