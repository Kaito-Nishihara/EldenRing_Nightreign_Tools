using NightreignTools.Record.Application.Patterns;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Tests.Unit;

/// <summary>
/// パターンのまとめ方と並び順、収縮先の名前と座標の入れ方(パターン検索 3.2、4 章)。
/// 記録の読み出しと master-service は手書きの偽物に差し替える。
/// </summary>
public class PatternSearchServiceTests
{
    private readonly FakeReader _reader = new();
    private readonly FakeMasterDirectory _masters = new();

    private static readonly DateTime Day0 = new(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);

    private static ShrinkTarget First(int day, int zoneId) => new(day, 1, zoneId, null);
    private static ShrinkTarget Second(int day, int pointId) => new(day, 2, null, pointId);

    private Task<PatternResponse> SearchAsync(string bossId = "gladius", int spawnPointId = 1, string? jihenId = null) =>
        new PatternSearchService(_reader, _masters).SearchAsync(new PatternSearchRequest(bossId, spawnPointId, jihenId), CancellationToken.None);

    [Fact]
    public async Task AC_PAT_01_同じ並びの記録3件と別の並び1件は2つのパターンになり1番目が3回()
    {
        _reader.Add(Day0, First(1, 4), First(2, 7));
        _reader.Add(Day0.AddDays(1), First(1, 4), First(2, 7));
        _reader.Add(Day0.AddDays(2), First(2, 7), First(1, 4));
        _reader.Add(Day0.AddDays(3), First(1, 7));

        var result = await SearchAsync();

        Assert.Equal(4, result.TotalRecords);
        Assert.Equal([3, 1], result.Patterns.Select(p => p.RecordCount));
        Assert.Equal(Day0.AddDays(2), result.Patterns[0].LastRecordedAt);
    }

    [Fact]
    public async Task AC_PAT_02_記録の数が同じなら最後の記録の新しい方が先に並ぶ()
    {
        _reader.Add(Day0, First(1, 4));
        _reader.Add(Day0.AddDays(5), First(1, 7));

        var result = await SearchAsync();

        Assert.Equal([7, 4], result.Patterns.Select(p => p.Entries[0].Zone!.Id));
    }

    [Fact]
    public async Task AC_PAT_03_patternIndexは並び順に1から振る()
    {
        _reader.Add(Day0, First(1, 4));
        _reader.Add(Day0, First(1, 7));
        _reader.Add(Day0, First(1, 7));
        _reader.Add(Day0, First(2, 4));
        _reader.Add(Day0, First(2, 4));
        _reader.Add(Day0, First(2, 4));

        var result = await SearchAsync();

        Assert.Equal([1, 2, 3], result.Patterns.Select(p => p.PatternIndex));
        Assert.Equal([3, 2, 1], result.Patterns.Select(p => p.RecordCount));
    }

    [Fact]
    public async Task AC_PAT_06_記録がなければ0件と空の一覧を返す()
    {
        var result = await SearchAsync();

        Assert.Equal(0, result.TotalRecords);
        Assert.Empty(result.Patterns);
    }

    [Fact]
    public async Task AC_PAT_07_第1次だけの記録と第2次まである記録は別のパターンになる()
    {
        _reader.Add(Day0, First(1, 4));
        _reader.Add(Day0, First(1, 4), Second(1, 4));

        var result = await SearchAsync();

        Assert.Equal(2, result.Patterns.Count);
    }

    [Fact]
    public async Task AC_PAT_15_第1次が同じで第2次の地点が違う記録は別のパターンになる()
    {
        _reader.Add(Day0, First(1, 4), Second(1, 4));
        _reader.Add(Day0, First(1, 4), Second(1, 9));
        _masters.ExtraPointIds.Add(9);

        var result = await SearchAsync();

        Assert.Equal(2, result.Patterns.Count);
    }

    [Fact]
    public async Task AC_PAT_16_第2次の収縮先はzoneがnullで第二収縮地点の名前と座標が入る()
    {
        _reader.Add(Day0, Second(1, 4), First(1, 4));

        var entries = (await SearchAsync()).Patterns[0].Entries;

        Assert.Equal([(1, 1), (1, 2)], entries.Select(e => (e.Day, e.Phase)));
        Assert.Null(entries[0].SecondShrinkPoint);
        Assert.Equal(new PatternZoneDto(4, "候補4", 500, 500, 255), entries[0].Zone);
        Assert.Null(entries[1].Zone);
        Assert.Equal(new PatternPointDto(4, "最北", 583, 297), entries[1].SecondShrinkPoint);
    }

    [Fact]
    public async Task AC_PAT_21_非表示の収縮候補地も名前と座標で返す()
    {
        _reader.Add(Day0, First(1, 4));
        _masters.HiddenZoneIds.Add(4);

        var zone = (await SearchAsync()).Patterns[0].Entries[0].Zone;

        Assert.Equal("候補4", zone!.NameJa);
    }

    [Fact]
    public async Task master_serviceへの問い合わせは1回で条件と出てきた収縮先だけを送る()
    {
        _reader.Add(Day0, First(1, 4), Second(1, 4));
        _reader.Add(Day0, First(1, 4), First(2, 7));

        await SearchAsync(jihenId: "crater");

        var query = Assert.Single(_masters.Queries);
        Assert.Equal(["gladius"], query.BossIds);
        Assert.Equal(["crater"], query.JihenIds);
        Assert.Equal([1], query.SpawnPointIds);
        Assert.Equal([4, 7], query.ZoneIds.Order());
        Assert.Equal([4], query.SecondShrinkPointIds);
        Assert.Empty(query.NightBossIds);
    }

    [Fact]
    public async Task 地変を省くと地変なしの条件で読む()
    {
        await SearchAsync(jihenId: "");

        Assert.Equal(("gladius", 1, (string?)null), Assert.Single(_reader.Conditions));
    }

    [Theory]
    [InlineData("unknown", 1, null, "bossId 'unknown' は存在しません")]
    [InlineData("gladius", 99, null, "spawnPointId '99' は存在しません")]
    [InlineData("gladius", 1, "unknown", "jihenId 'unknown' は存在しません")]
    public async Task AC_PAT_09_条件の行がなければ404(string bossId, int spawnPointId, string? jihenId, string message)
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => SearchAsync(bossId, spawnPointId, jihenId));

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task AC_PAT_20_master_serviceが応答しないと503の例外をそのまま伝える()
    {
        _masters.Unavailable = true;

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => SearchAsync());
    }

    /// <summary>
    /// 記録の読み出しの偽物。
    /// 条件に関係なく、足した記録をすべて返す。
    /// </summary>
    private sealed class FakeReader : IPatternRecordReader
    {
        private readonly List<RecordedShrinks> _records = [];

        public List<(string BossId, int SpawnPointId, string? JihenId)> Conditions { get; } = [];

        public void Add(DateTime createdAt, params ShrinkTarget[] shrinks) => _records.Add(new RecordedShrinks(createdAt, shrinks));

        public Task<IReadOnlyList<RecordedShrinks>> FindAsync(string bossId, int spawnPointId, string? jihenId, CancellationToken ct)
        {
            Conditions.Add((bossId, spawnPointId, jihenId));
            return Task.FromResult<IReadOnlyList<RecordedShrinks>>(_records);
        }
    }
}
