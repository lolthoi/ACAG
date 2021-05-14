using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      SingleInstanceApplication.Run(new PrerequisiteForm());
    }
  }

  public class SingleInstanceApplication : WindowsFormsApplicationBase
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    private SingleInstanceApplication()
    {
      IsSingleInstance = true;
      StartupNextInstance += OnStartupNextInstance;
      ShutdownStyle = ShutdownMode.AfterMainFormCloses;
    }

    /// <summary>
    /// Event handler when start another instance.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnStartupNextInstance(object sender, StartupNextInstanceEventArgs e)
    {
    }

    /// <summary>
    /// Starts an application message loop on the current thread and, optionally, makes a form visible.
    /// </summary>
    /// <param name="form"></param>
    public static void Run(Form form)
    {
      var myApp = new SingleInstanceApplication { MainForm = form };
      myApp.Run(Environment.GetCommandLineArgs());

    }
  }
}
