
namespace WinFormClient
{
    partial class fClientSettings
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
            this.bSaveSettings = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mtbIPAddress = new System.Windows.Forms.MaskedTextBox();
            this.mtbPort = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // bSaveSettings
            // 
            this.bSaveSettings.Location = new System.Drawing.Point(72, 64);
            this.bSaveSettings.Name = "bSaveSettings";
            this.bSaveSettings.Size = new System.Drawing.Size(100, 23);
            this.bSaveSettings.TabIndex = 2;
            this.bSaveSettings.Text = "Save settings";
            this.bSaveSettings.UseVisualStyleBackColor = true;
            this.bSaveSettings.Click += new System.EventHandler(this.bSaveSettings_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP-address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // mtbIPAddress
            // 
            this.mtbIPAddress.Location = new System.Drawing.Point(72, 15);
            this.mtbIPAddress.Name = "mtbIPAddress";
            this.mtbIPAddress.Size = new System.Drawing.Size(100, 20);
            this.mtbIPAddress.TabIndex = 5;
            // 
            // mtbPort
            // 
            this.mtbPort.Location = new System.Drawing.Point(72, 38);
            this.mtbPort.Name = "mtbPort";
            this.mtbPort.Size = new System.Drawing.Size(100, 20);
            this.mtbPort.TabIndex = 6;
            // 
            // fSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(191, 98);
            this.Controls.Add(this.mtbPort);
            this.Controls.Add(this.mtbIPAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bSaveSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "fSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.fSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bSaveSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.MaskedTextBox mtbIPAddress;
        public System.Windows.Forms.MaskedTextBox mtbPort;
    }
}