﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text;

namespace WFChatServer
{
    public partial class fMain : Form
    {
        public fMain()
        {
            Program.fMainReferense = this; //reference for access controls from another classes in any.cs
            InitializeComponent();
        }
        internal static ServerObject server;
        static ControllerObject controller;
        static Thread listenThread;
        StreamWriter logWriter;//Log file
        private void fMain_Load(object sender, EventArgs e)
        {

            try
            {
                controller = new ControllerObject();
                server = new ServerObject();

                listenThread = new Thread(new ThreadStart(server.Listen)); //thread for listen
                listenThread.IsBackground = true; //thread will close after close main(entry point?) thread

                listenThread.Start();
            }
            catch (Exception ex)
            {
                tbController.Text = ex.Message + ", click any button to continue" + Environment.NewLine;
                server.Disconnect();
            }
            string path = Environment.CurrentDirectory + "\\logs";
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\logs");
            }
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            controller.Controller();
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
            logWriter = new StreamWriter(logFilePath, true, Encoding.Unicode);
            tbChatObs.SelectionStart = tbChatObs.Text.Length;
            tbChatObs.ScrollToCaret();
            logWriter.WriteLine(tbChatObs.Lines[tbChatObs.Lines.Length - 2]);//-2 cause of NewLine
            logWriter.Close();
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
            fServerSettings fsettings = new fServerSettings();
            fsettings.Show();
        }
    }
}