using Enterprise.Api.Middleware;

namespace Enterprise.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder
            UseCustomExceptionHandler(
                this IApplicationBuilder app)
        {
            return app.UseMiddleware<
                ExceptionHandlingMiddleware>();
        }
    }
}
