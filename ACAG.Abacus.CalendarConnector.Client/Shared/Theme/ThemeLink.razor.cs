using System;
namespace ACAG.Abacus.CalendarConnector.Client.Shared.Theme
{
  public partial class ThemeLink
  {
    IDisposable _subscription;
    string _currentThemeLink;
    bool _initialized;

    protected override void OnInitialized()
    {
      base.OnInitialized();
      _subscription = ThemeLinkService.Subscribe(this);
      _initialized = true;
    }
    void IDisposable.Dispose()
    {
      using (_subscription) _subscription = null;
    }

    void IObserver<string>.OnNext(string newThemeLink)
    {
      _currentThemeLink = newThemeLink;
      if (_initialized)
        InvokeAsync(StateHasChanged);
    }
    void IObserver<string>.OnCompleted() { }
    void IObserver<string>.OnError(Exception error) { }
  }
}
