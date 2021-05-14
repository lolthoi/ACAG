using System;
using ACAG.Abacus.CalendarConnector.Models.Common.Toast;
using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Shared.Toast
{
  public partial class AbacusToast
  {
    [CascadingParameter]
    private AbacusToasts ToastContainer { get; set; }
    [Parameter]
    public Guid ToastId { get; set; }
    [Parameter]
    public ToastSettings ToastSettings { get; set; }

    private void Close()
    {
      ToastContainer.RemoveToast(ToastId);
    }
  }
}
