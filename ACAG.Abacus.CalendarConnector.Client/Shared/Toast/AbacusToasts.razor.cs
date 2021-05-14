using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ACAG.Abacus.CalendarConnector.Models.Common.Toast;
using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Shared.Toast
{
  public partial class AbacusToasts
  {
    [Parameter]
    public string InfoClass { get; set; }
    [Parameter]
    public string InfoIconClass { get; set; }
    [Parameter]
    public string SuccessClass { get; set; }
    [Parameter]
    public string SuccessIconClass { get; set; }
    [Parameter]
    public string WarningClass { get; set; }
    [Parameter]
    public string WarningIconClass { get; set; }
    [Parameter]
    public string ErrorClass { get; set; }
    [Parameter]
    public string ErrorIconClass { get; set; }
    [Parameter]
    public ToastPosition Position { get; set; } = ToastPosition.TopRight;
    [Parameter]
    public int Timeout { get; set; } = 5;

    private string PositionClass { get; set; } = string.Empty;

    public List<ToastInstance> ToastList { get; set; } = new List<ToastInstance>();

    protected override void OnInitialized()
    {
      base.OnInitialized();
      _toastService.OnShow += ShowToast;
      PositionClass = $"position-{Position.ToString().ToLower()}";
    }

    public void RemoveToast(Guid toastId)
    {
      InvokeAsync(() =>
      {
        var toastInstance = ToastList.SingleOrDefault(x => x.Id == toastId);
        ToastList.Remove(toastInstance);
        StateHasChanged();
      });
    }

    private ToastSettings BuildToastSettings(ToastLevel level, string message, string heading)
    {
      switch (level)
      {
        case ToastLevel.Info:
          return new ToastSettings(string.IsNullOrEmpty(heading) ? _localizer["INFO"] : heading, message, "blazored-toast-info", InfoClass, InfoIconClass);
        case ToastLevel.Success:
          return new ToastSettings(string.IsNullOrEmpty(heading) ? _localizer["SUCCESS"] : heading, message, "blazored-toast-success", SuccessClass, SuccessIconClass); ;
        case ToastLevel.Warning:
          return new ToastSettings(string.IsNullOrEmpty(heading) ? _localizer["WARNING"] : heading, message, "blazored-toast-warning", WarningClass, WarningIconClass); ;
        case ToastLevel.Error:
          return new ToastSettings(string.IsNullOrEmpty(heading) ? _localizer["ERROR"] : heading, message, "blazored-toast-error", ErrorClass, ErrorIconClass); ;
      }

      throw new InvalidOperationException();
    }

    private void ShowToast(ToastLevel level, string message, string heading)
    {
      InvokeAsync(() =>
      {
        var settings = BuildToastSettings(level, message, heading);
        Guid newGuid = Guid.NewGuid();
        var toast = new ToastInstance
        {
          Id = newGuid,
          TimeStamp = DateTime.Now,
          ToastSettings = settings
        };

        ToastList.Add(toast);
        var timeOut = Timeout * 1000;
        var toastTimer = new Timer(timeOut);
        toastTimer.Elapsed += (sender, args) => { RemoveToast(toast.Id); };
        toastTimer.AutoReset = false;
        toastTimer.Start();

        StateHasChanged();
      });
    }
  }
}
