using NightreignTools.Master.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Application.Masters;

/// <summary>プレイヤー向けのマスタの取得。エンティティを DTO に詰め替える。</summary>
internal sealed class MasterCatalogService(IMasterReadRepository repository) : IMasterCatalogService
{
    public async Task<IReadOnlyList<BossDto>> GetBossesAsync(CancellationToken cancellationToken) =>
        (await repository.GetBossesAsync(cancellationToken))
            .Select(b => new BossDto(
                b.Id, b.NameJa, b.NameEn, b.ImagePath,
                b.Weaknesses.OrderBy(w => w.Id).Select(w => w.Attribute).ToList(),
                b.IsDlc, b.UnlockCondition))
            .ToList();

    public async Task<IReadOnlyList<JihenDto>> GetJihenAsync(CancellationToken cancellationToken) =>
        (await repository.GetJihenAsync(cancellationToken))
            .Select(j => new JihenDto(j.Id, j.NameJa, j.ImagePath, j.Area, j.CenterX, j.CenterY, j.IsDlc, j.UnlockCondition))
            .ToList();

    public async Task<IReadOnlyList<SpawnPointDto>> GetSpawnPointsAsync(CancellationToken cancellationToken) =>
        (await repository.GetVisibleSpawnPointsAsync(cancellationToken))
            .Select(p => new SpawnPointDto(p.Id, p.NameJa, p.X, p.Y))
            .ToList();

    public async Task<IReadOnlyList<ShrinkZoneDto>> GetShrinkZonesAsync(CancellationToken cancellationToken) =>
        (await repository.GetVisibleShrinkZonesAsync(cancellationToken))
            .Select(z => new ShrinkZoneDto(z.Id, z.NameJa, z.IconPath, z.CenterX, z.CenterY, z.Radius))
            .ToList();

    public async Task<IReadOnlyList<SecondShrinkPointDto>> GetSecondShrinkPointsAsync(CancellationToken cancellationToken) =>
        (await repository.GetVisibleSecondShrinkPointsAsync(cancellationToken))
            .Select(p => new SecondShrinkPointDto(p.Id, p.NameJa, p.X, p.Y))
            .ToList();

    public async Task<IReadOnlyList<BasePointDto>> GetBasePointsAsync(string? mapId, CancellationToken cancellationToken)
    {
        var map = string.IsNullOrWhiteSpace(mapId) ? BasePoint.NormalMapId : mapId.Trim();
        if (map != BasePoint.NormalMapId && !await repository.JihenExistsAsync(map, cancellationToken))
        {
            throw new NotFoundException($"mapId '{map}' は存在しません");
        }

        return (await repository.GetVisibleBasePointsAsync(map, cancellationToken))
            .Select(p => new BasePointDto(p.Id, p.MapId, p.NameJa, p.X, p.Y))
            .ToList();
    }

    public async Task<IReadOnlyList<NightBossDto>> GetNightBossesAsync(CancellationToken cancellationToken) =>
        (await repository.GetNightBossesAsync(cancellationToken))
            .Select(n => new NightBossDto(n.Id, n.NameJa, n.Day))
            .ToList();
}
