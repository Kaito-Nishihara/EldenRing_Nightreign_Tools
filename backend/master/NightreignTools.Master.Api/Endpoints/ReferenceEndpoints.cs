using NightreignTools.Master.Application.References;
using NightreignTools.ServiceDefaults.ErrorHandling;

namespace NightreignTools.Master.Api.Endpoints;

/// <summary>
/// サービスの間だけで使う API(API の共通仕様 4.8)。/internal/v1 のグループに足す。Gateway は振り分けない。
/// </summary>
public static class ReferenceEndpoints
{
    public static IEndpointRouteBuilder MapReferenceEndpoints(this IEndpointRouteBuilder internalApi)
    {
        internalApi.MapPost("/references/resolve",
                (ResolveReferencesRequest request, IReferenceResolver resolver, CancellationToken ct) => resolver.ResolveAsync(request, ct))
            .WithName("ResolveReferences")
            .WithSummary("識別子でマスタを引く。非表示の行も返し、見つからない識別子は抜く(マスタの管理 4.4)")
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        return internalApi;
    }
}
