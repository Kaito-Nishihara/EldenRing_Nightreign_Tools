using NightreignTools.Record.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Tests.Unit;

/// <summary>
/// 記録の中だけで確かめられる規則(プレイ記録 4 章、5 章)。マスタにあるかどうかは、アプリケーションサービスのテストで確かめる。
/// </summary>
public class PlayRecordTests
{
    private static readonly ShrinkTarget Day1First = new(1, 1, ZoneId: 4, SecondShrinkPointId: null);
    private static readonly ShrinkTarget Day1Second = new(1, 2, ZoneId: null, SecondShrinkPointId: 4);
    private static readonly ShrinkTarget Day2First = new(2, 1, ZoneId: 7, SecondShrinkPointId: null);

    private static PlayRecord Create(IReadOnlyCollection<ShrinkTarget> entries, string? note = null, string? jihenId = null) =>
        PlayRecord.Create("gladius", jihenId, 1, day1NightBossId: 3, day2NightBossId: null, note, entries);

    [Fact]
    public void 正しい入力で作ると収縮先を日と段階の順に持つ()
    {
        var record = Create([Day2First, Day1Second, Day1First]);

        Assert.Equal([(1, 1), (1, 2), (2, 1)], record.Entries.Select(e => (e.Day, e.Phase)));
        Assert.Equal(4, record.Entries[0].ZoneId);
        Assert.Null(record.Entries[0].SecondShrinkPointId);
        Assert.Null(record.Entries[1].ZoneId);
        Assert.Equal(4, record.Entries[1].SecondShrinkPointId);
    }

    [Fact]
    public void AC_REC_20_夜ボスは指定した日だけ値を持つ()
    {
        var record = Create([Day1First]);

        Assert.Equal(3, record.Day1NightBossId);
        Assert.Null(record.Day2NightBossId);
    }

    [Fact]
    public void AC_REC_02_収縮先が空だと作れない()
    {
        var exception = Assert.Throws<BusinessRuleException>(() => Create([]));

        Assert.Equal("entries は 1 件以上 4 件以下です", exception.Message);
    }

    [Fact]
    public void 収縮先が5件だと作れない()
    {
        Assert.Throws<BusinessRuleException>(() => Create([Day1First, Day1Second, Day2First, new(2, 2, null, 5), new(1, 1, 3, null)]));
    }

    [Fact]
    public void AC_REC_03_日と段階の組が重複すると作れない()
    {
        var exception = Assert.Throws<BusinessRuleException>(() => Create([Day1First, new(1, 1, ZoneId: 5, SecondShrinkPointId: null)]));

        Assert.Equal("day=1, phase=1 が重複しています", exception.Message);
    }

    [Fact]
    public void AC_REC_16_第2次を収縮候補地で指定すると作れない()
    {
        var exception = Assert.Throws<BusinessRuleException>(() => Create([new(1, 2, ZoneId: 4, SecondShrinkPointId: null)]));

        Assert.Equal("day=1, phase=2 は secondShrinkPointId で指定してください", exception.Message);
    }

    [Fact]
    public void AC_REC_17_第1次を第二収縮地点で指定すると作れない()
    {
        var exception = Assert.Throws<BusinessRuleException>(() => Create([new(2, 1, ZoneId: null, SecondShrinkPointId: 4)]));

        Assert.Equal("day=2, phase=1 は zoneId で指定してください", exception.Message);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(3, 1)]
    [InlineData(1, 3)]
    public void 日か段階が1と2のほかだと作れない(int day, int phase)
    {
        Assert.Throws<BusinessRuleException>(() => Create([new(day, phase, ZoneId: 4, SecondShrinkPointId: null)]));
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void AC_REC_07_備考が空白だけならnullで持つ(string note)
    {
        Assert.Null(Create([Day1First], note).Note);
    }

    [Fact]
    public void 備考は前後の空白を除いて持つ()
    {
        Assert.Equal("2 日目は地変の方へ寄った", Create([Day1First], "  2 日目は地変の方へ寄った \n").Note);
    }

    [Fact]
    public void AC_REC_06_前後の空白を除いて501文字の備考は持てない()
    {
        var exception = Assert.Throws<BusinessRuleException>(() => Create([Day1First], new string('あ', 501)));

        Assert.Equal("note は 500 文字以内です", exception.Message);
    }

    [Fact]
    public void 前後の空白を除いて500文字の備考は持てる()
    {
        Assert.Equal(500, Create([Day1First], $"  {new string('あ', 500)}  ").Note!.Length);
    }

    [Fact]
    public void 地変の空文字は地変なしとして持つ()
    {
        Assert.Null(Create([Day1First], jihenId: "").JihenId);
    }
}
