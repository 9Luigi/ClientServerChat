
namespace WFChatServer
{
    partial class FMain
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
            this.tbChatObs = new System.Windows.Forms.TextBox();
            this.tbController = new System.Windows.Forms.TextBox();
            this.pButtons = new System.Windows.Forms.Panel();
            this.bSettings = new System.Windows.Forms.Button();
            this.bCloseOpenChatTB = new System.Windows.Forms.Button();
            this.bSend = new System.Windows.Forms.Button();
            this.tbSendCommand = new System.Windows.Forms.TextBox();
            this.pButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbChatObs
            // 
            this.tbChatObs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbChatObs.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.tbChatObs.Location = new System.Drawing.Point(344, 12);
            this.tbChatObs.Multiline = true;
            this.tbChatObs.Name = "tbChatObs";
            this.tbChatObs.ReadOnly = true;
            this.tbChatObs.Size = new System.Drawing.Size(562, 426);
            this.tbChatObs.TabIndex = 0;
            this.tbChatObs.TabStop = false;
            this.tbChatObs.TextChanged += new System.EventHandler(this.tbChatObs_TextChanged);
            this.tbChatObs.Enter += new System.EventHandler(this.tbChatObsAndChatController_Enter);
            // 
            // tbController
            // 
            this.tbController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tbController.Location = new System.Drawing.Point(12, 12);
            this.tbController.Multiline = true;
            this.tbController.Name = "tbController";
            this.tbController.ReadOnly = true;
            this.tbController.Size = new System.Drawing.Size(326, 315);
            this.tbController.TabIndex = 1;
            this.tbController.TextChanged += new System.EventHandler(this.tbController_TextChanged);
            this.tbController.Enter += new System.EventHandler(this.tbChatObsAndChatController_Enter);
            // 
            // pButtons
            // 
            this.pButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pButtons.Controls.Add(this.bSettings);
            this.pButtons.Controls.Add(this.bCloseOpenChatTB);
            this.pButtons.Controls.Add(this.bSend);
            this.pButtons.Location = new System.Drawing.Point(12, 410);
            this.pButtons.Name = "pButtons";
            this.pButtons.Size = new System.Drawing.Size(327, 28);
            this.pButtons.TabIndex = 2;
            // 
            // bSettings
            // 
            this.bSettings.Location = new System.Drawing.Point(166, 3);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(97, 23);
            this.bSettings.TabIndex = 3;
            this.bSettings.Text = "Change server";
            this.bSettings.UseVisualStyleBackColor = true;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            // 
            // bCloseOpenChatTB
            // 
            this.bCloseOpenChatTB.Location = new System.Drawing.Point(85, 4);
            this.bCloseOpenChatTB.Name = "bCloseOpenChatTB";
            this.bCloseOpenChatTB.Size = new System.Drawing.Size(75, 23);
            this.bCloseOpenChatTB.TabIndex = 1;
            this.bCloseOpenChatTB.Text = "Hide chat";
            this.bCloseOpenChatTB.UseVisualStyleBackColor = true;
            this.bCloseOpenChatTB.Click += new System.EventHandler(this.bCloseOpenChatTB_Click);
            // 
            // bSend
            // 
            this.bSend.Location = new System.Drawing.Point(3, 3);
            this.bSend.Name = "bSend";
            this.bSend.Size = new System.Drawing.Size(75, 23);
            this.bSend.TabIndex = 0;
            this.bSend.Text = "Send";
            this.bSend.UseVisualStyleBackColor = true;
            this.bSend.Click += new System.EventHandler(this.bSend_Click);
            // 
            // tbSendCommand
            // 
            this.tbSendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbSendCommand.Location = new System.Drawing.Point(12, 333);
            this.tbSendCommand.Multiline = true;
            this.tbSendCommand.Name = "tbSendCommand";
            this.tbSendCommand.Size = new System.Drawing.Size(327, 71);
            this.tbSendCommand.TabIndex = 3;
            this.tbSendCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSendCommand_KeyDown);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(918, 450);
            this.Controls.Add(this.tbSendCommand);
            this.Controls.Add(this.pButtons);
            this.Controls.Add(this.tbController);
            this.Controls.Add(this.tbChatObs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "fMain";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.fMain_Load);
            this.pButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pButtons;
        private System.Windows.Forms.Button bSend;
        private System.Windows.Forms.Button bCloseOpenChatTB;
        private System.Windows.Forms.Button bSettings;
        public System.Windows.Forms.TextBox tbChatObs;
        public System.Windows.Forms.TextBox tbController;
        private System.Windows.Forms.TextBox tbSendCommand;
    }
}

