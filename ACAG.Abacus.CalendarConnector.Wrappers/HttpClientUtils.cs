using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using static ACAG.Abacus.CalendarConnector.Wrappers.HeaderParam;

namespace ACAG.Abacus.CalendarConnector.Wrappers
{
  public static class HttpClientUtils
  {
    public const string UrlClientName = "AbacusWrapperApi";
    public const string Url = "http://localhost:55453/WrongUrl";
    public const int APITimeout = 30;

    #region GET

    public static ApiResult<T> GetRequest<T>(IHttpClientFactory clientFactory, IHeaderAPI headerAPI, string functionApi)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);
      
      SetupClient(headerAPI, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = client.GetAsync(uri).Result;

      return ReadStringAsObject<T>(responseMessage);
    }

    public static async Task<ApiResult<T>> GetRequestAync<T>(IHttpClientFactory clientFactory, ILocalStorageService localStorage, string functionApi)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      await SetupClient(localStorage, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = await client.GetAsync(uri);

      return ReadStringAsObject<T>(responseMessage);
    }

    #endregion

    #region POST

    public static ApiResult<T> PostRequest<T>(IHttpClientFactory clientFactory, IHeaderAPI headerAPI, string functionApi, object model)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);
      
      SetupClient(headerAPI, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = client.PostAsJsonAsync(uri, model).Result;

      return ReadStringAsObject<T>(responseMessage);
    }

    public static async Task<ApiResult<T>> PostRequestAsync<T>(IHttpClientFactory clientFactory, ILocalStorageService localStorage, string functionApi, object model)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      await SetupClient(localStorage, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = await client.PostAsJsonAsync(uri, model);

      return ReadStringAsObject<T>(responseMessage);
    }

    #endregion

    #region PUT

    public static ApiResult<T> PutRequest<T>(IHttpClientFactory clientFactory, IHeaderAPI headerAPI, string functionApi, object model)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      SetupClient(headerAPI, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = client.PutAsJsonAsync(uri, model).Result;

      return ReadStringAsObject<T>(responseMessage);
    }

    public static async Task<ApiResult<T>> PutRequestAsync<T>(IHttpClientFactory clientFactory, ILocalStorageService localStorage, string functionApi, object model)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      await SetupClient(localStorage, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = await client.PutAsJsonAsync(uri, model);

      return ReadStringAsObject<T>(responseMessage);
    }

    #endregion

    #region DELETE

    public static ApiResult<T> DeleteRequest<T>(IHttpClientFactory clientFactory, IHeaderAPI headerAPI, string functionApi)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      SetupClient(headerAPI, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = client.DeleteAsync(uri).Result;

      return ReadStringAsObject<T>(responseMessage);
    }

    public static async Task<ApiResult<T>> DeleteRequestAsync<T>(IHttpClientFactory clientFactory, ILocalStorageService localStorage, string functionApi)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      await SetupClient(localStorage, client);

      var uri = new Uri($"{client.BaseAddress}{functionApi}");
      HttpResponseMessage responseMessage = await client.DeleteAsync(uri);

      return ReadStringAsObject<T>(responseMessage);
    }

    public static async Task<ApiResult<T>> DeleteRequestAsync<T>(IHttpClientFactory clientFactory, ILocalStorageService localStorage, string functionApi, List<int> ids)
    {
      var client = clientFactory.CreateClient(UrlClientName);
      client.Timeout = new TimeSpan(0, 0, APITimeout);

      await SetupClient(localStorage, client);

      var listIdJson = JsonConvert.SerializeObject(ids);

      HttpRequestMessage request = new HttpRequestMessage
      {
        Content = new StringContent(listIdJson, Encoding.UTF8, "application/json"),
        Method = HttpMethod.Delete,
        RequestUri = new Uri($"{client.BaseAddress}{functionApi}")
      };

      HttpResponseMessage responseMessage = await client.SendAsync(request);

      return ReadStringAsObject<T>(responseMessage);
    }

    #endregion

    #region Private methods

    private static void SetupClient(IHeaderAPI headerAPI, HttpClient client)
    {
      if (client.BaseAddress == null)
      {
        client.BaseAddress = new Uri(Url);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      }

      var headerParam = headerAPI.GetHeaderParam();
      if (headerParam == null)
        return;

      if (!string.IsNullOrEmpty(headerParam.Language))
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(headerParam.Language));

      if (!string.IsNullOrEmpty(headerParam.Token))
        client.DefaultRequestHeaders.Add("Authorization", headerParam.Token);

      if (headerParam.TenantId.HasValue)
        client.DefaultRequestHeaders.TryAddWithoutValidation("TenantId", headerParam.TenantId.Value.ToString());
    }

    private async static Task SetupClient(ILocalStorageService localStorage, HttpClient client)
    {
      if (client.BaseAddress == null)
      {
        client.BaseAddress = new Uri(Url);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      }

      var token = await localStorage.GetItemAsync<string>("authToken");
      if (token == null)
        return;

      if (!string.IsNullOrEmpty(token))
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    }

    private static ApiResult<T> ReadStringAsObject<T>(HttpResponseMessage responseMessage)
    {
      if (responseMessage.IsSuccessStatusCode)
      {
        var data = typeof(T) == typeof(string)
          ? (T)(object)responseMessage.Content.ReadAsStringAsync().Result
          : responseMessage.Content.ReadAsAsync<T>().Result;

        return new ApiResult<T> { Data = data };
      }

      var message = responseMessage.Content.ReadAsStringAsync().Result;
      var error = StringToErrorJson<T>(message);
      error.StatusCode = responseMessage.StatusCode;
      error.Code = (int)responseMessage.StatusCode;
      return new ApiResult<T> { Error = error };
    }

    private static ApiResult<T>.ResponseError StringToErrorJson<T>(string message)
    {
      if (string.IsNullOrEmpty(message))
      {
        return new ApiResult<T>.ResponseError
        {
          Code = 3,
          Messages = Array.Empty<string>()
        };
      }
      try
      {
        return JsonConvert.DeserializeObject<ApiResult<T>.ResponseError>(message);
      }
      catch
      {
        return new ApiResult<T>.ResponseError
        {
          Code = 3,
          Messages = new string[] { message }
        };
      }
    }

    #endregion
  }
}
