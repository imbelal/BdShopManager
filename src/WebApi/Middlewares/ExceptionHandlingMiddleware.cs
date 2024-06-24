using Application.Services.Exceptions;
using Common.ResponseWrapper;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpResponse response = context.Response;
            string errorMessage;

            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        errorMessage = ex.Message;
                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = ex.Message;
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorMessage = ex.Message;
                    break;
                case SignInException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorMessage = ex.Message;
                    break;
                case UserNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorMessage = ex.Message;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = "Internal Server errors. Check Logs!";
                    break;
            }

            _logger.LogError(exception, exception.Message);
            var result = JsonSerializer.Serialize(Response.Fail<string>(errorMessage));
            await context.Response.WriteAsync(result);
        }
    }
}