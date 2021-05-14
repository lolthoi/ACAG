using System;
using System.Collections.Generic;
using System.Net.Http;
using ACAG.Abacus.CalendarConnector.Wrappers.Actions;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers
{
  public interface IApiWrapper
  {
    IHttpClientFactory _clientFactory { get; }

    ILocalStorageService _localStorage { get; }

    AbacusSettingAction AbacusSettings { get { return GetAction<AbacusSettingAction>(); } }

    AccountAction Accounts { get { return GetAction<AccountAction>(); } }

    AppSettingAction AppSettings { get { return GetAction<AppSettingAction>(); } }

    AuthenticationAction AuthenticationActions { get { return GetAction<AuthenticationAction>(); } }

    CultureAction Cultures { get { return GetAction<CultureAction>(); } }

    ExchangeSettingAction ExchangeSettings { get { return GetAction<ExchangeSettingAction>(); } }

    LogDiaryAction LogDiaries { get { return GetAction<LogDiaryAction>(); } }

    PayTypeAction PayTypes { get { return GetAction<PayTypeAction>(); } }

    RoleAction Roles { get { return GetAction<RoleAction>(); } }

    TenantAction Tenants { get { return GetAction<TenantAction>(); } }
    
    UserAction Users { get { return GetAction<UserAction>(); } }

    TAction GetAction<TAction>() where TAction : ActionApi;
  }

  public class ApiWrapper : IApiWrapper
  {
    public ILocalStorageService _localStorage { get; private set; }

    public IHttpClientFactory _clientFactory { get; private set; }

    private Dictionary<string, ActionApi> _actions = new Dictionary<string, ActionApi>();

    public ApiWrapper(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
    {
      _clientFactory = clientFactory;
      _localStorage = localStorage;
    }

    public AbacusSettingAction AbacusSettings { get { return GetAction<AbacusSettingAction>(); } }

    public AccountAction Accounts { get { return GetAction<AccountAction>(); } }

    public AppSettingAction AppSettings { get { return GetAction<AppSettingAction>(); } }

    public AuthenticationAction AuthenticationActions { get { return GetAction<AuthenticationAction>(); } }

    public CultureAction Cultures { get { return GetAction<CultureAction>(); } }

    public ExchangeSettingAction ExchangeSettings { get { return GetAction<ExchangeSettingAction>(); } }

    public LogDiaryAction LogDiaries { get { return GetAction<LogDiaryAction>(); } }

    public PayTypeAction PayTypes { get { return GetAction<PayTypeAction>(); } }

    public RoleAction Roles { get { return GetAction<RoleAction>(); } }

    public TenantAction Tenants { get { return GetAction<TenantAction>(); } }

    public UserAction Users { get { return GetAction<UserAction>(); } }

    public TAction GetAction<TAction>() where TAction : ActionApi
    {
      var key = typeof(TAction).FullName;

      if (_actions.TryGetValue(key, out ActionApi action))
      {
        return (TAction)action;
      }
      var tAction = (TAction)Activator.CreateInstance(typeof(TAction), _clientFactory, _localStorage);
      _actions[key] = tAction;

      return tAction;
    }
  }
}
