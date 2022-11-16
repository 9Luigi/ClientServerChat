
namespace WinFormClient
{
    partial class mainForm
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
            this.tbRecieve = new System.Windows.Forms.TextBox();
            this.bSend = new System.Windows.Forms.Button();
            this.tbSend = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.bLogin = new System.Windows.Forms.Button();
            this.lSymbolLeftCount = new System.Windows.Forms.Label();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bChangeServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbRecieve
            // 
            this.tbRecieve.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRecieve.Cursor = System.Windows.Forms.Cursors.No;
            this.tbRecieve.Location = new System.Drawing.Point(0, 0);
            this.tbRecieve.Multiline = true;
            this.tbRecieve.Name = "tbRecieve";
            this.tbRecieve.ReadOnly = true;
            this.tbRecieve.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbRecieve.Size = new System.Drawing.Size(252, 227);
            this.tbRecieve.TabIndex = 0;
            this.tbRecieve.Visible = false;
            this.tbRecieve.TextChanged += new System.EventHandler(this.tbRecieve_TextChanged);
            this.tbRecieve.Enter += new System.EventHandler(this.tbRecieve_Enter);
            // 
            // bSend
            // 
            this.bSend.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bSend.Location = new System.Drawing.Point(84, 337);
            this.bSend.Name = "bSend";
            this.bSend.Size = new System.Drawing.Size(85, 23);
            this.bSend.TabIndex = 1;
            this.bSend.Text = "Send message";
            this.bSend.UseVisualStyleBackColor = true;
            this.bSend.Visible = false;
            this.bSend.Click += new System.EventHandler(this.bSend_Click);
            // 
            // tbSend
            // 
            this.tbSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSend.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.tbSend.Location = new System.Drawing.Point(0, 233);
            this.tbSend.MaxLength = 250;
            this.tbSend.Multiline = true;
            this.tbSend.Name = "tbSend";
            this.tbSend.Size = new System.Drawing.Size(252, 16);
            this.tbSend.TabIndex = 0;
            this.tbSend.Visible = false;
            this.tbSend.TextChanged += new System.EventHandler(this.tbSend_TextChanged);
            this.tbSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSend_KeyDown);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Username";
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbPassword.Location = new System.Drawing.Point(46, 136);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(165, 20);
            this.tbPassword.TabIndex = 10;
            this.tbPassword.Text = "Roman112233";
            // 
            // tbUsername
            // 
            this.tbUsername.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbUsername.Location = new System.Drawing.Point(46, 97);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(165, 20);
            this.tbUsername.TabIndex = 9;
            this.tbUsername.Text = "Roman";
            // 
            // bLogin
            // 
            this.bLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bLogin.Location = new System.Drawing.Point(68, 162);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(121, 34);
            this.bLogin.TabIndex = 8;
            this.bLogin.Text = "Login";
            this.bLogin.UseVisualStyleBackColor = true;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // lSymbolLeftCount
            // 
            this.lSymbolLeftCount.AutoSize = true;
            this.lSymbolLeftCount.Location = new System.Drawing.Point(450, 424);
            this.lSymbolLeftCount.Name = "lSymbolLeftCount";
            this.lSymbolLeftCount.Size = new System.Drawing.Size(82, 13);
            this.lSymbolLeftCount.TabIndex = 13;
            this.lSymbolLeftCount.Text = "250 symbols left";
            this.lSymbolLeftCount.Visible = false;
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // bChangeServer
            // 
            this.bChangeServer.Location = new System.Drawing.Point(68, 202);
            this.bChangeServer.Name = "bChangeServer";
            this.bChangeServer.Size = new System.Drawing.Size(121, 32);
            this.bChangeServer.TabIndex = 14;
            this.bChangeServer.Text = "Change server";
            this.bChangeServer.UseVisualStyleBackColor = true;
            this.bChangeServer.Click += new System.EventHandler(this.bChangeServer_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 286);
            this.Controls.Add(this.bChangeServer);
            this.Controls.Add(this.lSymbolLeftCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbUsername);
            this.Controls.Add(this.bLogin);
            this.Controls.Add(this.bSend);
            this.Controls.Add(this.tbSend);
            this.Controls.Add(this.tbRecieve);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.Text = "Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bSend;
        public System.Windows.Forms.TextBox tbRecieve;
        public System.Windows.Forms.TextBox tbSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Button bLogin;
        private System.Windows.Forms.Label lSymbolLeftCount;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.Button bChangeServer;
    }
}

