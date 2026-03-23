using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace RealEstateApp.Api.Handlers;


public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
       
        LogException(exception, httpContext);

        
        var problemDetails = CreateProblemDetails(httpContext, exception);

       
        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

       
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, options), 
            cancellationToken);

        return true;
    }

    private void LogException(Exception exception, HttpContext context)
    {
        var logLevel = GetLogLevel(exception);
        var template = "API Exception: {ExceptionType} | Path: {Path} | Method: {Method} | User: {User} | TraceId: {TraceId}";

        _logger.Log(logLevel, exception, template,
            exception.GetType().Name,
            context.Request.Path,
            context.Request.Method,
            context.User.Identity?.Name ?? "Anonymous",
            context.TraceIdentifier);
    }

    private static LogLevel GetLogLevel(Exception exception) => exception switch
    {
        ValidationException => LogLevel.Warning,
        DomainException => LogLevel.Warning,
        ArgumentNullException => LogLevel.Warning, 
        ArgumentException => LogLevel.Warning,
        UnauthorizedAccessException => LogLevel.Warning,
        _ => LogLevel.Error
    };

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var (status, title, detail, type) = GetErrorDetails(exception);

        var problemDetails = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = _environment.IsDevelopment() ? detail : GetSafeDetail(exception, detail),
            Type = type,
            Instance = context.Request.Path
        };

        
        AddStandardExtensions(problemDetails, context, exception);

       
        AddSpecificExtensions(problemDetails, exception);

        return problemDetails;
    }

    private (HttpStatusCode status, string title, string detail, string type) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Errores de validación",
                "Uno o más campos tienen errores de validación",
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            ),

            
            BusinessValidationException validationEx => (
                validationEx.StatusCode,
                GetTitleForStatus(validationEx.StatusCode),
                validationEx.Message,
                validationEx.ErrorType
            ),

            NotFoundException notFoundEx => (
                notFoundEx.StatusCode,
                GetTitleForStatus(notFoundEx.StatusCode),
                notFoundEx.Message,
                notFoundEx.ErrorType
            ),

            ConflictException conflictEx => (
                conflictEx.StatusCode,
                GetTitleForStatus(conflictEx.StatusCode),
                conflictEx.Message,
                conflictEx.ErrorType
            ),

            ForbiddenException forbiddenEx => (
                forbiddenEx.StatusCode,
                GetTitleForStatus(forbiddenEx.StatusCode),
                forbiddenEx.Message,
                forbiddenEx.ErrorType
            ),

            
            DomainException domainEx => (
                domainEx.StatusCode,
                GetTitleForStatus(domainEx.StatusCode),
                domainEx.Message,
                domainEx.ErrorType
            ),

            
            ArgumentNullException argNullEx => (
                HttpStatusCode.BadRequest,
                "Parámetro requerido faltante",
                argNullEx.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            ),

            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                "Parámetros inválidos",
                argEx.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            ),

            KeyNotFoundException keyNotFoundEx => (
                HttpStatusCode.NotFound,
                "Recurso no encontrado",
                keyNotFoundEx.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            ),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Acceso no autorizado",
                "No tiene permisos para realizar esta operación",
                "https://tools.ietf.org/html/rfc7235#section-3.1"
            ),

            InvalidOperationException invalidOpEx => (
                HttpStatusCode.Conflict,
                "Operación no válida",
                invalidOpEx.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            ),

            TimeoutException => (
                HttpStatusCode.RequestTimeout,
                "Tiempo de espera agotado",
                "La operación tardó demasiado tiempo en completarse",
                "https://tools.ietf.org/html/rfc7231#section-6.5.7"
            ),

            NotImplementedException => (
                HttpStatusCode.NotImplemented,
                "Funcionalidad no implementada",
                "Esta funcionalidad aún no está disponible",
                "https://tools.ietf.org/html/rfc7231#section-6.6.2"
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
            ValidationException => originalDetail,
            DomainException => originalDetail, 
            ArgumentNullException => originalDetail, 
            ArgumentException => originalDetail, 
            KeyNotFoundException => originalDetail,
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
        HttpStatusCode.NotFound => "Recurso no encontrado",
        HttpStatusCode.Conflict => "Conflicto",
        HttpStatusCode.UnprocessableEntity => "Entidad no procesable",
        HttpStatusCode.InternalServerError => "Error interno del servidor",
        _ => "Error"
    };

    private static void AddStandardExtensions(ProblemDetails problemDetails, HttpContext context, Exception exception)
    {
        problemDetails.Extensions.Add("timestamp", DateTime.UtcNow.ToString("O"));
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
        problemDetails.Extensions.Add("path", context.Request.Path.Value);
        problemDetails.Extensions.Add("method", context.Request.Method);
        
       
        if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            problemDetails.Extensions.Add("machine", Environment.MachineName);
            problemDetails.Extensions.Add("exception", exception.GetType().FullName);
        }
    }

    private static void AddSpecificExtensions(ProblemDetails problemDetails, Exception exception)
    {
        switch (exception)
        {
            case ValidationException fluentValidationEx:
                var validationErrors = fluentValidationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                problemDetails.Extensions.Add("errors", validationErrors);
                break;

            case BusinessValidationException validationEx:
                problemDetails.Extensions.Add("errors", validationEx.Errors);
                break;

            case ArgumentException argumentEx when !string.IsNullOrEmpty(argumentEx.ParamName):
                problemDetails.Extensions.Add("parameterName", argumentEx.ParamName);
                break;

            case ArgumentNullException argNullEx when !string.IsNullOrEmpty(argNullEx.ParamName):
                problemDetails.Extensions.Add("parameterName", argNullEx.ParamName);
                problemDetails.Extensions.Add("helpUrl", "https://docs.realestate.app/errors/missing-parameter");
                break;

            case NotFoundException:
                problemDetails.Extensions.Add("supportUrl", "https://docs.realestate.app/errors/not-found");
                break;

            case ConflictException:
                problemDetails.Extensions.Add("helpUrl", "https://docs.realestate.app/errors/conflict");
                break;

            case ForbiddenException:
                problemDetails.Extensions.Add("supportUrl", "https://docs.realestate.app/errors/forbidden");
                break;
        }
    }
}
