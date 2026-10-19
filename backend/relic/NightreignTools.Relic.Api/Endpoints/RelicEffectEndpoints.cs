using NightreignTools.Relic.Application.RelicEffects;

namespace NightreignTools.Relic.Api.Endpoints;

/// <summary>
/// 遺物の効果の取得(ビルドシミュレーター 4 章)。
/// 必要な役割はない。
/// </summary>
public static class RelicEffectEndpoints
{
    public static IEndpointRouteBuilder MapRelicEffectEndpoints(this IEndpointRouteBuilder api)
    {
        api.MapGet("/relic-effects", (IRelicEffectCatalogService service, CancellationToken ct) => service.GetEffectsAsync(ct))
            .WithName("GetRelicEffects").WithSummary("遺物の効果の一覧。分類の並び順、効果の識別子の順");

        return api;
    }
}
