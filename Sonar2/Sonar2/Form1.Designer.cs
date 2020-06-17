namespace Sonar2
{
    partial class Form1
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
            this.cmbServer = new System.Windows.Forms.ComboBox();
            this.txtHostName = new System.Windows.Forms.TextBox();
            this.btnLookup = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtxResponse = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmbServer
            // 
            this.cmbServer.FormattingEnabled = true;
            this.cmbServer.Items.AddRange(new object[] {
            "whois.internic.net"});
            this.cmbServer.Location = new System.Drawing.Point(58, 524);
            this.cmbServer.Name = "cmbServer";
            this.cmbServer.Size = new System.Drawing.Size(210, 31);
            this.cmbServer.TabIndex = 1;
            // 
            // txtHostName
            // 
            this.txtHostName.Location = new System.Drawing.Point(58, 65);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(210, 32);
            this.txtHostName.TabIndex = 2;
            // 
            // btnLookup
            // 
            this.btnLookup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLookup.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLookup.ForeColor = System.Drawing.Color.White;
            this.btnLookup.ImageIndex = 1;
            this.btnLookup.Location = new System.Drawing.Point(58, 112);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(210, 94);
            this.btnLookup.TabIndex = 3;
            this.btnLookup.Text = "Search";
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(54, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Enter URL:";
            // 
            // txtxResponse
            // 
            this.txtxResponse.Location = new System.Drawing.Point(363, 12);
            this.txtxResponse.Multiline = true;
            this.txtxResponse.Name = "txtxResponse";
            this.txtxResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtxResponse.Size = new System.Drawing.Size(436, 557);
            this.txtxResponse.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(811, 590);
            this.Controls.Add(this.txtxResponse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLookup);
            this.Controls.Add(this.txtHostName);
            this.Controls.Add(this.cmbServer);
            this.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(138)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Sonar 2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbServer;
        private System.Windows.Forms.TextBox txtHostName;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtxResponse;
    }
}

