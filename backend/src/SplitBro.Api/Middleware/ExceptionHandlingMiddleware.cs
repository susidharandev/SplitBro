using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SplitBro.Api.Middleware
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
                _logger.LogError(ex,"Unhandled Exception in method : {method}, path : {path}", 
                    httpContext.Request.Method, httpContext.Request.Path);

                await HandleExceptionAsync(httpContext,ex);
            }
        }
        public async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = httpContext.Response.StatusCode.ToString(),
                Detail = (httpContext.Response.StatusCode == 500) ? "An unexpected error occurred." : exception.Message,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
    
}
