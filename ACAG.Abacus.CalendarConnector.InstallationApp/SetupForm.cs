using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.Administration;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public partial class SetupForm : Form
  {

    private readonly SiteInfo _site;

    private ConnectionInfo _connection = new ConnectionInfo();

    public SetupForm(SiteInfo site)
    {
      _site = site;

      InitializeComponent();
    }

    #region Events

    private void btnInstall_Click(object sender, EventArgs e)
    {
      var invalidControl = ValidateForm();
      if (invalidControl != null)
      {
        lblMessages.Visible = true;
        invalidControl.Focus();
        return;
      }
      lblMessages.Visible = false;

      var appSettings = BindControlToEntity();
      this.Hide();
      var installingForm = new InstallingForm(appSettings);
      installingForm.Owner = this;
      installingForm.ShowDialog(this);
      this.Show();

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      Environment.Exit(0);
    }

    private void btnPath_Click(object sender, EventArgs e)
    {
      using (var fbd = new FolderBrowserDialog())
      {
        if (!string.IsNullOrEmpty(txtPhysicalPath.Text))
          fbd.SelectedPath = txtPhysicalPath.Text;

        DialogResult result = fbd.ShowDialog();

        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
        {
          txtPhysicalPath.Text = fbd.SelectedPath;
        }
      }
    }

    #endregion

    private AppSettings BindControlToEntity()
    {
      var appSettings = new AppSettings
      {
        IISConfig = new IISConfig
        {
          WebSiteName = _site.SiteName,
          PhysicalPath = txtPhysicalPath.Text.Trim(),
          Type = cbxType.Text,
          HostName = txtSiteHostName.Text.Trim(),
          Port = int.TryParse(txtSitePort.Text.Trim(), out int iisPort) ? iisPort : 0
        },
        EmailSettings = new MailSettings
        {
          EmailSelector = cbxEmailType.Text.Trim(),
          Smtp = new SmtpEmail
          {
            SenderName = txtAgencyName.Text.Trim(),
            Mail = txtEmail.Text.Trim(),
            Password = txtPassword.Text.Trim(),
            Host = txtEmailHost.Text.Trim(),
            Port = int.TryParse(txtEmailPort.Text.Trim(), out int emailPort) ? emailPort : 0
          },
          SendGrid = new SendGridEmail
          {
            SenderName = txtSendGridSenderName.Text.Trim(),
            Account = txtSendGridAccount.Text.Trim(),
            Key = txtSendGridKey.Text.Trim(),
          }
        },
        SQLConfig = new SQLConfig
        {
          ConnectionString = txtConnectionString.Text.Trim(),
          Catalog = txtDbCatalog.Text.Trim(),
          Username = txtDbUsername.Text.Trim(),
          Password = txtDbPassword.Text.Trim(),
          AdminUsername = txtDbAdminUsername.Text.Trim(),
          AdminPassword = txtDbAdminPassword.Text.Trim()
        }
      };

      return appSettings;
    }

    private Control _firstInvalidControl = null;
    private Control FirstInvalidControl
    {
      get { return _firstInvalidControl; }
      set
      {
        if (_firstInvalidControl == null)
          _firstInvalidControl = value;

        var txtEx = value as IControlEx;
        if (txtEx != null)
        {
          txtEx.IsError = true;
        }
      }
    }

    private Control ValidateForm()
    {
      _firstInvalidControl = null;

      // site settings
      if (string.IsNullOrWhiteSpace(txtPhysicalPath.Text))
        FirstInvalidControl = txtPhysicalPath;
      if (string.IsNullOrWhiteSpace(cbxType.Text))
        FirstInvalidControl = cbxType;
      if (string.IsNullOrWhiteSpace(txtSitePort.Text))
        FirstInvalidControl = txtSitePort;

      // email settings
      var isSmtp = cbxEmailType.Text == "Smtp";

      if (isSmtp)
      {
        if (string.IsNullOrWhiteSpace(txtAgencyName.Text))
          FirstInvalidControl = txtAgencyName;
        if (string.IsNullOrWhiteSpace(txtEmail.Text))
          FirstInvalidControl = txtEmail;
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
          FirstInvalidControl = txtPassword;
        if (string.IsNullOrWhiteSpace(txtEmailHost.Text))
          FirstInvalidControl = txtEmailHost;
        if (string.IsNullOrWhiteSpace(txtEmailPort.Text) || !int.TryParse(txtEmailPort.Text.Trim(), out int port))
          FirstInvalidControl = txtEmailPort;
      }
      else
      {
        if (string.IsNullOrWhiteSpace(txtSendGridSenderName.Text))
          FirstInvalidControl = txtSendGridSenderName;
        if (string.IsNullOrWhiteSpace(txtSendGridAccount.Text))
          FirstInvalidControl = txtSendGridAccount;
        if (string.IsNullOrWhiteSpace(txtSendGridKey.Text))
          FirstInvalidControl = txtSendGridKey;
      }

      // DB settings
      var isDbType = cbxDbType.Text == "Instance";
      if (isDbType)
      {
        if (string.IsNullOrWhiteSpace(txtDbServerName.Text))
          FirstInvalidControl = txtDbServerName;
      }
      else
      {
        if (string.IsNullOrWhiteSpace(txtDbIP.Text))
          FirstInvalidControl = txtDbIP;
        if (string.IsNullOrWhiteSpace(txtDbPort.Text))
          FirstInvalidControl = txtDbPort;
      }
      if (string.IsNullOrWhiteSpace(txtDbCatalog.Text))
        FirstInvalidControl = txtDbCatalog;
      if (string.IsNullOrWhiteSpace(cbxDbSecurity.Text))
        FirstInvalidControl = cbxDbSecurity;
      if (string.IsNullOrWhiteSpace(txtDbUsername.Text))
        FirstInvalidControl = txtDbUsername;
      if (string.IsNullOrWhiteSpace(txtDbPassword.Text))
        FirstInvalidControl = txtDbPassword;

      if (string.IsNullOrWhiteSpace(txtDbAdminUsername.Text))
        FirstInvalidControl = txtDbAdminUsername;
      if (string.IsNullOrWhiteSpace(txtDbAdminPassword.Text))
        FirstInvalidControl = txtDbAdminPassword;

      if (string.IsNullOrWhiteSpace(txtConnectionString.Text))
        FirstInvalidControl = txtConnectionString;

      return FirstInvalidControl;
    }

    private void SetupForm_Load(object sender, EventArgs e)
    {
      Icon = Owner.Icon;
      BindSiteInfo(_site);

      BindEntityToControls(_site);
    }

    private void BindEntityToControls(SiteInfo site)
    {
      var emailSettings = site.MailSettings;

      if (emailSettings != null)
      {
        cbxEmailType.SelectedIndex = emailSettings.EmailSelector.Trim().ToUpper() == "SMTP" ? 0 : 1;
        if (emailSettings.Smtp != null)
        {
          txtAgencyName.Text = emailSettings.Smtp.SenderName;
          txtEmail.Text = emailSettings.Smtp.Mail;
          txtPassword.Text = emailSettings.Smtp.Password;
          txtEmailHost.Text = emailSettings.Smtp.Host;
          txtEmailPort.Text = emailSettings.Smtp.Port.ToString();
        }
        if (emailSettings.SendGrid != null)
        {
          txtSendGridSenderName.Text = emailSettings.SendGrid.SenderName;
          txtSendGridAccount.Text = emailSettings.SendGrid.Account;
          txtSendGridKey.Text = emailSettings.SendGrid.Key;
        }
      }
      else
      {
        cbxEmailType.SelectedIndex = 0;
      }
      if (site.DbInfo != null && site.DbInfo.Tag != null)
      {
        txtConnectionString.Text = site.DbInfo.Tag.ToString();
      }
      else
      {
        cbxDbType.SelectedIndex = 1;
      }
    }

    private void BindSiteInfo(SiteInfo site)
    {
      this.InvokeControl(() =>
      {
        lblSiteName.Text = site.SiteName;
        var isInstalled = SetSiteStatus(site);
        if (isInstalled)
        {
          txtPhysicalPath.Text = site.PhysicalPath;
          cbxType.Text = site.Type;
          txtSiteHostName.Text = site.HostName;
          txtSitePort.Text = site.Port;
        }
        else
        {

          txtPhysicalPath.Text = string.Empty;
          cbxType.Text = string.Empty;
          txtSiteHostName.Text = string.Empty;
          txtSitePort.Text = string.Empty;
        }
      });
    }

    private bool SetSiteStatus(SiteInfo site)
    {
      var isInstalled = site != null && site.State.HasValue;
      lblSiteStatus.Text = isInstalled ? site.State.Value.ToString() : "(New)";
      lblSiteStatus.ForeColor = isInstalled && (site.State == ObjectState.Started || site.State == ObjectState.Starting)
          ? Color.Blue
          : Color.Red;

      return isInstalled;
    }

    #region Other events

    private void btnBack_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void txtSitePort_TextChanged(object sender, EventArgs e)
    {
      NumberOnlyWhenValueChanged(txtSitePort);
    }

    private void txtEmailPort_TextChanged(object sender, EventArgs e)
    {
      NumberOnlyWhenValueChanged(txtEmailPort);
    }

    public static void NumberOnlyWhenValueChanged(UserControl control)
    {
      if (control.Text == string.Empty)
      {
        control.Tag = control.Text;
      }
      else if (int.TryParse(control.Text, out int value))
      {
        control.Text = value.ToString();
        control.Tag = control.Text;
      }
      else
      {
        control.Text = (string)control.Tag;
      }
    }

    #endregion

    private void cbxEmailType_SelectedIndexChanged(object sender, EventArgs e)
    {
      var isSmtp = cbxEmailType.Text == "Smtp";
      SetEmailTypeVisible(isSmtp);
    }

    private void SetEmailTypeVisible(bool isSmtp)
    {
      pnlSMTP.Visible = isSmtp;
      pnlSendGrid.Visible = !isSmtp;
    }

    private void cbxDbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      var isInstance = cbxDbType.Text == "Instance";
      SetDbTypeVisible(isInstance);

      BindControlToConnectionInfo(_connection);
      var connectionString = _connection.ToString();

      txtConnectionString.TextChanged -= txtConnectionString_TextChanged;
      txtConnectionString.Text = connectionString;
      txtConnectionString.TextChanged += txtConnectionString_TextChanged;
    }

    private void SetDbTypeVisible(bool isInstance)
    {
      lblDbServerName.Visible = isInstance;
      txtDbServerName.Visible = isInstance;

      lblDbIP.Visible = !isInstance;
      txtDbIP.Visible = !isInstance;
      lblDbPort.Visible = !isInstance;
      txtDbPort.Visible = !isInstance;
    }

    private void txtDbSplittedControl_TextChanged(object sender, EventArgs e)
    {
      if (sender == txtDbPort)
      {
        NumberOnlyWhenValueChanged(txtDbPort);
      }

      BindControlToConnectionInfo(_connection);
      var connectionString = _connection.ToString();

      txtConnectionString.TextChanged -= txtConnectionString_TextChanged;
      txtConnectionString.Text = connectionString;
      txtConnectionString.TextChanged += txtConnectionString_TextChanged;
    }

    private void BindControlToConnectionInfo(ConnectionInfo info)
    {
      info.Type = cbxDbType.Text;
      info.ServerName = txtDbServerName.Text.Trim();
      info.IP = txtDbIP.Text.Trim();
      info.Port = txtDbPort.Text.Trim();
      info.Catalog = txtDbCatalog.Text.Trim();
      info.Security = cbxDbSecurity.Text;
      info.Username = txtDbUsername.Text.Trim();
      info.Password = txtDbPassword.Text.Trim();
    }

    private void BindConnectionInfoToControl(ConnectionInfo info)
    {
      cbxDbType.Text = info.Type;
      var isInstance = info.Type == "Instance";
      SetDbTypeVisible(isInstance);

      if (isInstance)
      {
        txtDbServerName.Text = info.ServerName;
        txtDbServerName.CheckError();
      }
      else
      {
        txtDbIP.Text = info.IP;
        txtDbPort.Text = info.Port;

        txtDbIP.CheckError();
        txtDbPort.CheckError();
      }
      txtDbCatalog.Text = info.Catalog;
      txtDbCatalog.CheckError();

      if (info.Security == string.Empty)
      {
        cbxDbSecurity.SelectedIndex = -1;
      }
      else
      {
        cbxDbSecurity.Text = info.Security;
      }
      cbxDbSecurity.CheckError();

      txtDbUsername.Text = info.Username;
      txtDbPassword.Text = info.Password;

      txtDbUsername.CheckError();
      txtDbPassword.CheckError();
    }

    private void txtConnectionString_TextChanged(object sender, EventArgs e)
    {
      var connectionString = txtConnectionString.Text;
      var builder = new ConnectionBuilder(connectionString);
      _connection = builder.ToObject();
      if (_connection == null)
      {
        return;
      }

      RemoveDbEvent(true);

      BindConnectionInfoToControl(_connection);

      RemoveDbEvent(false);
    }

    private void RemoveDbEvent(bool isRemoved)
    {
      if (isRemoved)
      {
        cbxDbType.SelectedIndexChanged -= cbxDbType_SelectedIndexChanged;

        txtDbServerName.TextChanged -= txtDbSplittedControl_TextChanged;
        txtDbIP.TextChanged -= txtDbSplittedControl_TextChanged;
        txtDbPort.TextChanged -= txtDbSplittedControl_TextChanged;

        txtDbCatalog.TextChanged -= txtDbSplittedControl_TextChanged;
        cbxDbSecurity.SelectedIndexChanged -= txtDbSplittedControl_TextChanged;
        txtDbUsername.TextChanged -= txtDbSplittedControl_TextChanged;
        txtDbPassword.TextChanged -= txtDbSplittedControl_TextChanged;
      }
      else
      {
        cbxDbType.SelectedIndexChanged += cbxDbType_SelectedIndexChanged;

        txtDbServerName.TextChanged += txtDbSplittedControl_TextChanged;
        txtDbIP.TextChanged += txtDbSplittedControl_TextChanged;
        txtDbPort.TextChanged += txtDbSplittedControl_TextChanged;

        txtDbCatalog.TextChanged += txtDbSplittedControl_TextChanged;
        cbxDbSecurity.SelectedIndexChanged += txtDbSplittedControl_TextChanged;
        txtDbUsername.TextChanged += txtDbSplittedControl_TextChanged;
        txtDbPassword.TextChanged += txtDbSplittedControl_TextChanged;
      }
    }
  }
}
