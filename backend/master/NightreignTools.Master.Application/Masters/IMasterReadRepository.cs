using NightreignTools.Master.Domain.Entities;

namespace NightreignTools.Master.Application.Masters;

/// <summary>
/// マスタを読むためのポート。実装は Infrastructure に置く(サービスの共通構成 5.3)。
/// 並びはどれも識別子の順とする(マスタの管理 4.1)。
/// </summary>
public interface IMasterReadRepository
{
    /// <summary>夜の王を、弱点を含めて返す。</summary>
    Task<IReadOnlyList<Boss>> GetBossesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Jihen>> GetJihenAsync(CancellationToken cancellationToken);

    Task<bool> JihenExistsAsync(string jihenId, CancellationToken cancellationToken);

    Task<IReadOnlyList<NightBoss>> GetNightBossesAsync(CancellationToken cancellationToken);

    // マップの上の場所は、非表示の行を除いて返す(決定の記録 0013)

    Task<IReadOnlyList<SpawnPoint>> GetVisibleSpawnPointsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ShrinkZone>> GetVisibleShrinkZonesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<SecondShrinkPoint>> GetVisibleSecondShrinkPointsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<BasePoint>> GetVisibleBasePointsAsync(string mapId, CancellationToken cancellationToken);
}
