using NightreignTools.Master.Application.Admin;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Tests.Unit;

/// <summary>
/// 管理用の一覧、登録、修正の流れ(マスタの管理 4.2)。リポジトリは手書きの偽物に差し替える。
/// </summary>
public class MapPlaceAdminTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 18, 3, 0, 0, TimeSpan.Zero);

    private readonly FakeRepository _repository = new();
    private readonly FixedClock _clock = new(Now);

    [Fact]
    public async Task 登録すると保存して作った行を返し初めは表示中にする()
    {
        var admin = new SpawnPointAdmin(_repository, _clock);

        var created = await admin.CreateAsync(new SaveSpawnPointRequest("北東の教会", 351m, 387m, null), CancellationToken.None);

        Assert.Equal(("北東の教会", 351m, 387m, false), (created.NameJa, created.X, created.Y, created.IsHidden));
        Assert.Equal(1, _repository.SaveCount);
        Assert.Single(_repository.Places);
    }

    [Fact]
    public async Task 決まりに反する登録は保存しない()
    {
        var admin = new SpawnPointAdmin(_repository, _clock);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => admin.CreateAsync(new SaveSpawnPointRequest("北東の教会", 1025m, 1m, null), CancellationToken.None));

        Assert.Equal(0, _repository.SaveCount);
        Assert.Empty(_repository.Places);
    }

    [Fact]
    public async Task AC_ADM_03_ない行を直すと404にする()
    {
        var admin = new ShrinkZoneAdmin(_repository, _clock);

        var error = await Assert.ThrowsAsync<NotFoundException>(
            () => admin.UpdateAsync(99, new SaveShrinkZoneRequest("最北", null, 1m, 1m, 100m, null), CancellationToken.None));

        Assert.Equal("収縮候補地 '99' は存在しません", error.Message);
    }

    [Fact]
    public async Task AC_ADM_04_isHiddenを真にして直すと今の日時で非表示にする()
    {
        var point = _repository.Seed(SecondShrinkPoint.Create("最北", 1m, 1m));
        var admin = new SecondShrinkPointAdmin(_repository, _clock);

        var updated = await admin.UpdateAsync(point.Id, new SaveSecondShrinkPointRequest("最北", 1m, 1m, true), CancellationToken.None);

        Assert.True(updated.IsHidden);
        Assert.Equal(Now.UtcDateTime, point.HiddenAt);
    }

    [Fact]
    public async Task AC_ADM_05_isHiddenを偽にして直すと表示に戻す()
    {
        var point = _repository.Seed(SpawnPoint.Create("北東の教会", 1m, 1m));
        point.SetHidden(true, Now.UtcDateTime);
        var admin = new SpawnPointAdmin(_repository, _clock);

        var updated = await admin.UpdateAsync(point.Id, new SaveSpawnPointRequest("北東の教会", 1m, 1m, false), CancellationToken.None);

        Assert.False(updated.IsHidden);
    }

    [Fact]
    public async Task isHiddenを省いて直すと表示の状態を変えない()
    {
        var point = _repository.Seed(SpawnPoint.Create("北東の教会", 1m, 1m));
        point.SetHidden(true, Now.UtcDateTime);
        var admin = new SpawnPointAdmin(_repository, _clock);

        var updated = await admin.UpdateAsync(point.Id, new SaveSpawnPointRequest("南の教会", 2m, 2m, null), CancellationToken.None);

        Assert.Equal(("南の教会", true), (updated.NameJa, updated.IsHidden));
    }

    [Fact]
    public async Task 一覧には非表示の行も入れる()
    {
        _repository.Seed(SpawnPoint.Create("北東の教会", 1m, 1m));
        _repository.Seed(SpawnPoint.Create("南の教会", 2m, 2m)).SetHidden(true, Now.UtcDateTime);
        var admin = new SpawnPointAdmin(_repository, _clock);

        var list = await admin.ListAsync(CancellationToken.None);

        Assert.Equal([false, true], list.Select(p => p.IsHidden));
    }

    [Fact]
    public async Task 拠点候補地のmapIdを省くと通常のマップにする()
    {
        var admin = new BasePointAdmin(_repository, _clock);

        var created = await admin.CreateAsync(new SaveBasePointRequest(null, null, 1m, 1m, null), CancellationToken.None);

        Assert.Equal(BasePoint.NormalMapId, created.MapId);
    }

    [Fact]
    public async Task 拠点候補地のmapIdがない地変なら400にして保存しない()
    {
        var admin = new BasePointAdmin(_repository, _clock);

        var error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => admin.CreateAsync(new SaveBasePointRequest("unknown", null, 1m, 1m, null), CancellationToken.None));

        Assert.Equal("mapId 'unknown' は normal か地変の識別子にしてください", error.Message);
        Assert.Equal(0, _repository.SaveCount);
    }

    [Fact]
    public async Task 拠点候補地のmapIdにある地変を指定できる()
    {
        _repository.JihenIds.Add("crater");
        var admin = new BasePointAdmin(_repository, _clock);

        var created = await admin.CreateAsync(new SaveBasePointRequest("crater", "火口の拠点", 1m, 1m, null), CancellationToken.None);

        Assert.Equal("crater", created.MapId);
    }

    [Fact]
    public async Task 必須の数を省くと400にする()
    {
        var admin = new SpawnPointAdmin(_repository, _clock);

        var error = await Assert.ThrowsAsync<BusinessRuleException>(
            () => admin.CreateAsync(new SaveSpawnPointRequest("北東の教会", null, 1m, null), CancellationToken.None));

        Assert.Equal("x は必須です", error.Message);
    }

    private sealed class FakeRepository : IMapPlaceAdminRepository
    {
        private int _nextId = 1001;

        public List<MapPlace> Places { get; } = [];

        public HashSet<string> JihenIds { get; } = [];

        public int SaveCount { get; private set; }

        /// <summary>
        /// 保存済みの行として置く。
        /// 識別子はデータベースの代わりに振る
        /// </summary>
        /// <typeparam name="TPlace">場所の種類。</typeparam>
        /// <param name="place">置く行。</param>
        /// <returns>識別子を振った行。</returns>
        public TPlace Seed<TPlace>(TPlace place) where TPlace : MapPlace
        {
            Add(place);
            return place;
        }

        public Task<IReadOnlyList<TPlace>> ListAsync<TPlace>(CancellationToken cancellationToken) where TPlace : MapPlace =>
            Task.FromResult<IReadOnlyList<TPlace>>(Places.OfType<TPlace>().OrderBy(p => p.Id).ToList());

        public Task<TPlace?> FindAsync<TPlace>(int id, CancellationToken cancellationToken) where TPlace : MapPlace =>
            Task.FromResult(Places.OfType<TPlace>().SingleOrDefault(p => p.Id == id));

        public void Add<TPlace>(TPlace place) where TPlace : MapPlace
        {
            typeof(MapPlace).GetProperty(nameof(MapPlace.Id))!.SetValue(place, _nextId++);
            Places.Add(place);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCount++;
            return Task.CompletedTask;
        }

        public Task<bool> JihenExistsAsync(string jihenId, CancellationToken cancellationToken) =>
            Task.FromResult(JihenIds.Contains(jihenId));
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
