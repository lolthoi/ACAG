using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{

  public delegate void Notify(string message, bool isError);
  public class CommandExecuter
  {

    public event Notify Notify;

    public bool Execute(bool isInstalled, bool showWindow, bool isIgnoreError, CommandInfo command, Action action)
    {
      if (isInstalled)
      {
        action.Invoke();
        return true;
      }
      string str = null;
      try
      {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
        {
          CreateNoWindow = !showWindow,
          WindowStyle = ProcessWindowStyle.Hidden,
          RedirectStandardError = true,
          UseShellExecute = false,
          FileName = command.FileName,
          Arguments = command.Arguments
        };

        using (Process exeProcess = Process.Start(psi))
        {
          str = exeProcess.StandardError.ReadLine();
          var str1 = exeProcess.StandardError.ReadToEnd();
          if (str != null && action == null)
          {
            Notify(str, true);
            exeProcess.WaitForExit();
            return false;
          }
          else
          {
            exeProcess.WaitForExit();
            while(true)
            {
              Thread.Sleep(500);
              
              if (!IsExists(command))
                break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (!isIgnoreError)
        {
          Notify(ex.ToString(), true);
        }
          
        return false;
      }
      if (action != null)
        action.Invoke();

      return true;
    }
    private bool IsExists(CommandInfo command)
    {
      var exists = Process.GetProcesses().Any(t => t.ProcessName.Contains(command.Name));

      return exists;
    }
  }

  public class CommandInfo
  {
    public CommandInfo(bool isShowWindow, string fileName, string arguments, string name, string message)
    {
      IsShowWindow = isShowWindow;
      FileName = fileName;
      Arguments = arguments;
      Name = name;
      Message = message;
    }
    public bool IsShowWindow { get; set; }

    public string Name { get; set; }

    public string Message { get; set; }

    public string FileName { get; set; }

    public string Arguments { get; set; }
  }
}
