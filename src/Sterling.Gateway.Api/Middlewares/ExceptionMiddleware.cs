using System.Net;
using Serilog;

namespace Sterling.Gateway.Api;

public class ExceptionMiddleware
{
    private readonly IDiagnosticContext diagnosticContext;
    private readonly ILogger<ExceptionMiddleware> logger;
    private readonly RequestDelegate request;

    public ExceptionMiddleware(IDiagnosticContext diagnosticContext, ILogger<ExceptionMiddleware> logger, RequestDelegate request)
    {
        this.diagnosticContext = diagnosticContext;
        this.logger = logger;
        this.request = request;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await request(context);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
            }
            else
            {
                logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
            }
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("An Error occurred, please try again later.");

        }
    }
}

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
