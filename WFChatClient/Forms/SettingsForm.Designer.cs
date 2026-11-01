
namespace WFChatClient
{
    partial class SettingsForm
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
            this.lIpAddress = new System.Windows.Forms.Label();
            this.mtbIPAddress = new System.Windows.Forms.MaskedTextBox();
            this.lPort = new System.Windows.Forms.Label();
            this.mtbPort = new System.Windows.Forms.MaskedTextBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bSaveSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lIpAddress
            // 
            this.lIpAddress.AutoSize = true;
            this.lIpAddress.Location = new System.Drawing.Point(20, 20);
            this.lIpAddress.Name = "lIpAddress";
            this.lIpAddress.Size = new System.Drawing.Size(70, 17);
            this.lIpAddress.TabIndex = 4;
            this.lIpAddress.Text = "IP address";
            // 
            // mtbIPAddress
            // 
            this.mtbIPAddress.Location = new System.Drawing.Point(20, 42);
            this.mtbIPAddress.Name = "mtbIPAddress";
            this.mtbIPAddress.Size = new System.Drawing.Size(280, 25);
            this.mtbIPAddress.TabIndex = 0;
            // 
            // lPort
            // 
            this.lPort.AutoSize = true;
            this.lPort.Location = new System.Drawing.Point(20, 80);
            this.lPort.Name = "lPort";
            this.lPort.Size = new System.Drawing.Size(32, 17);
            this.lPort.TabIndex = 5;
            this.lPort.Text = "Port";
            // 
            // mtbPort
            // 
            this.mtbPort.Location = new System.Drawing.Point(20, 102);
            this.mtbPort.Name = "mtbPort";
            this.mtbPort.Size = new System.Drawing.Size(280, 25);
            this.mtbPort.TabIndex = 1;
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(80, 148);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(100, 34);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = false;
            // 
            // bSaveSettings
            // 
            this.bSaveSettings.Location = new System.Drawing.Point(190, 148);
            this.bSaveSettings.Name = "bSaveSettings";
            this.bSaveSettings.Size = new System.Drawing.Size(110, 34);
            this.bSaveSettings.TabIndex = 2;
            this.bSaveSettings.Text = "Save";
            this.bSaveSettings.UseVisualStyleBackColor = false;
            this.bSaveSettings.Click += new System.EventHandler(this.bSaveSettings_Click);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.bSaveSettings;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(320, 202);
            this.Controls.Add(this.lIpAddress);
            this.Controls.Add(this.mtbIPAddress);
            this.Controls.Add(this.lPort);
            this.Controls.Add(this.mtbPort);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bSaveSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server connection";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lIpAddress;
        private System.Windows.Forms.MaskedTextBox mtbIPAddress;
        private System.Windows.Forms.Label lPort;
        private System.Windows.Forms.MaskedTextBox mtbPort;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bSaveSettings;
    }
}
