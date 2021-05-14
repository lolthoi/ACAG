﻿using System;
using System.Net;
using ACAG.Abacus.CalendarConnector.Models;

namespace ACAG.Abacus.CalendarConnector.Server
{
  public class LogicException : Exception
  {
    public LogicException(ErrorModel error) : base(error.Message)
    {
      Error = error;
      StatusCode = GetHttpStatus(error.Type);
    }

    public ErrorModel Error { get; set; }

    private HttpStatusCode GetHttpStatus(ErrorType type)
    {
      switch (type)
      {
        case ErrorType.BAD_REQUEST:
          return HttpStatusCode.BadRequest;

        case ErrorType.NOT_AUTHORIZED:
          return HttpStatusCode.Unauthorized;

        case ErrorType.NOT_EXIST:
          return HttpStatusCode.NotFound;

        case ErrorType.CONFLICTED:
        case ErrorType.DUPLICATED:
          return HttpStatusCode.Conflict;

        case ErrorType.CONFLICTED_ROLE_CHANGE:
          return HttpStatusCode.ExpectationFailed;

        case ErrorType.NO_ROLE:
        case ErrorType.NO_DATA_ROLE:
        case ErrorType.NOT_AUTHENTICATED:
          return HttpStatusCode.Forbidden;

        case ErrorType.INTERNAL_ERROR:
        default:
          return HttpStatusCode.InternalServerError;
      }
    }
    public HttpStatusCode StatusCode { get; private set; }
  }
}
