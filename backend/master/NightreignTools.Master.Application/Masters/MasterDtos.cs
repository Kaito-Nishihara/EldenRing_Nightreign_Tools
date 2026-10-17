namespace NightreignTools.Master.Application.Masters;

// プレイヤー向けのマスタの取得の出力(マスタの管理 4.1)。エンティティを応答に返さず、これに詰め替える

/// <summary>夜の王。<paramref name="Weaknesses"/> は弱点の属性の並び。</summary>
public sealed record BossDto(
    string Id, string NameJa, string? NameEn, string? ImagePath, IReadOnlyList<string> Weaknesses, bool IsDlc, string? UnlockCondition);

/// <summary>地変。中心は 1024 四方の座標空間の数。</summary>
public sealed record JihenDto(
    string Id, string NameJa, string? ImagePath, string Area, decimal CenterX, decimal CenterY, bool IsDlc, string? UnlockCondition);

/// <summary>開始地点。</summary>
public sealed record SpawnPointDto(int Id, string NameJa, decimal X, decimal Y);

/// <summary>収縮候補地。</summary>
public sealed record ShrinkZoneDto(int Id, string NameJa, string? IconPath, decimal CenterX, decimal CenterY, decimal Radius);

/// <summary>第二収縮地点。</summary>
public sealed record SecondShrinkPointDto(int Id, string NameJa, decimal X, decimal Y);

/// <summary>拠点候補地。</summary>
public sealed record BasePointDto(int Id, string MapId, string? NameJa, decimal X, decimal Y);

/// <summary>夜ボス。<paramref name="Day"/> は出る日(1 か 2)。</summary>
public sealed record NightBossDto(int Id, string NameJa, int Day);
