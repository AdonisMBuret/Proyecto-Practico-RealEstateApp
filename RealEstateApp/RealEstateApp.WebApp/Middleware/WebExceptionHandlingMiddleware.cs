using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Domain.Exceptions;
using System.Net;

namespace RealEstateApp.WebApp.Middleware;


public class WebExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WebExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public WebExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<WebExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
       
        LogException(exception, context);

       
        if (IsAjaxRequest(context))
        {
            await HandleAjaxException(context, exception);
        }
        else
        {
            await HandleWebException(context, exception);
        }
    }

    private void LogException(Exception exception, HttpContext context)
    {
        var logLevel = GetLogLevel(exception);
        var template = "WebApp Exception: {ExceptionType} | Path: {Path} | User: {User} | TraceId: {TraceId}";

        _logger.Log(logLevel, exception, template,
            exception.GetType().Name,
            context.Request.Path,
            context.User.Identity?.Name ?? "Anonymous",
            context.TraceIdentifier);
    }

    private static LogLevel GetLogLevel(Exception exception) => exception switch
    {
        DomainException => LogLevel.Warning,
        ArgumentException => LogLevel.Warning,
        UnauthorizedAccessException => LogLevel.Warning,
        _ => LogLevel.Error
    };

    private static bool IsAjaxRequest(HttpContext context)
    {
        return context.Request.Headers.XRequestedWith == "XMLHttpRequest" ||
               context.Request.Headers.Accept.ToString().Contains("application/json") ||
               context.Request.Path.StartsWithSegments("/api");
    }

    private async Task HandleAjaxException(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = problemDetails.Detail,
            error = problemDetails.Title,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private async Task HandleWebException(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);

        
        if (context.Features.Get<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory>()?.GetTempData(context) is var tempData && tempData != null)
        {
            var userFriendlyMessage = GetUserFriendlyMessage(exception);
            tempData["Error"] = userFriendlyMessage;

            
            if (_environment.IsDevelopment())
            {
                tempData["ErrorDetails"] = problemDetails.Detail;
                tempData["ErrorType"] = exception.GetType().Name;
                tempData["TraceId"] = context.TraceIdentifier;
            }
        }

       
        var errorPath = GetErrorPath(problemDetails.Status ?? 500);
        context.Response.Redirect(errorPath);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var (status, title, detail, type) = GetErrorDetails(exception);

        return new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = _environment.IsDevelopment() ? detail : GetSafeDetail(exception, detail),
            Type = type,
            Instance = context.Request.Path,
            Extensions =
            {
                { "timestamp", DateTime.UtcNow.ToString("O") },
                { "traceId", context.TraceIdentifier }
            }
        };
    }

    private (HttpStatusCode status, string title, string detail, string type) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            DomainException domainEx => (
                domainEx.StatusCode,
                GetTitleForStatus(domainEx.StatusCode),
                domainEx.Message,
                domainEx.ErrorType
            ),
            ArgumentException => (
                HttpStatusCode.BadRequest,
                "Parámetros inválidos",
                exception.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Acceso no autorizado",
                "No tiene permisos para realizar esta operación",
                "https://tools.ietf.org/html/rfc7235#section-3.1"
            ),
            InvalidOperationException => (
                HttpStatusCode.Conflict,
                "Operación no válida",
                exception.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Error interno del servidor",
                "Ha ocurrido un error interno inesperado",
                "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            )
        };
    }

    private string GetSafeDetail(Exception exception, string originalDetail)
    {
        return exception switch
        {
            DomainException => originalDetail,
            ArgumentException => originalDetail,
            _ when originalDetail.Contains("Internal", StringComparison.OrdinalIgnoreCase) => 
                "Ha ocurrido un error interno. Contacte al administrador si persiste.",
            _ => originalDetail
        };
    }

    private static string GetTitleForStatus(HttpStatusCode status) => status switch
    {
        HttpStatusCode.BadRequest => "Solicitud incorrecta",
        HttpStatusCode.Unauthorized => "No autorizado",
        HttpStatusCode.Forbidden => "Prohibido",
        HttpStatusCode.NotFound => "Página no encontrada",
        HttpStatusCode.Conflict => "Conflicto",
        HttpStatusCode.InternalServerError => "Error interno del servidor",
        _ => "Error"
    };

    private static string GetUserFriendlyMessage(Exception exception) => exception switch
    {
        NotFoundException => "El recurso solicitado no fue encontrado",
        BusinessValidationException => "Los datos proporcionados no son válidos",
        ConflictException => "No se puede completar la operación debido a un conflicto",
        ForbiddenException => "No tiene permisos para realizar esta acción",
        ArgumentException => "Los parámetros proporcionados no son válidos",
        UnauthorizedAccessException => "Debe iniciar sesión para continuar",
        _ => "Ha ocurrido un error. Intente nuevamente o contacte al administrador"
    };

    private static string GetErrorPath(int statusCode) => statusCode switch
    {
        401 => "/Account/Login",
        403 => "/Home/AccessDenied", 
        404 => "/Home/NotFound",
        _ => "/Home/Error"
    };
}