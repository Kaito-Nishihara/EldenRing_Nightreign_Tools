namespace NightreignTools.Master.Application.References;

/// <summary>マスタの参照の解決(マスタの管理 4.4)。</summary>
public interface IReferenceResolver
{
    Task<ResolvedReferences> ResolveAsync(ResolveReferencesRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// 識別子をまとめて引き、見つかった行を返す。足りないものを 404 にするかは、呼ぶ側(record-service)が決める。
/// 空の配列と省いた配列は、データベースに問い合わせずに空で返す。
/// </summary>
internal sealed class ReferenceResolver(IReferenceRepository repository) : IReferenceResolver
{
    public async Task<ResolvedReferences> ResolveAsync(ResolveReferencesRequest request, CancellationToken cancellationToken)
    {
        var bosses = await FindAsync(request.BossIds, repository.FindBossesAsync, cancellationToken);
        var jihen = await FindAsync(request.JihenIds, repository.FindJihenAsync, cancellationToken);
        var spawns = await FindAsync(request.SpawnPointIds, repository.FindSpawnPointsAsync, cancellationToken);
        var zones = await FindAsync(request.ZoneIds, repository.FindShrinkZonesAsync, cancellationToken);
        var points = await FindAsync(request.SecondShrinkPointIds, repository.FindSecondShrinkPointsAsync, cancellationToken);
        var nightBosses = await FindAsync(request.NightBossIds, repository.FindNightBossesAsync, cancellationToken);

        return new ResolvedReferences(
            bosses.Select(b => new ResolvedBoss(b.Id, b.NameJa)).ToList(),
            jihen.Select(j => new ResolvedJihen(j.Id, j.NameJa)).ToList(),
            spawns.Select(p => new ResolvedPoint(p.Id, p.NameJa, p.X, p.Y, p.IsHidden)).ToList(),
            zones.Select(z => new ResolvedZone(z.Id, z.NameJa, z.CenterX, z.CenterY, z.Radius, z.IsHidden)).ToList(),
            points.Select(p => new ResolvedPoint(p.Id, p.NameJa, p.X, p.Y, p.IsHidden)).ToList(),
            nightBosses.Select(n => new ResolvedNightBoss(n.Id, n.NameJa, n.Day)).ToList());
    }

    /// <summary>重なりを除いてから引く。何も渡されていなければ、問い合わせずに空を返す。</summary>
    private static async Task<IReadOnlyList<TEntity>> FindAsync<TId, TEntity>(
        IReadOnlyList<TId>? ids,
        Func<IReadOnlyCollection<TId>, CancellationToken, Task<IReadOnlyList<TEntity>>> find,
        CancellationToken cancellationToken)
    {
        var distinct = ids?.Distinct().ToList() ?? [];
        return distinct.Count == 0 ? [] : await find(distinct, cancellationToken);
    }
}
