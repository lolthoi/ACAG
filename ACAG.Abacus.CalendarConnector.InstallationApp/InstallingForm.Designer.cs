
namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  partial class InstallingForm
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 263);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(625, 23);
            this.panel2.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(550, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(544, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtbMessage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(625, 263);
            this.panel1.TabIndex = 3;
            // 
            // rtbMessage
            // 
            this.rtbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMessage.Location = new System.Drawing.Point(0, 0);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.Size = new System.Drawing.Size(625, 263);
            this.rtbMessage.TabIndex = 0;
            this.rtbMessage.Text = "";
            // 
            // InstallingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 286);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "InstallingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ACAG Abacus Calendar Connector Installing...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallingForm_FormClosing);
            this.Load += new System.EventHandler(this.InstallingForm_Load);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.RichTextBox rtbMessage;
    private System.Windows.Forms.Button btnClose;
  }
}