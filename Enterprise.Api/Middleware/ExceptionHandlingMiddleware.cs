using Enterprise.Application.Common.Models;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Enterprise.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static async Task HandleException(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType =
                "application/json";

            switch (exception)
            {
                case ValidationException validationException:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.BadRequest;

                    var response =
                                ApiResponse.FailureResponse(
                                    "Validation failed",
                                    validationException.Errors
                                        .Select(x => x.ErrorMessage));

                    await context.Response.WriteAsJsonAsync(response);

                    break;

                default:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.InternalServerError;

                    var responseS =
                            ApiResponse.FailureResponse(
                                "An unexpected error occurred");

                    await context.Response.WriteAsJsonAsync(responseS);

                    break;
            }
        }
    }
}
