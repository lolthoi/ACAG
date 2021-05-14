using System.Net;

namespace ACAG.Abacus.CalendarConnector.Wrappers
{
  public class ApiResult<T>
  {
    public T Data { get; set; }

    public ResponseError Error { get; set; }

    public bool IsSuccess => Error == null;

    public class ResponseError
    {
      public HttpStatusCode StatusCode { get; set; }

      public int Code { get; set; }

      public string[] Messages { get; set; }

      /// <summary>
      /// Get first message from messages array.
      /// It returns Empty if Messages is null or empty.
      /// </summary>
      public string Message
      {
        get
        {
          if (Messages == null && Messages.Length == 0)
          {
            return string.Empty;
          }
          return Messages[0];
        }
      }
    }
  }
}
