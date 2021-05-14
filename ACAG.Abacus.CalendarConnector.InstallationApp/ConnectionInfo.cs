using System.Linq;
using Microsoft.Data.SqlClient;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public class ConnectionInfo
  {
    public string Type { get; set; } = "Instance";
    public string IP { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public string Catalog { get; set; } = string.Empty;
    public string Security { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    //public override string ToString()
    //{
    //  var builder = new SqlConnectionStringBuilder();
    //  var isInstance = Type == "Instance";

    //  builder.DataSource = isInstance ? ServerName : (IP + "," + Port);
    //  builder.InitialCatalog = Catalog;

    //  switch (Security)
    //  {
    //    case "SSPI":
    //      builder["Integrated Security"] = "SSPI";
    //      break;
    //    case "Persist":
    //      builder.PersistSecurityInfo = true;
    //      break;
    //    case "Trusted":
    //      builder["Trusted_Connection"] = true;
    //      break;
    //    default:
    //      break;
    //  }

    //  builder.UserID = Username;
    //  builder.Password = Password;
    //  return builder.ConnectionString;
    //}

    public override string ToString()
    {
      var dataSource = Type == "Instance" 
        ? ServerName
        //: Port == string.Empty ? IP : (IP + "," + Port);
        : (IP + "," + Port);

      var security = GetSecurity(Security);

      var res = string.Format("Data Source={0};Initial Catalog={1};{2}User ID={3};Password={4}",
        dataSource,
        Catalog,
        security,
        Username,
        Password
        );
      return res;
    }

    private string GetSecurity(string security)
    {
      switch (security)
      {
        case "SSPI":
          return "Integrated Security=SSPI;";
        case "Persist":
          return "Persist Security Info=True;";
        case "Trusted":
          return "Trusted_Connection=True;";
        default:
          return string.Empty;
      }
    }
  }
  public class ConnectionBuilder
  {
    public string ConnectionString { get; private set; }

    private ConnectionInfo _connectionInfo;

    public ConnectionBuilder(string connectionString)
    {
      ConnectionString = connectionString;
    }

    private bool IsNumberOrEmpty(string input, int maxLength, out string result)
    {
      if (string.IsNullOrWhiteSpace(input))
      {
        result = string.Empty;
        return true;
      }
      if (input.Trim().Length <= maxLength && int.TryParse(input, out int port))
      {
        result = port.ToString();
        return true;
      }
      result = string.Empty;
      return false;
    }

    public ConnectionInfo ToObject()
    {
      if (_connectionInfo != null)
      {
        return _connectionInfo;
      }
      try
      {
        var connectionInfo = new ConnectionInfo();

        var arrs = ConnectionString
          .Split(';', System.StringSplitOptions.RemoveEmptyEntries);

        var pairs = arrs.Select(t => t.Split('='))
          .Where(t => t.Length == 2)
          .Select(t => new { key = t[0].Trim(), value = t[1].Trim() })
          .ToList();

        foreach (var item in pairs)
        {
          var key = item.key.ToLower();
          switch (key)
          {
            case "data source":
              var sourceArrs = item.value.Split(',');
              if (sourceArrs.Length == 2 && IsNumberOrEmpty(sourceArrs[1], 6, out string number))
              {
                connectionInfo.Type = "IP";
                connectionInfo.IP = sourceArrs[0];
                connectionInfo.Port = number;
              }
              else
              {
                connectionInfo.Type = "Instance";
                connectionInfo.ServerName = item.value;
              }
              break;
            case "initial catalog":
              connectionInfo.Catalog = item.value;
              break;

            // Security
            case "trusted_connection":
              if (item.value.ToUpper() == "TRUE")
              {
                connectionInfo.Security = "Trusted";
              }
              break;
            case "persist security info":
              if (item.value.ToUpper() == "TRUE")
              {
                connectionInfo.Security = "Persist";
              }
              break;
            case "integrated security":
              if (item.value.ToUpper() == "SSPI")
              {
                connectionInfo.Security = "SSPI";
              }
              break;
            // End Security

            case "user id":
              connectionInfo.Username = item.value;
              break;
            case "password":
              connectionInfo.Password = item.value;
              break;
          }
        }

        if (connectionInfo.Security == string.Empty && arrs.Length == 4)
        {
          connectionInfo.Security = "None (Azure)";
        }

        _connectionInfo = connectionInfo;

        return _connectionInfo;
      }
      catch
      {
        return null;
      }
    }
  }
}