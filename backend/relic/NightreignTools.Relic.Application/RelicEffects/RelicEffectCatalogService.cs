namespace NightreignTools.Relic.Application.RelicEffects;

/// <summary>
/// 遺物の効果の一覧の取得。
/// 分類を効果に付け、エンティティを DTO に詰め替える。
/// </summary>
/// <param name="repository">分類と効果を読む。</param>
internal sealed class RelicEffectCatalogService(IRelicEffectReadRepository repository) : IRelicEffectCatalogService
{
    public async Task<IReadOnlyList<RelicEffectDto>> GetEffectsAsync(CancellationToken cancellationToken)
    {
        var categories = (await repository.GetCategoriesAsync(cancellationToken)).ToDictionary(c => c.Id);
        var effects = await repository.GetEffectsAsync(cancellationToken);

        // 分類は外部キーで必ずあるので、引けないときはデータの壊れとして 500 にする
        return effects
            .Select(e => (Effect: e, Category: categories[e.CategoryId]))
            .OrderBy(x => x.Category.SortOrder).ThenBy(x => x.Effect.Id)
            .Select(x => new RelicEffectDto(
                x.Effect.Id,
                x.Category.Id,
                x.Category.NameJa,
                x.Category.SortOrder,
                x.Effect.NameJa,
                x.Effect.IsDeep,
                x.Effect.EffectSign,
                x.Effect.CalcType,
                x.Effect.Attribute,
                x.Effect.Stackable,
                x.Effect.RankMin,
                x.Effect.RankMax,
                x.Effect.ConditionJa,
                x.Effect.NotesJa,
                x.Effect.Ranks.OrderBy(r => r.Rank).Select(r => new RelicEffectRankDto(r.Rank, r.Value, r.Unit, r.HasDemerit)).ToList()))
            .ToList();
    }
}
