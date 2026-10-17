using NightreignTools.Master.Application.Masters;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Tests.Unit;

/// <summary>
/// プレイヤー向けのマスタの取得の規則(マスタの管理 4.1)。リポジトリは手書きの偽物に差し替える。
/// </summary>
public class MasterCatalogServiceTests
{
    [Fact]
    public async Task 拠点候補地のmapIdを省くと通常のマップを引く()
    {
        var repository = new FakeRepository();
        var service = new MasterCatalogService(repository);

        await service.GetBasePointsAsync(null, CancellationToken.None);

        Assert.Equal(BasePoint.NormalMapId, repository.RequestedMapId);
    }

    [Fact]
    public async Task 拠点候補地のmapIdが地変の識別子ならその地変を引く()
    {
        var repository = new FakeRepository { ExistingJihen = ["crater"] };
        var service = new MasterCatalogService(repository);

        await service.GetBasePointsAsync("crater", CancellationToken.None);

        Assert.Equal("crater", repository.RequestedMapId);
    }

    [Fact]
    public async Task AC_ADM_21_拠点候補地のmapIdが通常でも地変でもなければNotFoundExceptionを投げる()
    {
        var service = new MasterCatalogService(new FakeRepository());

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.GetBasePointsAsync("unknown", CancellationToken.None));

        Assert.Equal("mapId 'unknown' は存在しません", exception.Message);
    }

    private sealed class FakeRepository : IMasterReadRepository
    {
        public string[] ExistingJihen { get; init; } = [];

        public string? RequestedMapId { get; private set; }

        public Task<bool> JihenExistsAsync(string jihenId, CancellationToken cancellationToken) =>
            Task.FromResult(ExistingJihen.Contains(jihenId));

        public Task<IReadOnlyList<BasePoint>> GetVisibleBasePointsAsync(string mapId, CancellationToken cancellationToken)
        {
            RequestedMapId = mapId;
            return Task.FromResult<IReadOnlyList<BasePoint>>([]);
        }

        public Task<IReadOnlyList<Boss>> GetBossesAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Boss>>([]);
        public Task<IReadOnlyList<Jihen>> GetJihenAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Jihen>>([]);
        public Task<IReadOnlyList<NightBoss>> GetNightBossesAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<NightBoss>>([]);
        public Task<IReadOnlyList<SpawnPoint>> GetVisibleSpawnPointsAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<SpawnPoint>>([]);
        public Task<IReadOnlyList<ShrinkZone>> GetVisibleShrinkZonesAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<ShrinkZone>>([]);
        public Task<IReadOnlyList<SecondShrinkPoint>> GetVisibleSecondShrinkPointsAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<SecondShrinkPoint>>([]);
    }
}
