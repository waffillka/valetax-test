using System.Net;
using System.Text.Json;
using Valetax.Domain.Models.Exceptions;
using Valetax.Services.Interfaces;
using Valetax.Services.Model.DTOs;

namespace Valetax.Host.Infrastructure.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly IExceptionJournalService _exceptionJournalService;

    public ExceptionMiddleware(IExceptionJournalService exceptionJournalService)
    {
        _exceptionJournalService = exceptionJournalService;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var dto = await _exceptionJournalService.CreateAsync(new ExceptionJournalDto()
        {
            StackTrace = exception.StackTrace,
            RequestParameters = context.Request.QueryString.Value,
            Type = exception.GetType().Name,
            TraceIdentifier = context.Request.HttpContext.TraceIdentifier,
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails()
        {
            Id = dto.TraceIdentifier,
            Type = exception.GetType().Name,
            Data = new ErrorDetailsData()
            {
                Message = exception.Message
            }
        }));
    }
}