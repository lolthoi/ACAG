using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Toolbelt.Blazor;

namespace ACAG.Abacus.CalendarConnector.Client.Shared
{
  public partial class Header
  {
    bool isDesktop = true, isToggled = true, isInitialized;

    bool themeSwitcherShown = false;
    bool ThemeSwitcherShown
    {
      get => themeSwitcherShown;
      set
      {
        themeSwitcherShown = value;
        InvokeAsync(StateHasChanged);
      }
    }

    string _unauthorizedError;

    public string FullName { get; set; }

    [Parameter] public string StateCssClass { get; set; }
    [Parameter] public EventCallback<string> StateCssClassChanged { get; set; }

    public List<LanguageModel> Cultures { get; set; }
    public LanguageModel CurrentCulture { get; set; }

    [Inject] public HttpInterceptorService Interceptor { get; set; }


    protected override async Task OnInitializedAsync()
    {
      _unauthorizedError = string.Format(_localizer[LangKey.MSG_USER_PERMISSION_MAY_HAS_CHANGED_ON_OTHER_DEVICE], 
        "<span id=\"counter\" style=\"color: red; font-weight: bold\">10</span>");
      Interceptor.RegisterEvent();

      var rs = await _apiWrapper.Cultures.GetAllAsync();

      if (rs.IsSuccess && rs.Error == null)
      {
        Cultures = rs.Data;
        CurrentCulture = await GetCurrentCultureAsync();
        HandleLanguageLocalStorage();
      }
      else
      {
        _toastAppService.ShowWarning(_localizer[LangKey.MSG_FAILED_TO_LOAD_LANGUAGES]);
      }

      _navigationManager.LocationChanged += OnLocationChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        await _jsRuntime.InvokeVoidAsync("windowMinWidthMatchesQuery", DotNetObjectReference.Create(this));
      }
      isInitialized = true;
      base.OnAfterRender(firstRender);

      FullName = await _authService.GetCurrentFullName();
    }

    [JSInvokable]
    public async Task OnWindowMinWidthQueryChanged(bool matchesDesktop)
    {

      if (!isInitialized || isDesktop != matchesDesktop)
      {
        isDesktop = matchesDesktop;
        if (isInitialized || !isDesktop)
          await ToggleNavMenu(matchesDesktop);
      }
    }

    async Task OnToggleClick() => await ToggleNavMenu();

    async void OnLocationChanged(object sender, LocationChangedEventArgs args)
    {
      if (!isDesktop)
        await ToggleNavMenu(false);
    }

    async Task ToggleNavMenu(bool? value = null)
    {
      var newValue = value ?? !isToggled;
      isToggled = value ?? !isToggled;
      string stateCssClass = isToggled ? "expand" : "collapse";
      if (StateCssClass != stateCssClass)
      {
        StateCssClass = stateCssClass;
        await StateCssClassChanged.InvokeAsync(StateCssClass);
      }
    }

    public void Dispose()
    {
      _navigationManager.LocationChanged -= OnLocationChanged;
    }

    void OnLogoTittleClick()
    {
      _navigationManager.NavigateTo("", true);
    }

    void ToggleThemeSwitcherPanel()
    {
      ThemeSwitcherShown = !ThemeSwitcherShown;
    }

    async void HandleLogout()
    {
      await _authService.Logout();
    }

    #region Language & Culture

    private async void HandleLanguageLocalStorage()
    {
      var js = (IJSInProcessRuntime)_jsRuntime;
      var currentCultureStorage = await js.InvokeAsync<string>("blazorCulture.get");

      if (!currentCultureStorage.Contains(CurrentCulture.Code))
      {
        await js.InvokeVoidAsync("blazorCulture.set", CurrentCulture.Code);
        await _localStorage.SetItemAsync("userCultureId", CurrentCulture.Id);
        _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
      }
    }

    private void RequestCultureChange(LanguageModel lang)
    {
      CurrentCulture = lang;
      _localStorage.SetItemAsync("userCultureId", lang.Id);

      var js = (IJSInProcessRuntime)_jsRuntime;
      js.InvokeVoid("blazorCulture.set", lang.Code);
      _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
    }

    private async Task<LanguageModel> GetCurrentCultureAsync()
    {
      var defaultCulture = Cultures.Where(c => c.Code.Contains("en")).FirstOrDefault();

      var userCultureIdString = await _localStorage.GetItemAsync<string>("userCultureId");
      var userCultureIdInt = int.TryParse(userCultureIdString, out int userCultureId) ? userCultureId : defaultCulture.Id;

      var foundCulture = Cultures.Where(c => c.Id == userCultureIdInt).FirstOrDefault();
      var result = foundCulture == null ? defaultCulture : foundCulture;

      return result;
    }

    #endregion
  }
}
