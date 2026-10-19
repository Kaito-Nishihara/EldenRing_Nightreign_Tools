using NightreignTools.Record.Application.Patterns;
using NightreignTools.ServiceDefaults.ErrorHandling;

namespace NightreignTools.Record.Api.Endpoints;

/// <summary>
/// パターン検索の API(パターン検索 4 章)。
/// /api/v1 のグループに足す。
/// </summary>
public static class PatternEndpoints
{
    public static IEndpointRouteBuilder MapPatternEndpoints(this IEndpointRouteBuilder api)
    {
        api.MapGet("/patterns",
                ([AsParameters] PatternSearchRequest request, IPatternSearchService service, CancellationToken ct) => service.SearchAsync(request, ct))
            .WithName("SearchPatterns")
            .WithSummary("同じ条件の記録をパターンにまとめて返す。記録がなくても 200(パターン検索 4 章)")
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        return api;
    }
}
