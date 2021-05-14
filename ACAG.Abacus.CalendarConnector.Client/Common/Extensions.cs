using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Common
{
  public static class Extensions
  {
    public static T GetDataX<T>(this Task<T> response)
    {
      return response.GetAwaiter().GetResult();
    }

    public static string RemoveInvalidCharacters(this string text)
    {
      if (string.IsNullOrEmpty(text)) return text;
      var c = text.Trim().ToCharArray();
      return string.Join(string.Empty, c.Where(t => t != '\u202c').ToList());
    }

    public static int GetNewIntIdX<T>(this List<T> list, int startId = 1)
    {
      if (list == null || list.Count() == 0) return startId;
      var avaiableIds = new List<int>();
      foreach (var item in list)
      {
        int oldId = (int)item.GetType().GetProperty("Id")?.GetValue(item, null);
        avaiableIds.Add(oldId);
      }
      avaiableIds.RemoveAll(t => t == 0);
      int maxId = avaiableIds.Max(t => t);
      return maxId + 1;
    }

    public static int GetIdMaxWithStringProperty<T>(this List<T> list, string propertyId = "Id", int startId = 1)
    {
      if (list == null || list.Count() == 0) return startId;
      var avaiableIds = new List<int>();
      foreach (var item in list)
      {
        int oldId = (int)item.GetType().GetProperty(propertyId)?.GetValue(item, null);
        avaiableIds.Add(oldId);
      }
      avaiableIds.RemoveAll(t => t == 0);
      int maxId = avaiableIds.Max(t => t);
      return maxId + 1;
    }

  }

  public static class AppUtils
  {
    public static Roles ConvertToRole(string role)
    {
      switch (role)
      {
        case AppDefiner.Roles.ADMINISTRATOR:
          return Roles.ADMINISTRATOR;

        default:
        case AppDefiner.Roles.USER:
          return Roles.USER;
      }
    }
  }
}
