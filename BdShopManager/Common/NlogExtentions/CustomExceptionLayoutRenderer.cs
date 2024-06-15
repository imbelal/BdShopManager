using Common.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.LayoutRenderers;
using System.Text;

namespace Common.NlogExtentions
{
    [LayoutRenderer("formatted-exception")]
    public class CustomExceptionLayoutRenderer : LayoutRenderer
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExceptionRendererService _exceptionRendererService;
        public CustomExceptionLayoutRenderer(IHostEnvironment hostingEnvironment, ICurrentUserService currentUserService, IExceptionRendererService exceptionRendererService)
        {
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
            _exceptionRendererService = exceptionRendererService;
        }
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            AppendMessage(builder, logEvent);
            this.AddRequestInfo(builder);
            if (logEvent.Exception != null)
            {
                (Exception innerMostException, object messageObject) = this.AppendExceptionInfo(builder, logEvent);
                AppendMessageObject(builder, messageObject);
                AppendStackTrace(builder, innerMostException);
            }
        }

        private static void AppendMessage(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append("<h2>Logged message</h2>");
            builder.AppendLine(logEvent.FormattedMessage);
        }

        private void AddRequestInfo(StringBuilder builder)
        {
            string user;
            if (_hostingEnvironment.IsDevelopment())
            {
                user = "System from dev env.";
            }
            else
            {
                user = _currentUserService.GetUser().Identity?.Name ?? "System";
            }

            builder.Append("<h2>RequestInfo:</h2>");
            builder.AppendLine($"Environment: {_hostingEnvironment.EnvironmentName}<br>");
            builder.AppendLine($"User: {user}<br>");
        }

        private (Exception, object) AppendExceptionInfo(StringBuilder builder, LogEventInfo logEvent)
        {
            Exception exception = logEvent.Exception;
            builder.AppendLine("<h2>Exceptions</h2>");
            builder.AppendLine($"<b>{exception.GetType()}</b>: {exception.Message}");
            this._exceptionRendererService.RenderAdditionalExceptionInformation(builder, exception);
            object messageObject = this._exceptionRendererService.GetMessageObject(exception);
            Exception? innerException = exception.InnerException;
            Exception innerMostException = exception;
            while (innerException != null)
            {
                innerMostException = innerException;
                builder.AppendLine("");
                builder.AppendLine($"<b>[Inner exception] {innerException.GetType()}</b> : {innerException.Message}");
                this._exceptionRendererService.RenderAdditionalExceptionInformation(builder, innerException);
                innerException = innerException.InnerException;

            }

            return (innerMostException, messageObject);
        }

        private static void AppendMessageObject(StringBuilder builder, object messageObject)
        {
            if (messageObject != null)
            {
                builder.AppendLine("<h2>Processed message</h2>");
                builder.AppendLine($"<b>{messageObject.GetType().Name}</b> : {Newtonsoft.Json.JsonConvert.SerializeObject(messageObject, Newtonsoft.Json.Formatting.Indented)}");

            }
        }

        private static void AppendStackTrace(StringBuilder builder, Exception exception)
        {
            builder.Append("<h2>Stacktrace</h2>");
            builder.AppendLine(exception.StackTrace);
        }

    }
}
