using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Newtonsoft.Json;

namespace ACAG.Abacus.CalendarConnector.Logic.Common
{
  public static class Utils
  {
    public static string AppGetFolderPath(string subFolder, bool createByDefault = true)
    {
      if (string.IsNullOrEmpty(subFolder))
        throw new Exception("Folder argument Null Or Empty.");

      subFolder = RemoveInvalidCharacters(subFolder);

      if (Directory.Exists(subFolder))
        return new DirectoryInfo(subFolder).FullName;

      string folderPath = Path.Combine(Environment.CurrentDirectory, subFolder);

      if (createByDefault && !Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);

      return Directory.Exists(folderPath) ? new DirectoryInfo(folderPath).FullName : new DirectoryInfo(subFolder).FullName;
    }

    public static string AppGetFilePath(string subFilePath, bool createByDefault = false)
    {
      if (string.IsNullOrEmpty(subFilePath))
        throw new Exception("File argument Null Or Empty.");

      subFilePath = RemoveInvalidCharacters(subFilePath);

      if (File.Exists(subFilePath))
        return subFilePath;

      string filePath = Path.Combine(Environment.CurrentDirectory, subFilePath);

      if (createByDefault && !File.Exists(filePath))
        File.WriteAllText(filePath, "");

      return filePath;
    }

    public static Dictionary<string, string> LoadLanguageFile(string sourceFolder, string langCode = "en")
    {
      var dicLang = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(langCode)) return dicLang;
      var fileName = string.Format("lang-{0}.txt", langCode);
      var filePath = Path.Combine(sourceFolder, fileName);
      if (string.IsNullOrEmpty(fileName) || !File.Exists(filePath)) return dicLang;

      string line;

      using (var file = new StreamReader(filePath))
      {
        
        while ((line = file.ReadLine()) != null)
        {
          try
          {
            var lines = line.Split('=');
            if (lines.Length > 1 && !dicLang.ContainsKey(lines[0].Trim()))
            {
              dicLang.Add(lines[0].Trim(), lines[1].Trim());
            }
          }
          catch { }
        }

        file.Close();
      }

      return dicLang;
    }

    public static string EncryptedPassword(string password, string salt = null)
    {
      var cryptoher = new Cryptopher();
      if (!string.IsNullOrEmpty(salt))
        cryptoher.AppKeySalt = salt;
      return cryptoher.PasswordHash(password);
    }

    public static string ConvertToStrongId(string idInput)
    {
      return EncryptedPassword(idInput);
    }

    private static string RemoveInvalidCharacters(string text)
    {
      if (string.IsNullOrEmpty(text)) return text;
      var c = text.Trim().ToCharArray();
      return string.Join(string.Empty, c.Where(t => t != '\u202c').ToList());
    }

    public static string ConvertAbacusSettingHealthStatus(bool? healthStatus)
    {
      if (healthStatus == null)
      {
        return "";
      }
      else if (healthStatus.Value)
      {
        return LangKey.TEST_PASS;
      }
      else
      {
        return LangKey.TEST_FAIL;
      }
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
      var itemValue = JsonConvert.SerializeObject(item, GetJsonSetting());
      return itemValue;
    }

    public static T StringToObject<T>(string itemValue)
    {
      return JsonConvert.DeserializeObject<T>(itemValue);
    }

    public static string ValidUsername = "^\\s*[.A-Za-z0-9]*\\s*$";
    public static string FOOTER = "FOOTER";



  }

  public static class ValueExtensions
  {
    public static int LimitToRange(
        this int value, int inclusiveMinimum, int inclusiveMaximum)
    {
      if (value < inclusiveMinimum) { return inclusiveMinimum; }
      if (value > inclusiveMaximum) { return inclusiveMaximum; }
      return value;
    }
  }
}
