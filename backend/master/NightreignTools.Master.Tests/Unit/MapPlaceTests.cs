using NightreignTools.Master.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Tests.Unit;

/// <summary>
/// マップの上の場所の入力の決まり(マスタの管理 3.4)と、非表示(決定の記録 0013)。
/// </summary>
public class MapPlaceTests
{
    private static readonly DateTime Now = new(2026, 10, 18, 3, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void 収縮候補地を作ると名前の前後の空白を除き空のアイコンはnullにする()
    {
        var zone = ShrinkZone.Create(" 最北 ", " ", 583m, 297m, 150m);

        Assert.Equal("最北", zone.NameJa);
        Assert.Null(zone.IconPath);
        Assert.Equal((583m, 297m, 150m), (zone.CenterX, zone.CenterY, zone.Radius));
        Assert.False(zone.IsHidden);
    }

    [Theory]
    [InlineData(1025)]
    [InlineData(-0.01)]
    [InlineData(10.125)]
    public void AC_ADM_02_座標が0から1024の小数2桁までの数でなければ400にする(double x)
    {
        var error = Assert.Throws<BusinessRuleException>(() => SpawnPoint.Create("北東の教会", (decimal)x, 100m));

        Assert.Equal(MapPlace.CoordinateMessage, error.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1024)]
    [InlineData(512.25)]
    public void 座標は0と1024を含み小数2桁まで使える(double value)
    {
        var point = SecondShrinkPoint.Create("最北", (decimal)value, (decimal)value);

        Assert.Equal((decimal)value, point.X);
    }

    [Theory]
    [InlineData(9.99)]
    [InlineData(512.01)]
    public void 半径が10から512でなければ400にする(double radius)
    {
        var error = Assert.Throws<BusinessRuleException>(() => ShrinkZone.Create("最北", null, 500m, 500m, (decimal)radius));

        Assert.Equal(MapPlace.RadiusMessage, error.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void 名前が空なら400にする(string? name)
    {
        var error = Assert.Throws<BusinessRuleException>(() => SpawnPoint.Create(name, 1m, 1m));

        Assert.Equal(MapPlace.NameRequiredMessage, error.Message);
    }

    [Fact]
    public void 名前は100文字までで101文字なら400にする()
    {
        SpawnPoint.Create(new string('あ', 100), 1m, 1m);

        Assert.Throws<BusinessRuleException>(() => SpawnPoint.Create(new string('あ', 101), 1m, 1m));
    }

    [Fact]
    public void 拠点候補地の名前は任意で空ならnullにする()
    {
        var point = BasePoint.Create("crater", "minor", " ", 10m, 20m);

        Assert.Null(point.NameJa);
        Assert.Equal("crater", point.MapId);
        Assert.Equal(BasePointKinds.Minor, point.Kind);
    }

    [Fact]
    public void 拠点候補地の種類を誤った修正では何も変えない()
    {
        var point = BasePoint.Create("normal", "major", null, 10m, 20m);

        Assert.Throws<BusinessRuleException>(() => point.Update("normal", "red", null, 30m, 40m));

        Assert.Equal(BasePointKinds.Major, point.Kind);
        Assert.Equal(10m, point.X);
    }

    [Fact]
    public void アイコンのパスは200文字を超えると400にする()
    {
        Assert.Throws<BusinessRuleException>(() => ShrinkZone.Create("最北", new string('a', 201), 500m, 500m, 100m));
    }

    [Fact]
    public void 決まりに反する修正では何も変えない()
    {
        var point = SpawnPoint.Create("北東の教会", 100m, 200m);

        Assert.Throws<BusinessRuleException>(() => point.Update("南の教会", 300m, 2000m));

        Assert.Equal(("北東の教会", 100m, 200m), (point.NameJa, point.X, point.Y));
    }

    [Fact]
    public void 非表示にすると日時が入り表示に戻すとnullに戻る()
    {
        var point = SecondShrinkPoint.Create("最北", 1m, 1m);

        point.SetHidden(true, Now);
        Assert.Equal(Now, point.HiddenAt);
        Assert.True(point.IsHidden);

        point.SetHidden(false, Now);
        Assert.Null(point.HiddenAt);
    }

    [Fact]
    public void 非表示の行をもう一度非表示にしても初めの日時を残す()
    {
        var point = SpawnPoint.Create("北東の教会", 1m, 1m);
        point.SetHidden(true, Now);

        point.SetHidden(true, Now.AddHours(1));

        Assert.Equal(Now, point.HiddenAt);
    }
}
