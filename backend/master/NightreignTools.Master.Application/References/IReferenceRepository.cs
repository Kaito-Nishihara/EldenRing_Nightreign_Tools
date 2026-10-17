using NightreignTools.Master.Domain.Entities;

namespace NightreignTools.Master.Application.References;

/// <summary>
/// 識別子でマスタを引くためのポート。非表示の行も返す(マスタの管理 4.4)。
/// 渡した識別子のうち、見つかった行だけを返す。
/// </summary>
public interface IReferenceRepository
{
    Task<IReadOnlyList<Boss>> FindBossesAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<Jihen>> FindJihenAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<SpawnPoint>> FindSpawnPointsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<ShrinkZone>> FindShrinkZonesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<SecondShrinkPoint>> FindSecondShrinkPointsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<NightBoss>> FindNightBossesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
}
