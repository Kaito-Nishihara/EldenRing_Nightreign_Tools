using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Application.Masters;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Tests.Integration;

/// <summary>
/// 登録した master-service のクライアントに、呼び先、サービス間キー、再試行が付いていること(サービスの共通構成 5.5)。
/// 送り出しの手前だけを偽物に差し替え、登録の組み立てはそのまま使う。
/// </summary>
public class MasterDirectoryRegistrationTests
{
    private static readonly MasterReferenceQuery Query = new(["gladius"], [], [1], [], [], []);

    [Fact]
    public async Task 呼び先とサービス間キーを付けて呼ぶ()
    {
        var handler = StubHttpHandler.Returning(HttpStatusCode.OK,
            """{ "bosses": [], "jihen": [], "spawnPoints": [], "zones": [], "secondShrinkPoints": [], "nightBosses": [] }""");
        using var factory = new FactoryWithHandler(handler);

        await ResolveAsync(factory);

        var request = Assert.Single(handler.Requests);
        Assert.Equal($"{RecordServiceFactory.MasterServiceUrl}/internal/v1/references/resolve", request.RequestUri!.ToString());
        Assert.Equal(RecordServiceFactory.ServiceKey, request.Headers.GetValues("X-Service-Key").Single());
    }

    [Fact]
    public async Task AC_REC_23_5xxが続くと2回まで再試行してから503の例外にする()
    {
        var handler = StubHttpHandler.Returning(HttpStatusCode.ServiceUnavailable);
        using var factory = new FactoryWithHandler(handler);

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => ResolveAsync(factory));

        // 最初の 1 回と、再試行の 2 回
        Assert.Equal(3, handler.Requests.Count);
    }

    [Fact]
    public async Task 応答が4xxなら再試行しない()
    {
        var handler = StubHttpHandler.Returning(HttpStatusCode.Unauthorized);
        using var factory = new FactoryWithHandler(handler);

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => ResolveAsync(factory));

        Assert.Single(handler.Requests);
    }

    private static async Task<MasterReferences> ResolveAsync(RecordServiceFactory factory)
    {
        factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMasterDirectory>().ResolveAsync(Query, CancellationToken.None);
    }

    /// <summary>
    /// master-service のクライアントの送り出しを、偽物に差し替えて起動する。
    /// </summary>
    /// <param name="handler">master-service のクライアントの送り出しに使う偽物。</param>
    private sealed class FactoryWithHandler(StubHttpHandler handler) : RecordServiceFactory
    {
        protected override void ConfigureTestServices(IServiceCollection services) =>
            services.AddHttpClient<IMasterDirectory, NightreignTools.Record.Infrastructure.Masters.MasterDirectoryClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler);
    }
}
