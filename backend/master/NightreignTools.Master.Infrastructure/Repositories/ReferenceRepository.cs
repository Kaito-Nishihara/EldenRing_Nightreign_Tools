using Microsoft.EntityFrameworkCore;
using NightreignTools.Master.Application.References;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence;

namespace NightreignTools.Master.Infrastructure.Repositories;

/// <summary>識別子でマスタを引く。非表示の行も返す。読むだけなので、変更の追跡をしない。</summary>
internal sealed class ReferenceRepository(MasterDbContext db) : IReferenceRepository
{
    public async Task<IReadOnlyList<Boss>> FindBossesAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken) =>
        await db.Bosses.AsNoTracking().Where(b => ids.Contains(b.Id)).OrderBy(b => b.SortOrder).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Jihen>> FindJihenAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken) =>
        await db.Jihen.AsNoTracking().Where(j => ids.Contains(j.Id)).OrderBy(j => j.SortOrder).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SpawnPoint>> FindSpawnPointsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) =>
        await db.SpawnPoints.AsNoTracking().Where(p => ids.Contains(p.Id)).OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ShrinkZone>> FindShrinkZonesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) =>
        await db.ShrinkZones.AsNoTracking().Where(z => ids.Contains(z.Id)).OrderBy(z => z.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SecondShrinkPoint>> FindSecondShrinkPointsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) =>
        await db.SecondShrinkPoints.AsNoTracking().Where(p => ids.Contains(p.Id)).OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<NightBoss>> FindNightBossesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) =>
        await db.NightBosses.AsNoTracking().Where(n => ids.Contains(n.Id)).OrderBy(n => n.Id).ToListAsync(cancellationToken);
}
