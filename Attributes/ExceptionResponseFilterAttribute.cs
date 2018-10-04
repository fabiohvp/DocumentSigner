using DocumentSigner.Helpers;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace DocumentSigner.Attributes
{
    internal class ExceptionResponseFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            base.OnException(context);
            LogHelper.WriteLog("An exception has occurred." + Environment.NewLine + context.Exception.StackTrace, EventLogEntryType.Error);
            
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, context.Exception);
        }
    }
}
