using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Valetax.Domain.Models.Exceptions;
using Valetax.Services.Interfaces;
using Valetax.Services.Model.DTOs;

namespace Valetax.Host.Infrastructure.Filters;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    private readonly IExceptionJournalService _exceptionJournalService;

    public GlobalExceptionFilter(IExceptionJournalService exceptionJournalService)
    {
        _exceptionJournalService = exceptionJournalService;
    }

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        var dto = await _exceptionJournalService.CreateAsync(new ExceptionJournalDto()
        {
            StackTrace = context.Exception.StackTrace,
            RequestParameters = context.HttpContext.Request.QueryString.Value,
            Type = context.Exception.GetType().Name,
            TraceIdentifier = context.HttpContext.Request.HttpContext.TraceIdentifier,
        });

        var errorDetails = new ErrorDetails()
        {
            Id = dto.TraceIdentifier,
            Type = dto.Type,
            Data = new ErrorDetailsData()
            {
                Message = context.Exception.Message
            }
        };

        var jsonResult = new JsonResult(errorDetails)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.Result = jsonResult;
    }
}