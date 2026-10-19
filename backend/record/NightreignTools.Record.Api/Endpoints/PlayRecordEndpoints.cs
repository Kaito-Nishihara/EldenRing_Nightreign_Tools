using NightreignTools.Record.Application.PlayRecords;
using NightreignTools.ServiceDefaults.ErrorHandling;

namespace NightreignTools.Record.Api.Endpoints;

/// <summary>
/// プレイ記録の API(プレイ記録 4 章)。
/// /api/v1 のグループに足す。
/// </summary>
public static class PlayRecordEndpoints
{
    public static IEndpointRouteBuilder MapPlayRecordEndpoints(this IEndpointRouteBuilder api)
    {
        api.MapPost("/play-records", async (CreatePlayRecordRequest request, IPlayRecordService service, CancellationToken ct) =>
            {
                var created = await service.CreateAsync(request, ct);
                // 記録を 1 件取る API は持たないので、場所の見出しは付けない
                return TypedResults.Created((string?)null, created);
            })
            .WithName("CreatePlayRecord")
            .WithSummary("プレイ記録を保存する。参照は master-service で確かめる(プレイ記録 4 章)")
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        return api;
    }
}
