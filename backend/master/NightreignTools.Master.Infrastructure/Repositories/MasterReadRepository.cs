using Microsoft.EntityFrameworkCore;
using NightreignTools.Master.Application.Masters;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence;

namespace NightreignTools.Master.Infrastructure.Repositories;

/// <summary>
/// マスタを読む。
/// 読むだけなので、どのクエリも変更の追跡をしない。
/// </summary>
/// <param name="db">master-service のデータベース。</param>
internal sealed class MasterReadRepository(MasterDbContext db) : IMasterReadRepository
{
    public async Task<IReadOnlyList<Boss>> GetBossesAsync(CancellationToken cancellationToken) =>
        await db.Bosses.AsNoTracking().Include(b => b.Weaknesses)
            .OrderBy(b => b.SortOrder).ThenBy(b => b.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Jihen>> GetJihenAsync(CancellationToken cancellationToken) =>
        await db.Jihen.AsNoTracking().OrderBy(j => j.SortOrder).ThenBy(j => j.Id).ToListAsync(cancellationToken);

    public Task<bool> JihenExistsAsync(string jihenId, CancellationToken cancellationToken) =>
        db.Jihen.AnyAsync(j => j.Id == jihenId, cancellationToken);

    public async Task<IReadOnlyList<NightBoss>> GetNightBossesAsync(CancellationToken cancellationToken) =>
        await db.NightBosses.AsNoTracking().OrderBy(n => n.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SpawnPoint>> GetVisibleSpawnPointsAsync(CancellationToken cancellationToken) =>
        await db.SpawnPoints.AsNoTracking().Where(p => p.HiddenAt == null).OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ShrinkZone>> GetVisibleShrinkZonesAsync(CancellationToken cancellationToken) =>
        await db.ShrinkZones.AsNoTracking().Where(z => z.HiddenAt == null).OrderBy(z => z.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SecondShrinkPoint>> GetVisibleSecondShrinkPointsAsync(CancellationToken cancellationToken) =>
        await db.SecondShrinkPoints.AsNoTracking().Where(p => p.HiddenAt == null).OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<BasePoint>> GetVisibleBasePointsAsync(string mapId, CancellationToken cancellationToken) =>
        await db.BasePoints.AsNoTracking()
            .Where(p => p.MapId == mapId && p.HiddenAt == null)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
}
