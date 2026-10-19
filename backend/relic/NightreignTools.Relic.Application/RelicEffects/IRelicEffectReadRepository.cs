using NightreignTools.Relic.Domain.Entities;

namespace NightreignTools.Relic.Application.RelicEffects;

/// <summary>
/// 遺物の効果のマスタを読むためのポート。実装は Infrastructure に置く(サービスの共通構成 5.3)。
/// </summary>
public interface IRelicEffectReadRepository
{
    Task<IReadOnlyList<RelicEffectCategory>> GetCategoriesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 効果を、ランクごとの値を含めて返す。
    /// </summary>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>すべての効果。並び順は決めない。</returns>
    Task<IReadOnlyList<RelicEffect>> GetEffectsAsync(CancellationToken cancellationToken);
}
