using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models.Common.Toast;

namespace ACAG.Abacus.CalendarConnector.Client.Services
{
  public interface IToastAppService
  {
    event Action<ToastLevel, string, string> OnShow;
    void ShowInfo(string message, string heading = "");
    void ShowSuccess(string message, string heading = "");
    void ShowWarning(string message, string heading = "");
    void ShowError(string message, string heading = "");
    void ShowToast(ToastLevel level,string message,string heading = "");
  }

  public class ToastAppService : IToastAppService
  {
    public event Action<ToastLevel, string, string> OnShow;

    public ToastAppService()
    {

    }

    public void ShowError(string message, string heading = "")
    {
      ShowToast(ToastLevel.Error, message, heading);
    }

    public void ShowInfo(string message, string heading = "")
    {
      ShowToast(ToastLevel.Info, message, heading);
    }

    public void ShowSuccess(string message, string heading = "")
    {
      ShowToast(ToastLevel.Success, message, heading);
    }

    public void ShowWarning(string message, string heading = "")
    {
      ShowToast(ToastLevel.Warning, message, heading);
    }
    public void ShowToast(ToastLevel level, string message, string heading = "")
    {
      OnShow.Invoke(level, message, heading);
    }
  }
}
