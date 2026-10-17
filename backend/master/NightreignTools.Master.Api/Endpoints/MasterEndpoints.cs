using NightreignTools.Master.Application.Masters;
using NightreignTools.ServiceDefaults.ErrorHandling;

namespace NightreignTools.Master.Api.Endpoints;

/// <summary>プレイヤー向けのマスタの取得(マスタの管理 4.1)。必要な役割はない。</summary>
public static class MasterEndpoints
{
    public static IEndpointRouteBuilder MapMasterEndpoints(this IEndpointRouteBuilder api)
    {
        api.MapGet("/bosses", (IMasterCatalogService service, CancellationToken ct) => service.GetBossesAsync(ct))
            .WithName("GetBosses").WithSummary("夜の王の一覧");

        api.MapGet("/jihen", (IMasterCatalogService service, CancellationToken ct) => service.GetJihenAsync(ct))
            .WithName("GetJihen").WithSummary("地変の一覧");

        api.MapGet("/spawn-points", (IMasterCatalogService service, CancellationToken ct) => service.GetSpawnPointsAsync(ct))
            .WithName("GetSpawnPoints").WithSummary("開始地点の一覧。非表示の行を除く");

        api.MapGet("/shrink-zones", (IMasterCatalogService service, CancellationToken ct) => service.GetShrinkZonesAsync(ct))
            .WithName("GetShrinkZones").WithSummary("収縮候補地の一覧。非表示の行を除く");

        api.MapGet("/second-shrink-points", (IMasterCatalogService service, CancellationToken ct) => service.GetSecondShrinkPointsAsync(ct))
            .WithName("GetSecondShrinkPoints").WithSummary("第二収縮地点の一覧。非表示の行を除く");

        api.MapGet("/base-points", (string? mapId, IMasterCatalogService service, CancellationToken ct) => service.GetBasePointsAsync(mapId, ct))
            .WithName("GetBasePoints").WithSummary("マップごとの拠点候補地の一覧。mapId を省くと通常のマップ")
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        api.MapGet("/night-bosses", (IMasterCatalogService service, CancellationToken ct) => service.GetNightBossesAsync(ct))
            .WithName("GetNightBosses").WithSummary("夜ボスの一覧");

        return api;
    }
}
