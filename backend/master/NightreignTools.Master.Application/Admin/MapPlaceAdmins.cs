using NightreignTools.Master.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Application.Admin;

// 種類ごとの管理(マスタの管理 4.2)。共通の流れは MapPlaceAdmin にあり、ここには種類ごとの違いだけを書く

/// <summary>開始地点の管理</summary>
internal sealed class SpawnPointAdmin(IMapPlaceAdminRepository repository, TimeProvider clock)
    : MapPlaceAdmin<SpawnPoint, SaveSpawnPointRequest, AdminSpawnPointDto>(repository, clock)
{
    protected override string Label => "開始地点";

    protected override SpawnPoint Create(SaveSpawnPointRequest r) =>
        SpawnPoint.Create(r.NameJa, Required(r.X, "x"), Required(r.Y, "y"));

    protected override void Update(SpawnPoint place, SaveSpawnPointRequest r) =>
        place.Update(r.NameJa, Required(r.X, "x"), Required(r.Y, "y"));

    protected override bool? IsHidden(SaveSpawnPointRequest r) => r.IsHidden;

    protected override AdminSpawnPointDto ToDto(SpawnPoint p) => new(p.Id, p.NameJa, p.X, p.Y, p.IsHidden);
}

/// <summary>収縮候補地の管理</summary>
internal sealed class ShrinkZoneAdmin(IMapPlaceAdminRepository repository, TimeProvider clock)
    : MapPlaceAdmin<ShrinkZone, SaveShrinkZoneRequest, AdminShrinkZoneDto>(repository, clock)
{
    protected override string Label => "収縮候補地";

    protected override ShrinkZone Create(SaveShrinkZoneRequest r) =>
        ShrinkZone.Create(r.NameJa, r.IconPath, Required(r.CenterX, "centerX"), Required(r.CenterY, "centerY"), Required(r.Radius, "radius"));

    protected override void Update(ShrinkZone place, SaveShrinkZoneRequest r) =>
        place.Update(r.NameJa, r.IconPath, Required(r.CenterX, "centerX"), Required(r.CenterY, "centerY"), Required(r.Radius, "radius"));

    protected override bool? IsHidden(SaveShrinkZoneRequest r) => r.IsHidden;

    protected override AdminShrinkZoneDto ToDto(ShrinkZone z) => new(z.Id, z.NameJa, z.IconPath, z.CenterX, z.CenterY, z.Radius, z.IsHidden);
}

/// <summary>第二収縮地点の管理</summary>
internal sealed class SecondShrinkPointAdmin(IMapPlaceAdminRepository repository, TimeProvider clock)
    : MapPlaceAdmin<SecondShrinkPoint, SaveSecondShrinkPointRequest, AdminSecondShrinkPointDto>(repository, clock)
{
    protected override string Label => "第二収縮地点";

    protected override SecondShrinkPoint Create(SaveSecondShrinkPointRequest r) =>
        SecondShrinkPoint.Create(r.NameJa, Required(r.X, "x"), Required(r.Y, "y"));

    protected override void Update(SecondShrinkPoint place, SaveSecondShrinkPointRequest r) =>
        place.Update(r.NameJa, Required(r.X, "x"), Required(r.Y, "y"));

    protected override bool? IsHidden(SaveSecondShrinkPointRequest r) => r.IsHidden;

    protected override AdminSecondShrinkPointDto ToDto(SecondShrinkPoint p) => new(p.Id, p.NameJa, p.X, p.Y, p.IsHidden);
}

/// <summary>拠点候補地の管理。マップは通常のマップか、ある地変の識別子に限る(マスタの管理 3.4)</summary>
internal sealed class BasePointAdmin(IMapPlaceAdminRepository repository, TimeProvider clock)
    : MapPlaceAdmin<BasePoint, SaveBasePointRequest, AdminBasePointDto>(repository, clock)
{
    protected override string Label => "拠点候補地";

    protected override async Task ValidateAsync(SaveBasePointRequest r, CancellationToken cancellationToken)
    {
        var mapId = MapId(r);
        if (mapId != BasePoint.NormalMapId && !await Repository.JihenExistsAsync(mapId, cancellationToken))
        {
            throw new BusinessRuleException($"mapId '{mapId}' は normal か地変の識別子にしてください");
        }
    }

    protected override BasePoint Create(SaveBasePointRequest r) =>
        BasePoint.Create(MapId(r), r.NameJa, Required(r.X, "x"), Required(r.Y, "y"));

    protected override void Update(BasePoint place, SaveBasePointRequest r) =>
        place.Update(MapId(r), r.NameJa, Required(r.X, "x"), Required(r.Y, "y"));

    protected override bool? IsHidden(SaveBasePointRequest r) => r.IsHidden;

    protected override AdminBasePointDto ToDto(BasePoint p) => new(p.Id, p.MapId, p.NameJa, p.X, p.Y, p.IsHidden);

    /// <summary>省いたときは通常のマップ</summary>
    private static string MapId(SaveBasePointRequest r) =>
        string.IsNullOrWhiteSpace(r.MapId) ? BasePoint.NormalMapId : r.MapId.Trim();
}
