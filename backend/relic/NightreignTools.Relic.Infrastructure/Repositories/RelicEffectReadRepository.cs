using Microsoft.EntityFrameworkCore;
using NightreignTools.Relic.Application.RelicEffects;
using NightreignTools.Relic.Domain.Entities;
using NightreignTools.Relic.Infrastructure.Persistence;

namespace NightreignTools.Relic.Infrastructure.Repositories;

/// <summary>
/// 遺物の効果のマスタを読む。
/// 読むだけなので、どのクエリも変更の追跡をしない。
/// </summary>
/// <param name="db">relic-service のデータベース。</param>
internal sealed class RelicEffectReadRepository(RelicDbContext db) : IRelicEffectReadRepository
{
    public async Task<IReadOnlyList<RelicEffectCategory>> GetCategoriesAsync(CancellationToken cancellationToken) =>
        await db.RelicEffectCategories.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RelicEffect>> GetEffectsAsync(CancellationToken cancellationToken) =>
        await db.RelicEffects.AsNoTracking().Include(e => e.Ranks).ToListAsync(cancellationToken);
}
