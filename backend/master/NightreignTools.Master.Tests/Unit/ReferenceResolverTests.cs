using NightreignTools.Master.Application.References;
using NightreignTools.Master.Domain.Entities;

namespace NightreignTools.Master.Tests.Unit;

/// <summary>
/// マスタの参照の解決の規則(マスタの管理 4.4)。リポジトリは手書きの偽物に差し替える。
/// </summary>
public class ReferenceResolverTests
{
    [Fact]
    public async Task 配列を省くか空にすると問い合わせない()
    {
        var repository = new RecordingRepository();
        var resolver = new ReferenceResolver(repository);

        var result = await resolver.ResolveAsync(new ResolveReferencesRequest(ZoneIds: []), CancellationToken.None);

        Assert.Empty(repository.Calls);
        Assert.Empty(result.Zones);
    }

    [Fact]
    public async Task 同じ識別子は1回だけ問い合わせる()
    {
        var repository = new RecordingRepository();
        var resolver = new ReferenceResolver(repository);

        await resolver.ResolveAsync(new ResolveReferencesRequest(ZoneIds: [4, 4, 7], BossIds: ["gladius", "gladius"]), CancellationToken.None);

        Assert.Equal(["zones:4,7", "bosses:gladius"], repository.Calls.Order().Reverse().ToArray());
    }

    /// <summary>呼ばれた問い合わせを記録し、何も見つからなかったとして空を返す偽物。</summary>
    private sealed class RecordingRepository : IReferenceRepository
    {
        public List<string> Calls { get; } = [];

        private Task<IReadOnlyList<T>> Record<T, TId>(string name, IReadOnlyCollection<TId> ids)
        {
            Calls.Add(name + ":" + string.Join(",", ids));
            return Task.FromResult<IReadOnlyList<T>>([]);
        }

        public Task<IReadOnlyList<Boss>> FindBossesAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken) => Record<Boss, string>("bosses", ids);
        public Task<IReadOnlyList<Jihen>> FindJihenAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken) => Record<Jihen, string>("jihen", ids);
        public Task<IReadOnlyList<SpawnPoint>> FindSpawnPointsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) => Record<SpawnPoint, int>("spawns", ids);
        public Task<IReadOnlyList<ShrinkZone>> FindShrinkZonesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) => Record<ShrinkZone, int>("zones", ids);
        public Task<IReadOnlyList<SecondShrinkPoint>> FindSecondShrinkPointsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) => Record<SecondShrinkPoint, int>("points", ids);
        public Task<IReadOnlyList<NightBoss>> FindNightBossesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) => Record<NightBoss, int>("nightBosses", ids);
    }
}
