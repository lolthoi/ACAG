using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public delegate void StateChange(MessageType type, string message);
  public partial class InstallingForm : Form
  {
    public InstallingForm(AppSettings appSettings)
    {
      _appSettings = appSettings;
      InitializeComponent();
    }
    
    private Status _status = Status.INSTALLING;
    private readonly AppSettings _appSettings;

    private void InstallingForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      switch (_status)
      {
        case Status.INSTALLING: e.Cancel = true; break;
        case Status.FAIL: break;
        case Status.DONE: Environment.Exit(0); break;
      }
    }

    private void InstallingForm_Load(object sender, EventArgs e)
    {
      Icon = Owner.Icon;
      Task.Factory.StartNew(() =>
      {
        SetEnableClose(false);

        var install = Install();
        if (install)
        {
          _status = Status.DONE;
        }
        else
        {
          _status = Status.FAIL;
        }
        SetEnableClose(true);
      });
    }

    private void MessageLog(MessageType type, string message)
    {
      var color = type.GetColor();
      rtbMessage.AppendText(color, message);
    }

    private bool Install()
    {
      var executeFilePath = Application.ExecutablePath;
      var executableFileInfo = new FileInfo(executeFilePath);
      Utils.CurrentFolder = executableFileInfo.DirectoryName;
      
      MessageLog(MessageType.TITLE, "START VERIFYING...");
      var check = new VerifyProvider(_appSettings);
      check.OnStateChange += MessageLog;

      bool isDbExist;
      SetProgress(10);
      if (!check.Verify(out isDbExist))
      {
        MessageLog(MessageType.ERROR, "STOP");
        SetProgress(100);
        return false;
      }
      MessageLog(MessageType.TITLE, "COMPLETE VERIFYING");
      SetProgress(20);

      MessageLog(MessageType.TITLE, Environment.NewLine + "START EXTRACTING PACKAGE...");
      SetProgress(5);            
      if (!Utils.IsPackageExtracted())
      {
        MessageLog(MessageType.ERROR, "Could not extract package");
        MessageLog(MessageType.ERROR, "STOP");
        Utils.DeleteTempFolder();
        SetProgress(100);
        return false;
      }
      MessageLog(MessageType.TITLE, "COMPLETE EXTRACTING PACKAGE");
      SetProgress(5);

      MessageLog(MessageType.TITLE, Environment.NewLine + "START UPDATING CONNECTION STRING...");
      SetProgress(5);
      if (!Utils.WriteAppSettings(_appSettings))
      {
        MessageLog(MessageType.ERROR, "Could not update connection string of the appsetting.json");
        MessageLog(MessageType.ERROR, "STOP");
        Utils.DeleteTempFolder();
        SetProgress(100);
        return false;
      }
      MessageLog(MessageType.TITLE, "COMPLETE UPDATING CONNECTION STRING");
      SetProgress(5);

      MessageLog(MessageType.TITLE, Environment.NewLine + "START INSTALLING SQL....");
      var sql = new SQLProvider(_appSettings);
      sql.OnStateChange += MessageLog;
      if (!sql.Install())
      {
        MessageLog(MessageType.ERROR, "STOP");
        SetProgress(100);
        return false;
      }
      MessageLog(MessageType.TITLE, "COMPLETE INSTALLING SQL");
      SetProgress(40);

      MessageLog(MessageType.TITLE, Environment.NewLine + "START INSTALLING  SITE APP (IIS)....");
      var iis = new IISProvicer(_appSettings);
      iis.OnStateChange += MessageLog;

      if (!iis.Install())
      {
        MessageLog(MessageType.ERROR, "STOP");
        SetProgress(100);
        return false;
      }
      MessageLog(MessageType.TITLE, "COMPLETE INSTALLING SITE APP");

      MessageLog(MessageType.TITLE, Environment.NewLine + "DONE");
      SetProgress(100);
      return true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      switch (_status)
      {
        case Status.INSTALLING: return;
        case Status.FAIL: Close(); break;
        case Status.DONE: Environment.Exit(0); break;
      }
    }

    private void SetProgress(int value)
    {
      this.InvokeControl(() =>
      {
        progressBar1.Value = value;
      });
    }

    private void SetEnableClose(bool enabled)
    {
      this.InvokeControl(() =>
      {
        btnClose.Enabled = enabled;
      });
    }

    public enum Status
    {
      INSTALLING,
      FAIL,
      DONE
    }
  }
}
