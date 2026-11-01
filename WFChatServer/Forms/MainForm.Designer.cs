
namespace WFChatServer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pHeader = new System.Windows.Forms.Panel();
            this.lTitle = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.pBody = new System.Windows.Forms.Panel();
            this.pChatArea = new System.Windows.Forms.Panel();
            this.pChatFrame = new System.Windows.Forms.Panel();
            this.rtbChatObs = new System.Windows.Forms.RichTextBox();
            this.lChatCaption = new System.Windows.Forms.Label();
            this.pGap = new System.Windows.Forms.Panel();
            this.pConsole = new System.Windows.Forms.Panel();
            this.pConsoleFrame = new System.Windows.Forms.Panel();
            this.tbController = new System.Windows.Forms.TextBox();
            this.lConsoleCaption = new System.Windows.Forms.Label();
            this.pCommandGap = new System.Windows.Forms.Panel();
            this.tbSendCommand = new System.Windows.Forms.TextBox();
            this.pButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bSend = new System.Windows.Forms.Button();
            this.bCloseOpenChatTB = new System.Windows.Forms.Button();
            this.bSettings = new System.Windows.Forms.Button();
            this.pHeader.SuspendLayout();
            this.pBody.SuspendLayout();
            this.pChatArea.SuspendLayout();
            this.pConsole.SuspendLayout();
            this.pButtons.SuspendLayout();
            this.pChatFrame.SuspendLayout();
            this.pConsoleFrame.SuspendLayout();
            this.SuspendLayout();
            //
            // pHeader
            //
            this.pHeader.Controls.Add(this.lTitle);
            this.pHeader.Controls.Add(this.lStatus);
            this.pHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pHeader.Location = new System.Drawing.Point(0, 0);
            this.pHeader.Name = "pHeader";
            this.pHeader.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.pHeader.Size = new System.Drawing.Size(1000, 56);
            this.pHeader.TabIndex = 0;
            //
            // lTitle
            //
            this.lTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F);
            this.lTitle.Location = new System.Drawing.Point(16, 0);
            this.lTitle.Name = "lTitle";
            this.lTitle.Size = new System.Drawing.Size(568, 56);
            this.lTitle.TabIndex = 0;
            this.lTitle.Text = "Chat server";
            this.lTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lStatus
            //
            this.lStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.lStatus.Location = new System.Drawing.Point(584, 0);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(400, 56);
            this.lStatus.TabIndex = 1;
            this.lStatus.Text = "Starting...";
            this.lStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pBody
            //
            this.pBody.Controls.Add(this.pChatArea);
            this.pBody.Controls.Add(this.pGap);
            this.pBody.Controls.Add(this.pConsole);
            this.pBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBody.Location = new System.Drawing.Point(0, 56);
            this.pBody.Name = "pBody";
            this.pBody.Padding = new System.Windows.Forms.Padding(12);
            this.pBody.Size = new System.Drawing.Size(1000, 544);
            this.pBody.TabIndex = 1;
            //
            // pChatArea
            //
            this.pChatArea.Controls.Add(this.pChatFrame);
            this.pChatArea.Controls.Add(this.lChatCaption);
            this.pChatArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pChatArea.Location = new System.Drawing.Point(384, 12);
            this.pChatArea.Name = "pChatArea";
            this.pChatArea.Size = new System.Drawing.Size(604, 520);
            this.pChatArea.TabIndex = 2;
            //
            // pChatFrame
            //
            this.pChatFrame.Controls.Add(this.rtbChatObs);
            this.pChatFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pChatFrame.Location = new System.Drawing.Point(0, 26);
            this.pChatFrame.Name = "pChatFrame";
            this.pChatFrame.Size = new System.Drawing.Size(604, 494);
            this.pChatFrame.TabIndex = 1;
            //
            // rtbChatObs
            //
            this.rtbChatObs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChatObs.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbChatObs.Location = new System.Drawing.Point(0, 0);
            this.rtbChatObs.Name = "rtbChatObs";
            this.rtbChatObs.ReadOnly = true;
            this.rtbChatObs.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbChatObs.Size = new System.Drawing.Size(604, 494);
            this.rtbChatObs.TabIndex = 1;
            this.rtbChatObs.TabStop = false;
            this.rtbChatObs.Text = "";
            this.rtbChatObs.Enter += new System.EventHandler(this.tbChatObsAndChatController_Enter);
            //
            // lChatCaption
            //
            this.lChatCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lChatCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.lChatCaption.Location = new System.Drawing.Point(0, 0);
            this.lChatCaption.Name = "lChatCaption";
            this.lChatCaption.Size = new System.Drawing.Size(604, 26);
            this.lChatCaption.TabIndex = 0;
            this.lChatCaption.Text = "Chat";
            this.lChatCaption.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pGap
            //
            this.pGap.Dock = System.Windows.Forms.DockStyle.Left;
            this.pGap.Location = new System.Drawing.Point(372, 12);
            this.pGap.Name = "pGap";
            this.pGap.Size = new System.Drawing.Size(12, 520);
            this.pGap.TabIndex = 1;
            //
            // pConsole
            //
            this.pConsole.Controls.Add(this.pConsoleFrame);
            this.pConsole.Controls.Add(this.lConsoleCaption);
            this.pConsole.Controls.Add(this.pCommandGap);
            this.pConsole.Controls.Add(this.tbSendCommand);
            this.pConsole.Controls.Add(this.pButtons);
            this.pConsole.Dock = System.Windows.Forms.DockStyle.Left;
            this.pConsole.Location = new System.Drawing.Point(12, 12);
            this.pConsole.Name = "pConsole";
            this.pConsole.Size = new System.Drawing.Size(360, 520);
            this.pConsole.TabIndex = 0;
            //
            // pConsoleFrame
            //
            this.pConsoleFrame.Controls.Add(this.tbController);
            this.pConsoleFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pConsoleFrame.Location = new System.Drawing.Point(0, 26);
            this.pConsoleFrame.Name = "pConsoleFrame";
            this.pConsoleFrame.Size = new System.Drawing.Size(360, 400);
            this.pConsoleFrame.TabIndex = 1;
            //
            // tbController
            //
            this.tbController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbController.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.tbController.Location = new System.Drawing.Point(0, 0);
            this.tbController.Multiline = true;
            this.tbController.Name = "tbController";
            this.tbController.ReadOnly = true;
            this.tbController.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbController.Size = new System.Drawing.Size(360, 400);
            this.tbController.TabIndex = 1;
            this.tbController.TabStop = false;
            this.tbController.TextChanged += new System.EventHandler(this.tbController_TextChanged);
            this.tbController.Enter += new System.EventHandler(this.tbChatObsAndChatController_Enter);
            //
            // lConsoleCaption
            //
            this.lConsoleCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lConsoleCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.lConsoleCaption.Location = new System.Drawing.Point(0, 0);
            this.lConsoleCaption.Name = "lConsoleCaption";
            this.lConsoleCaption.Size = new System.Drawing.Size(360, 26);
            this.lConsoleCaption.TabIndex = 0;
            this.lConsoleCaption.Text = "Console";
            //
            // pCommandGap
            //
            this.pCommandGap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pCommandGap.Location = new System.Drawing.Point(0, 426);
            this.pCommandGap.Name = "pCommandGap";
            this.pCommandGap.Size = new System.Drawing.Size(360, 8);
            this.pCommandGap.TabIndex = 2;
            //
            // tbSendCommand
            //
            this.tbSendCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbSendCommand.Font = new System.Drawing.Font("Consolas", 10.5F);
            this.tbSendCommand.Location = new System.Drawing.Point(0, 434);
            this.tbSendCommand.Name = "tbSendCommand";
            this.tbSendCommand.PlaceholderText = "Command: help, users, exit";
            this.tbSendCommand.Size = new System.Drawing.Size(360, 24);
            this.tbSendCommand.TabIndex = 0;
            //
            // pButtons
            //
            this.pButtons.Controls.Add(this.bSend);
            this.pButtons.Controls.Add(this.bCloseOpenChatTB);
            this.pButtons.Controls.Add(this.bSettings);
            this.pButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pButtons.Location = new System.Drawing.Point(0, 476);
            this.pButtons.Name = "pButtons";
            this.pButtons.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.pButtons.Size = new System.Drawing.Size(360, 44);
            this.pButtons.TabIndex = 3;
            this.pButtons.WrapContents = false;
            //
            // bSend
            //
            this.bSend.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.bSend.Name = "bSend";
            this.bSend.Size = new System.Drawing.Size(100, 34);
            this.bSend.TabIndex = 0;
            this.bSend.Text = "Run";
            this.bSend.UseVisualStyleBackColor = false;
            this.bSend.Click += new System.EventHandler(this.bSend_Click);
            //
            // bCloseOpenChatTB
            //
            this.bCloseOpenChatTB.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.bCloseOpenChatTB.Name = "bCloseOpenChatTB";
            this.bCloseOpenChatTB.Size = new System.Drawing.Size(110, 34);
            this.bCloseOpenChatTB.TabIndex = 1;
            this.bCloseOpenChatTB.Text = "Hide chat";
            this.bCloseOpenChatTB.UseVisualStyleBackColor = false;
            this.bCloseOpenChatTB.Click += new System.EventHandler(this.bCloseOpenChatTB_Click);
            //
            // bSettings
            //
            this.bSettings.Margin = new System.Windows.Forms.Padding(0);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(110, 34);
            this.bSettings.TabIndex = 2;
            this.bSettings.Text = "Settings";
            this.bSettings.UseVisualStyleBackColor = false;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            //
            // MainForm
            //
            this.AcceptButton = this.bSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.pBody);
            this.Controls.Add(this.pHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimumSize = new System.Drawing.Size(420, 400);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat server";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.pHeader.ResumeLayout(false);
            this.pBody.ResumeLayout(false);
            this.pChatArea.ResumeLayout(false);
            this.pConsole.ResumeLayout(false);
            this.pConsole.PerformLayout();
            this.pButtons.ResumeLayout(false);
            this.pChatFrame.ResumeLayout(false);
            this.pConsoleFrame.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pHeader;
        private System.Windows.Forms.Label lTitle;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.Panel pBody;
        private System.Windows.Forms.Panel pChatArea;
        private System.Windows.Forms.Panel pChatFrame;
        private System.Windows.Forms.RichTextBox rtbChatObs;
        private System.Windows.Forms.Label lChatCaption;
        private System.Windows.Forms.Panel pGap;
        private System.Windows.Forms.Panel pConsole;
        private System.Windows.Forms.Panel pConsoleFrame;
        private System.Windows.Forms.TextBox tbController;
        private System.Windows.Forms.Label lConsoleCaption;
        private System.Windows.Forms.Panel pCommandGap;
        private System.Windows.Forms.TextBox tbSendCommand;
        private System.Windows.Forms.FlowLayoutPanel pButtons;
        private System.Windows.Forms.Button bSend;
        private System.Windows.Forms.Button bCloseOpenChatTB;
        private System.Windows.Forms.Button bSettings;
    }
}

