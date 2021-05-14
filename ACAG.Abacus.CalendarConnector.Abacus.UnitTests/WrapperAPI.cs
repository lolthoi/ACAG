using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ACAG.Abacus.CalendarConnector.Abacus.UnitTests
{

  public class WrapperAPI
  {
    private readonly IHttpClientFactory _clientFactory;

    private readonly AppSettings _appSettings;

    private readonly IHeaderAPI _headerAPI;

    public WrapperAPI(IOptions<AppSettings> options, IHttpClientFactory clientFactory, IHeaderAPI headerAPI)
    {
      _appSettings = options.Value;
      _clientFactory = clientFactory;
      _headerAPI = headerAPI;
    }

    #region Methods call API

    public HttpResponseMessage GetRequest(string functionApi, TenantModel tenant)
    {
      var client = _clientFactory.CreateClient(_appSettings.UrlClientName);
      client.Timeout = new TimeSpan(0, 0, _appSettings.APITimeout);
      SetupClient(client);
      var requestUrl = GetFunctionWithSubUrl(functionApi);
      var json = JsonConvert.SerializeObject(tenant);
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = new Uri(requestUrl),
        Content = new StringContent(json, Encoding.UTF8, "application/json")
      };

      var response = client.SendAsync(request).ConfigureAwait(false);
      var responseInfo = response.GetAwaiter().GetResult();

      return responseInfo;
    }
    #endregion

    private string GetFunctionWithSubUrl(string functionApi)
    {
      return _appSettings.Url + _appSettings.SubUrl + functionApi;
    }

    private void SetupClient(HttpClient client)
    {
      if (client.BaseAddress == null)
      {
        client.BaseAddress = new Uri(_appSettings.Url);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      }

      var headerParam = _headerAPI.GetHeaderParam();
      if (headerParam == null)
        return;

      if (!string.IsNullOrEmpty(headerParam.Language))
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(headerParam.Language));

      if (!string.IsNullOrEmpty(headerParam.Token))
        client.DefaultRequestHeaders.Add("Authorization", headerParam.Token);

      if (headerParam.TenantId.HasValue)
        client.DefaultRequestHeaders.TryAddWithoutValidation("TenantId", headerParam.TenantId.Value.ToString());

    }
  }

  public static class WrapperExtension
  {
    public static T ToContentObject<T>(this HttpResponseMessage responseMessage)
    {
      if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
      {
        return default;
      }

      var data = typeof(T) == typeof(string)
          ? (T)(object)responseMessage.Content.ReadAsStringAsync().Result
          : responseMessage.Content.ReadFromJsonAsync<T>().Result;

      return data;
    }

    public static string ToContentString(this HttpResponseMessage responseMessage)
    {
      if (responseMessage == null || ! responseMessage.IsSuccessStatusCode)
      {
        return null;
      }

      var data = responseMessage.Content.ReadAsStringAsync().Result;

      return data;
    }
  }
}
