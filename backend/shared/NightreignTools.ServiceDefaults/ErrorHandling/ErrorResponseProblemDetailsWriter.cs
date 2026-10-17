using Microsoft.AspNetCore.Http;

namespace NightreignTools.ServiceDefaults.ErrorHandling;

/// <summary>
/// ASP.NET Core が ProblemDetails で返す誤り(主に .NET 10 の入力の検証)を、<see cref="ErrorResponse"/> の形に書き替える(API の共通仕様 4.3)。
/// 入力の検証の誤りが複数あるときは、文言を読点でつなぐ。
/// </summary>
public sealed class ErrorResponseProblemDetailsWriter : IProblemDetailsWriter
{
    public bool CanWrite(ProblemDetailsContext context) => true;

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        var http = context.HttpContext;
        var problem = context.ProblemDetails;
        var status = problem.Status ?? http.Response.StatusCode;

        var message = problem is HttpValidationProblemDetails validation && validation.Errors.Count > 0
            ? string.Join("、", validation.Errors.SelectMany(e => e.Value).Distinct())
            : problem.Detail ?? ApiExceptionHandler.BadRequestMessage;

        return new ValueTask(ErrorResponseWriter.WriteAsync(http, status, message, http.RequestAborted));
    }
}
