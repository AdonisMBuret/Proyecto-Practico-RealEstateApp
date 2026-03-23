using Microsoft.AspNetCore.Builder;

namespace RealEstateApp.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
        {
            // Usa el GlobalExceptionHandler (IExceptionHandler) registrado en ServiceCollectionExtensions
            // Maneja correctamente:
            // - KeyNotFoundException -> 404 NotFound
            // - ArgumentException -> 400 BadRequest
            // - ValidationException -> 400 BadRequest
            // - etc.
            return app.UseExceptionHandler();
        }
    }
}