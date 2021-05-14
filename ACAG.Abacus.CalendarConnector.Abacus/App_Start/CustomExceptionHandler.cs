using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace ACAG.Abacus.CalendarConnector.Abacus
{
    public class CustomExceptionHandler : IExceptionHandler
    {

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var ex = context.Exception as BusinessException;

            var jsonObj = new
            {
                Error = ex == null ? HttpStatusCode.InternalServerError : ex.StatusCode,
                context.Exception.Message
            };

            var httpResponse = context.Request.CreateResponse(jsonObj.Error, jsonObj);

            context.Result = new ResponseMessageResult(httpResponse);

            return Task.FromResult(0);

        }

        public virtual bool ShouldHandle(ExceptionHandlerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.ExceptionContext.CatchBlock.IsTopLevel;
        }
    }

    public class CustomExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            var log = context.Exception.ToString();
        }
    }
}