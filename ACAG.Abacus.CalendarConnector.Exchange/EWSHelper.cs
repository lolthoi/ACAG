using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using Newtonsoft.Json;

namespace ACAG.Abacus.CalendarConnector.Exchange
{
  public static class EWSHelper
  {
    private static string ToHexString(string str)
    {
      var sb = new StringBuilder();

      var bytes = Encoding.UTF8.GetBytes(str);

      foreach (var t in bytes)
      {
        sb.Append(t.ToString("X2"));
      }

      return sb.ToString();
    }

    private static string FromHexString(string hexStr)
    {
      var bytes = new byte[hexStr.Length / 2];

      for (int i = 0; i < bytes.Length; i++)
      {
        bytes[i] = Convert.ToByte(hexStr.Substring(i * 2, 2), 16);
      }

      return Encoding.UTF8.GetString(bytes);
    }

    private static JsonSerializerSettings GetJsonSetting()
    {
      return new JsonSerializerSettings
      {
        StringEscapeHandling = StringEscapeHandling.EscapeHtml,
        Formatting = Formatting.Indented
      };
    }

    public static string ObjectToString<T>(T item)
    {
      var itemValue = ToHexString(JsonConvert.SerializeObject(item, GetJsonSetting()));
      return itemValue;
    }

    public static T StringToObject<T>(string itemValue)
    {
      return JsonConvert.DeserializeObject<T>(FromHexString(itemValue));
    }

    /// <summary>
    /// You can modify this logic style to generate itemId for appointment
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string CreateItemId(AppointmentModel data, int tenantId)
    {
      return ToHexString(string.Format("{0}{1}{2}", tenantId, data.AbacusID, data.MailAccount));
    }
  }
}
