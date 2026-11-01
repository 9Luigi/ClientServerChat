
namespace WFChatClient
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
            this.lSubtitle = new System.Windows.Forms.Label();
            this.pLogin = new System.Windows.Forms.Panel();
            this.pLoginCard = new System.Windows.Forms.Panel();
            this.lUsername = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.lPassword = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.bLogin = new System.Windows.Forms.Button();
            this.bChangeServer = new System.Windows.Forms.Button();
            this.lServer = new System.Windows.Forms.Label();
            this.pChat = new System.Windows.Forms.Panel();
            this.pReceiveFrame = new System.Windows.Forms.Panel();
            this.rtbReceive = new System.Windows.Forms.RichTextBox();
            this.pInput = new System.Windows.Forms.Panel();
            this.tbSend = new System.Windows.Forms.TextBox();
            this.pSendGap = new System.Windows.Forms.Panel();
            this.bSend = new System.Windows.Forms.Button();
            this.lSymbolLeftCount = new System.Windows.Forms.Label();
            this.pHeader.SuspendLayout();
            this.pLogin.SuspendLayout();
            this.pLoginCard.SuspendLayout();
            this.pChat.SuspendLayout();
            this.pInput.SuspendLayout();
            this.pReceiveFrame.SuspendLayout();
            this.SuspendLayout();
            //
            // pHeader
            //
            this.pHeader.Controls.Add(this.lTitle);
            this.pHeader.Controls.Add(this.lSubtitle);
            this.pHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pHeader.Location = new System.Drawing.Point(0, 0);
            this.pHeader.Name = "pHeader";
            this.pHeader.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.pHeader.Size = new System.Drawing.Size(384, 56);
            this.pHeader.TabIndex = 0;
            //
            // lTitle
            //
            this.lTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F);
            this.lTitle.Location = new System.Drawing.Point(16, 0);
            this.lTitle.Name = "lTitle";
            this.lTitle.Size = new System.Drawing.Size(152, 56);
            this.lTitle.TabIndex = 0;
            this.lTitle.Text = "Chat";
            this.lTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lSubtitle
            //
            this.lSubtitle.Dock = System.Windows.Forms.DockStyle.Right;
            this.lSubtitle.Location = new System.Drawing.Point(168, 0);
            this.lSubtitle.Name = "lSubtitle";
            this.lSubtitle.Size = new System.Drawing.Size(200, 56);
            this.lSubtitle.TabIndex = 1;
            this.lSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pLogin
            //
            this.pLogin.Controls.Add(this.pLoginCard);
            this.pLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pLogin.Location = new System.Drawing.Point(0, 56);
            this.pLogin.Name = "pLogin";
            this.pLogin.Size = new System.Drawing.Size(384, 405);
            this.pLogin.TabIndex = 1;
            //
            // pLoginCard
            //
            this.pLoginCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pLoginCard.Controls.Add(this.lUsername);
            this.pLoginCard.Controls.Add(this.tbUsername);
            this.pLoginCard.Controls.Add(this.lPassword);
            this.pLoginCard.Controls.Add(this.tbPassword);
            this.pLoginCard.Controls.Add(this.bLogin);
            this.pLoginCard.Controls.Add(this.bChangeServer);
            this.pLoginCard.Controls.Add(this.lServer);
            this.pLoginCard.Location = new System.Drawing.Point(42, 46);
            this.pLoginCard.Name = "pLoginCard";
            this.pLoginCard.Size = new System.Drawing.Size(300, 312);
            this.pLoginCard.TabIndex = 0;
            //
            // lUsername
            //
            this.lUsername.AutoSize = true;
            this.lUsername.Location = new System.Drawing.Point(24, 24);
            this.lUsername.Name = "lUsername";
            this.lUsername.Size = new System.Drawing.Size(72, 17);
            this.lUsername.TabIndex = 0;
            this.lUsername.Text = "User name";
            //
            // tbUsername
            //
            this.tbUsername.Location = new System.Drawing.Point(24, 46);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(252, 25);
            this.tbUsername.TabIndex = 1;
            this.tbUsername.Text = "Roman";
            //
            // lPassword
            //
            this.lPassword.AutoSize = true;
            this.lPassword.Location = new System.Drawing.Point(24, 84);
            this.lPassword.Name = "lPassword";
            this.lPassword.Size = new System.Drawing.Size(64, 17);
            this.lPassword.TabIndex = 2;
            this.lPassword.Text = "Password";
            //
            // tbPassword
            //
            this.tbPassword.Location = new System.Drawing.Point(24, 106);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(252, 25);
            this.tbPassword.TabIndex = 3;
            this.tbPassword.Text = "Roman112233";
            this.tbPassword.UseSystemPasswordChar = true;
            //
            // bLogin
            //
            this.bLogin.Location = new System.Drawing.Point(24, 152);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(252, 38);
            this.bLogin.TabIndex = 4;
            this.bLogin.Text = "Log in";
            this.bLogin.UseVisualStyleBackColor = false;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            //
            // bChangeServer
            //
            this.bChangeServer.Location = new System.Drawing.Point(24, 200);
            this.bChangeServer.Name = "bChangeServer";
            this.bChangeServer.Size = new System.Drawing.Size(252, 34);
            this.bChangeServer.TabIndex = 5;
            this.bChangeServer.Text = "Server settings";
            this.bChangeServer.UseVisualStyleBackColor = false;
            this.bChangeServer.Click += new System.EventHandler(this.bChangeServer_Click);
            //
            // lServer
            //
            this.lServer.Location = new System.Drawing.Point(24, 248);
            this.lServer.Name = "lServer";
            this.lServer.Size = new System.Drawing.Size(252, 40);
            this.lServer.TabIndex = 6;
            this.lServer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            // pChat
            //
            this.pChat.Controls.Add(this.pReceiveFrame);
            this.pChat.Controls.Add(this.pInput);
            this.pChat.Controls.Add(this.lSymbolLeftCount);
            this.pChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pChat.Location = new System.Drawing.Point(0, 56);
            this.pChat.Name = "pChat";
            this.pChat.Padding = new System.Windows.Forms.Padding(12, 12, 12, 4);
            this.pChat.Size = new System.Drawing.Size(384, 405);
            this.pChat.TabIndex = 2;
            this.pChat.Visible = false;
            //
            // pReceiveFrame
            //
            this.pReceiveFrame.Controls.Add(this.rtbReceive);
            this.pReceiveFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pReceiveFrame.Location = new System.Drawing.Point(12, 12);
            this.pReceiveFrame.Name = "pReceiveFrame";
            this.pReceiveFrame.Size = new System.Drawing.Size(360, 297);
            this.pReceiveFrame.TabIndex = 1;
            //
            // rtbReceive
            //
            this.rtbReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbReceive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbReceive.Location = new System.Drawing.Point(0, 0);
            this.rtbReceive.Name = "rtbReceive";
            this.rtbReceive.ReadOnly = true;
            this.rtbReceive.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbReceive.Size = new System.Drawing.Size(360, 297);
            this.rtbReceive.TabIndex = 1;
            this.rtbReceive.TabStop = false;
            this.rtbReceive.Text = "";
            this.rtbReceive.Enter += new System.EventHandler(this.rtbReceive_Enter);
            //
            // pInput
            //
            this.pInput.Controls.Add(this.tbSend);
            this.pInput.Controls.Add(this.pSendGap);
            this.pInput.Controls.Add(this.bSend);
            this.pInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pInput.Location = new System.Drawing.Point(12, 309);
            this.pInput.Name = "pInput";
            this.pInput.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pInput.Size = new System.Drawing.Size(360, 72);
            this.pInput.TabIndex = 0;
            //
            // tbSend
            //
            this.tbSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSend.Location = new System.Drawing.Point(0, 10);
            this.tbSend.MaxLength = 250;
            this.tbSend.Multiline = true;
            this.tbSend.Name = "tbSend";
            this.tbSend.Size = new System.Drawing.Size(232, 62);
            this.tbSend.TabIndex = 0;
            this.tbSend.TextChanged += new System.EventHandler(this.tbSend_TextChanged);
            this.tbSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSend_KeyDown);
            //
            // pSendGap
            //
            this.pSendGap.Dock = System.Windows.Forms.DockStyle.Right;
            this.pSendGap.Location = new System.Drawing.Point(232, 10);
            this.pSendGap.Name = "pSendGap";
            this.pSendGap.Size = new System.Drawing.Size(8, 62);
            this.pSendGap.TabIndex = 1;
            //
            // bSend
            //
            this.bSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.bSend.Location = new System.Drawing.Point(240, 10);
            this.bSend.Name = "bSend";
            this.bSend.Size = new System.Drawing.Size(120, 62);
            this.bSend.TabIndex = 2;
            this.bSend.Text = "Send";
            this.bSend.UseVisualStyleBackColor = false;
            this.bSend.Click += new System.EventHandler(this.bSend_Click);
            //
            // lSymbolLeftCount
            //
            this.lSymbolLeftCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lSymbolLeftCount.Location = new System.Drawing.Point(12, 381);
            this.lSymbolLeftCount.Name = "lSymbolLeftCount";
            this.lSymbolLeftCount.Size = new System.Drawing.Size(360, 20);
            this.lSymbolLeftCount.TabIndex = 2;
            this.lSymbolLeftCount.Text = "250 symbols left";
            this.lSymbolLeftCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainForm
            //
            this.AcceptButton = this.bLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.pChat);
            this.Controls.Add(this.pLogin);
            this.Controls.Add(this.pHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat";
            this.pHeader.ResumeLayout(false);
            this.pLogin.ResumeLayout(false);
            this.pLoginCard.ResumeLayout(false);
            this.pLoginCard.PerformLayout();
            this.pChat.ResumeLayout(false);
            this.pInput.ResumeLayout(false);
            this.pInput.PerformLayout();
            this.pReceiveFrame.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pHeader;
        private System.Windows.Forms.Label lTitle;
        private System.Windows.Forms.Label lSubtitle;
        private System.Windows.Forms.Panel pLogin;
        private System.Windows.Forms.Panel pLoginCard;
        private System.Windows.Forms.Label lUsername;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label lPassword;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button bLogin;
        private System.Windows.Forms.Button bChangeServer;
        private System.Windows.Forms.Label lServer;
        private System.Windows.Forms.Panel pChat;
        private System.Windows.Forms.Panel pReceiveFrame;
        private System.Windows.Forms.RichTextBox rtbReceive;
        private System.Windows.Forms.Panel pInput;
        private System.Windows.Forms.TextBox tbSend;
        private System.Windows.Forms.Panel pSendGap;
        private System.Windows.Forms.Button bSend;
        private System.Windows.Forms.Label lSymbolLeftCount;
    }
}

