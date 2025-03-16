using System.Text.Json;
using FluentValidation;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Api.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {errorMessage}", exception.Message);

        var statusCode = GetStatusCode(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        string result;

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);

            var response = new ValidationErrorResponse(statusCode, errors);
            result = JsonSerializer.Serialize(response, _options);
        }
        else
        {
            var response = new ErrorResponse(statusCode, exception.Message);
            result = JsonSerializer.Serialize(response, _options);
        }

        return context.Response.WriteAsync(result);
    }

    private static int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentNullException or ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            AlreadyExistsException => StatusCodes.Status409Conflict,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}