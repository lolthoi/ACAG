using System;
using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Client.Common;

namespace ACAG.Abacus.CalendarConnector.Client.Services
{
  public class ThemeAppService : IObservable<string>
  {
    private readonly IAuthService _authService;
    public ThemeAppService(IAuthService authService)
    {
      _authService = authService;
    }
    string currentUrl = "/css/switcher-resources/themes/default/bootstrap.min.css";
    List<IObserver<string>> observers = new List<IObserver<string>>();

    public void SetTheme(string newUrl)
    {
      if (currentUrl != newUrl)
      {
        currentUrl = newUrl;
        observers.ForEach(o => o.OnNext(currentUrl));
      }
    }
    public IDisposable Subscribe(IObserver<string> observer)
    {
      if (!observers.Contains(observer))
        observers.Add(observer);

      var user = _authService.GetCurrentUserAsync();
      AppDefiner.AppThemes.TryGetValue(user.Result.Key, out string urlUserTheme);
      if (urlUserTheme != currentUrl)
      {
        currentUrl = urlUserTheme;
      }

      observer.OnNext(currentUrl);
      return new Unsubscriber(observers, observer);
    }

    class Unsubscriber : IDisposable
    {
      private List<IObserver<string>> _observers;
      private IObserver<string> _observer;

      public Unsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
      {
        this._observers = observers;
        this._observer = observer;
      }

      public void Dispose()
      {
        if (_observer != null && _observers.Contains(_observer))
          _observers.Remove(_observer);
      }
    }
  }
}
