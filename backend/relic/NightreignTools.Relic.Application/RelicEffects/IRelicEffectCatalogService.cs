namespace NightreignTools.Relic.Application.RelicEffects;

/// <summary>遺物の効果の一覧の取得(ビルドシミュレーター 4 章)。</summary>
public interface IRelicEffectCatalogService
{
    /// <summary>効果を、分類の並び順、効果の識別子の順に返す。ランクごとの値は、ランクの小さい順に並べる。</summary>
    Task<IReadOnlyList<RelicEffectDto>> GetEffectsAsync(CancellationToken cancellationToken);
}
