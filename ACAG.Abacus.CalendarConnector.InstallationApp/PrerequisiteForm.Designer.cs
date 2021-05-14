
namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  partial class PrerequisiteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrerequisiteForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblIisVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblIisStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSqlVersion = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSqlStatus = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxSites = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblSiteAppVersion = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblSiteStatus = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblPackageAppVersion = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblPackageDbVersion = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblDbStatus = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblDbName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDbVersion = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblNetCoreVersion = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblNetCoreStatus = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnInstallNetCore = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lblUrlRewriteVersion = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblUrlRewriteStatus = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.btnInstallUrlRewrite = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblIisVersion);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblIisStatus);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(144, 82);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Web server (IIS)";
            // 
            // lblIisVersion
            // 
            this.lblIisVersion.AutoSize = true;
            this.lblIisVersion.Location = new System.Drawing.Point(65, 53);
            this.lblIisVersion.Name = "lblIisVersion";
            this.lblIisVersion.Size = new System.Drawing.Size(16, 15);
            this.lblIisVersion.TabIndex = 3;
            this.lblIisVersion.Text = "...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Version:";
            // 
            // lblIisStatus
            // 
            this.lblIisStatus.AutoSize = true;
            this.lblIisStatus.Location = new System.Drawing.Point(65, 29);
            this.lblIisStatus.Name = "lblIisStatus";
            this.lblIisStatus.Size = new System.Drawing.Size(16, 15);
            this.lblIisStatus.TabIndex = 1;
            this.lblIisStatus.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblSqlVersion);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.lblSqlStatus);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(474, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(145, 82);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SQL server (Local)";
            // 
            // lblSqlVersion
            // 
            this.lblSqlVersion.AutoSize = true;
            this.lblSqlVersion.Location = new System.Drawing.Point(60, 53);
            this.lblSqlVersion.Name = "lblSqlVersion";
            this.lblSqlVersion.Size = new System.Drawing.Size(16, 15);
            this.lblSqlVersion.TabIndex = 3;
            this.lblSqlVersion.Text = "...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Version:";
            // 
            // lblSqlStatus
            // 
            this.lblSqlStatus.AutoSize = true;
            this.lblSqlStatus.Location = new System.Drawing.Point(60, 29);
            this.lblSqlStatus.Name = "lblSqlStatus";
            this.lblSqlStatus.Size = new System.Drawing.Size(16, 15);
            this.lblSqlStatus.TabIndex = 1;
            this.lblSqlStatus.Text = "...";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "Status:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxSites);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.lblSiteAppVersion);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.lblSiteStatus);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(12, 166);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(316, 126);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Site application selection";
            // 
            // cbxSites
            // 
            this.cbxSites.DropDownWidth = 201;
            this.cbxSites.FormattingEnabled = true;
            this.cbxSites.Location = new System.Drawing.Point(92, 33);
            this.cbxSites.Name = "cbxSites";
            this.cbxSites.Size = new System.Drawing.Size(210, 23);
            this.cbxSites.TabIndex = 6;
            this.cbxSites.TextChanged += new System.EventHandler(this.cbxSites_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(62, 15);
            this.label12.TabIndex = 4;
            this.label12.Text = "Site name:";
            // 
            // lblSiteAppVersion
            // 
            this.lblSiteAppVersion.AutoSize = true;
            this.lblSiteAppVersion.Location = new System.Drawing.Point(91, 84);
            this.lblSiteAppVersion.Name = "lblSiteAppVersion";
            this.lblSiteAppVersion.Size = new System.Drawing.Size(16, 15);
            this.lblSiteAppVersion.TabIndex = 3;
            this.lblSiteAppVersion.Text = "...";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(37, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "Version:";
            // 
            // lblSiteStatus
            // 
            this.lblSiteStatus.AutoSize = true;
            this.lblSiteStatus.Location = new System.Drawing.Point(91, 60);
            this.lblSiteStatus.Name = "lblSiteStatus";
            this.lblSiteStatus.Size = new System.Drawing.Size(16, 15);
            this.lblSiteStatus.TabIndex = 1;
            this.lblSiteStatus.Text = "...";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(43, 60);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "Status:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblPackageAppVersion);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.lblPackageDbVersion);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Location = new System.Drawing.Point(12, 100);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(606, 60);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Package version";
            // 
            // lblPackageAppVersion
            // 
            this.lblPackageAppVersion.AutoSize = true;
            this.lblPackageAppVersion.Location = new System.Drawing.Point(123, 28);
            this.lblPackageAppVersion.Name = "lblPackageAppVersion";
            this.lblPackageAppVersion.Size = new System.Drawing.Size(16, 15);
            this.lblPackageAppVersion.TabIndex = 3;
            this.lblPackageAppVersion.Text = "...";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(21, 28);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(96, 15);
            this.label17.TabIndex = 2;
            this.label17.Text = "Web application:";
            // 
            // lblPackageDbVersion
            // 
            this.lblPackageDbVersion.AutoSize = true;
            this.lblPackageDbVersion.Location = new System.Drawing.Point(426, 28);
            this.lblPackageDbVersion.Name = "lblPackageDbVersion";
            this.lblPackageDbVersion.Size = new System.Drawing.Size(16, 15);
            this.lblPackageDbVersion.TabIndex = 1;
            this.lblPackageDbVersion.Text = "...";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(362, 28);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(58, 15);
            this.label19.TabIndex = 0;
            this.label19.Text = "Database:";
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(462, 338);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 7;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(543, 338);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblDbStatus);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.lblDbName);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.lblDbVersion);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(344, 166);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(275, 126);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Database information";
            // 
            // lblDbStatus
            // 
            this.lblDbStatus.AutoSize = true;
            this.lblDbStatus.Location = new System.Drawing.Point(94, 60);
            this.lblDbStatus.Name = "lblDbStatus";
            this.lblDbStatus.Size = new System.Drawing.Size(16, 15);
            this.lblDbStatus.TabIndex = 7;
            this.lblDbStatus.Text = "...";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "Connection:";
            // 
            // lblDbName
            // 
            this.lblDbName.AutoSize = true;
            this.lblDbName.Location = new System.Drawing.Point(94, 36);
            this.lblDbName.Name = "lblDbName";
            this.lblDbName.Size = new System.Drawing.Size(16, 15);
            this.lblDbName.TabIndex = 5;
            this.lblDbName.Text = "...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "DB name:";
            // 
            // lblDbVersion
            // 
            this.lblDbVersion.AutoSize = true;
            this.lblDbVersion.Location = new System.Drawing.Point(94, 84);
            this.lblDbVersion.Name = "lblDbVersion";
            this.lblDbVersion.Size = new System.Drawing.Size(16, 15);
            this.lblDbVersion.TabIndex = 3;
            this.lblDbVersion.Text = "...";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(40, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Version:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 307);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(607, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblNetCoreVersion);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.lblNetCoreStatus);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Location = new System.Drawing.Point(162, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(146, 82);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = ".NetCore Hosting";
            // 
            // lblNetCoreVersion
            // 
            this.lblNetCoreVersion.AutoSize = true;
            this.lblNetCoreVersion.Location = new System.Drawing.Point(60, 53);
            this.lblNetCoreVersion.Name = "lblNetCoreVersion";
            this.lblNetCoreVersion.Size = new System.Drawing.Size(16, 15);
            this.lblNetCoreVersion.TabIndex = 3;
            this.lblNetCoreVersion.Text = "...";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Version:";
            // 
            // lblNetCoreStatus
            // 
            this.lblNetCoreStatus.AutoSize = true;
            this.lblNetCoreStatus.Location = new System.Drawing.Point(60, 29);
            this.lblNetCoreStatus.Name = "lblNetCoreStatus";
            this.lblNetCoreStatus.Size = new System.Drawing.Size(16, 15);
            this.lblNetCoreStatus.TabIndex = 1;
            this.lblNetCoreStatus.Text = "...";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 29);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 15);
            this.label13.TabIndex = 0;
            this.label13.Text = "Status:";
            // 
            // btnInstallNetCore
            // 
            this.btnInstallNetCore.Enabled = false;
            this.btnInstallNetCore.Location = new System.Drawing.Point(12, 338);
            this.btnInstallNetCore.Name = "btnInstallNetCore";
            this.btnInstallNetCore.Size = new System.Drawing.Size(144, 23);
            this.btnInstallNetCore.TabIndex = 4;
            this.btnInstallNetCore.Text = "Install .NetCore Hosting";
            this.btnInstallNetCore.UseVisualStyleBackColor = true;
            this.btnInstallNetCore.Click += new System.EventHandler(this.btnInstallNetCore_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lblUrlRewriteVersion);
            this.groupBox7.Controls.Add(this.label14);
            this.groupBox7.Controls.Add(this.lblUrlRewriteStatus);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Location = new System.Drawing.Point(314, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(154, 82);
            this.groupBox7.TabIndex = 9;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Url Rewrite";
            // 
            // lblUrlRewriteVersion
            // 
            this.lblUrlRewriteVersion.AutoSize = true;
            this.lblUrlRewriteVersion.Location = new System.Drawing.Point(60, 53);
            this.lblUrlRewriteVersion.Name = "lblUrlRewriteVersion";
            this.lblUrlRewriteVersion.Size = new System.Drawing.Size(16, 15);
            this.lblUrlRewriteVersion.TabIndex = 3;
            this.lblUrlRewriteVersion.Text = "...";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 15);
            this.label14.TabIndex = 2;
            this.label14.Text = "Version:";
            // 
            // lblUrlRewriteStatus
            // 
            this.lblUrlRewriteStatus.AutoSize = true;
            this.lblUrlRewriteStatus.Location = new System.Drawing.Point(60, 29);
            this.lblUrlRewriteStatus.Name = "lblUrlRewriteStatus";
            this.lblUrlRewriteStatus.Size = new System.Drawing.Size(16, 15);
            this.lblUrlRewriteStatus.TabIndex = 1;
            this.lblUrlRewriteStatus.Text = "...";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(42, 15);
            this.label16.TabIndex = 0;
            this.label16.Text = "Status:";
            // 
            // btnInstallUrlRewrite
            // 
            this.btnInstallUrlRewrite.Enabled = false;
            this.btnInstallUrlRewrite.Location = new System.Drawing.Point(162, 338);
            this.btnInstallUrlRewrite.Name = "btnInstallUrlRewrite";
            this.btnInstallUrlRewrite.Size = new System.Drawing.Size(107, 23);
            this.btnInstallUrlRewrite.TabIndex = 5;
            this.btnInstallUrlRewrite.Text = "Install Url Rewrite";
            this.btnInstallUrlRewrite.UseVisualStyleBackColor = true;
            this.btnInstallUrlRewrite.Click += new System.EventHandler(this.btnInstallUrlRewrite_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Enabled = true;
            this.btnRefresh.Location = new System.Drawing.Point(275, 338);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(63, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // PrerequisiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 373);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnInstallUrlRewrite);
            this.Controls.Add(this.btnInstallNetCore);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrerequisiteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ACAG Abacus Calendar Connector Prerequisite [Version 2.0]";
            this.Load += new System.EventHandler(this.PrerequisiteForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label lblIisVersion;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label lblIisStatus;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label lblSqlVersion;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label lblSqlStatus;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label lblSiteAppVersion;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label lblSiteStatus;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.Label lblPackageAppVersion;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label lblPackageDbVersion;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.Label lblDbName;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lblDbVersion;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.GroupBox groupBox6;
    private System.Windows.Forms.Label lblNetCoreVersion;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label lblNetCoreStatus;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label lblDbStatus;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.ComboBox cbxSites;
    private System.Windows.Forms.GroupBox groupBox7;
    private System.Windows.Forms.Label lblUrlRewriteVersion;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label lblUrlRewriteStatus;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Button btnInstallNetCore;
    private System.Windows.Forms.Button btnInstallUrlRewrite;
    private System.Windows.Forms.Button btnRefresh;
  }
}