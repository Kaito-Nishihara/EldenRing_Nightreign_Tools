using NightreignTools.Master.Application.Admin;

namespace NightreignTools.Master.Api.Endpoints;

/// <summary>
/// 管理用の API(マスタの管理 4.2)。管理用の鍵は Gateway が確かめるので、ここでは確かめない(決定の記録 0009)。
/// 行を消す API は作らない(決定の記録 0013)。
/// 4 つの種類を型引数で 1 つにまとめて登録すると、ASP.NET Core のルートの解析器が落ちてビルドが通らないので、種類ごとに具体的な型で登録する。
/// </summary>
public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder api)
    {
        var admin = api.MapGroup("/admin");

        var spawns = admin.MapGroup("/spawn-points");
        spawns.MapGet("/", (IMapPlaceAdmin<SaveSpawnPointRequest, AdminSpawnPointDto> s, CancellationToken ct) => s.ListAsync(ct));
        spawns.MapPost("/", async (SaveSpawnPointRequest request, IMapPlaceAdmin<SaveSpawnPointRequest, AdminSpawnPointDto> s, CancellationToken ct) =>
            TypedResults.Created((string?)null, await s.CreateAsync(request, ct)));
        spawns.MapPut("/{id:int}", (int id, SaveSpawnPointRequest request, IMapPlaceAdmin<SaveSpawnPointRequest, AdminSpawnPointDto> s, CancellationToken ct) =>
            s.UpdateAsync(id, request, ct));

        var zones = admin.MapGroup("/shrink-zones");
        zones.MapGet("/", (IMapPlaceAdmin<SaveShrinkZoneRequest, AdminShrinkZoneDto> s, CancellationToken ct) => s.ListAsync(ct));
        zones.MapPost("/", async (SaveShrinkZoneRequest request, IMapPlaceAdmin<SaveShrinkZoneRequest, AdminShrinkZoneDto> s, CancellationToken ct) =>
            TypedResults.Created((string?)null, await s.CreateAsync(request, ct)));
        zones.MapPut("/{id:int}", (int id, SaveShrinkZoneRequest request, IMapPlaceAdmin<SaveShrinkZoneRequest, AdminShrinkZoneDto> s, CancellationToken ct) =>
            s.UpdateAsync(id, request, ct));

        var points = admin.MapGroup("/second-shrink-points");
        points.MapGet("/", (IMapPlaceAdmin<SaveSecondShrinkPointRequest, AdminSecondShrinkPointDto> s, CancellationToken ct) => s.ListAsync(ct));
        points.MapPost("/", async (SaveSecondShrinkPointRequest request, IMapPlaceAdmin<SaveSecondShrinkPointRequest, AdminSecondShrinkPointDto> s, CancellationToken ct) =>
            TypedResults.Created((string?)null, await s.CreateAsync(request, ct)));
        points.MapPut("/{id:int}", (int id, SaveSecondShrinkPointRequest request, IMapPlaceAdmin<SaveSecondShrinkPointRequest, AdminSecondShrinkPointDto> s, CancellationToken ct) =>
            s.UpdateAsync(id, request, ct));

        var bases = admin.MapGroup("/base-points");
        bases.MapGet("/", (IMapPlaceAdmin<SaveBasePointRequest, AdminBasePointDto> s, CancellationToken ct) => s.ListAsync(ct));
        bases.MapPost("/", async (SaveBasePointRequest request, IMapPlaceAdmin<SaveBasePointRequest, AdminBasePointDto> s, CancellationToken ct) =>
            TypedResults.Created((string?)null, await s.CreateAsync(request, ct)));
        bases.MapPut("/{id:int}", (int id, SaveBasePointRequest request, IMapPlaceAdmin<SaveBasePointRequest, AdminBasePointDto> s, CancellationToken ct) =>
            s.UpdateAsync(id, request, ct));

        return api;
    }
}
