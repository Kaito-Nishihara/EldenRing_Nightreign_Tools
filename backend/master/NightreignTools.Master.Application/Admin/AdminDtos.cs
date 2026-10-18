namespace NightreignTools.Master.Application.Admin;

// 管理用の API の入力と出力(マスタの管理 4.2)。
// 出力はプレイヤー向けの出力に isHidden を足した形、入力はそこから id を除いた形とする。
// 数の項目は、省いたときに 0 として通らないよう null を許す型で受け、MapPlaceAdmin が必須として確かめる。
// エンドポイントは 4 つの種類を 1 つの汎用の登録で作るので、注釈による検証(ソースジェネレーター)が働かない。
// そのため注釈は付けず、必須と値の範囲と文言は、アプリケーションサービスとエンティティの決まり(マスタの管理 3.4)で確かめる。

/// <summary>開始地点の登録と修正の入力。<paramref name="IsHidden"/> を省くと、登録では表示中、修正では今のまま。</summary>
public sealed record SaveSpawnPointRequest(
    string? NameJa,
    decimal? X,
    decimal? Y,
    bool? IsHidden);

/// <summary>収縮候補地の登録と修正の入力。</summary>
public sealed record SaveShrinkZoneRequest(
    string? NameJa,
    string? IconPath,
    decimal? CenterX,
    decimal? CenterY,
    decimal? Radius,
    bool? IsHidden);

/// <summary>第二収縮地点の登録と修正の入力。</summary>
public sealed record SaveSecondShrinkPointRequest(
    string? NameJa,
    decimal? X,
    decimal? Y,
    bool? IsHidden);

/// <summary>拠点候補地の登録と修正の入力。<paramref name="MapId"/> を省くと通常のマップ。</summary>
public sealed record SaveBasePointRequest(
    string? MapId,
    string? NameJa,
    decimal? X,
    decimal? Y,
    bool? IsHidden);

/// <summary>管理用の開始地点。</summary>
public sealed record AdminSpawnPointDto(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>管理用の収縮候補地。</summary>
public sealed record AdminShrinkZoneDto(int Id, string NameJa, string? IconPath, decimal CenterX, decimal CenterY, decimal Radius, bool IsHidden);

/// <summary>管理用の第二収縮地点。</summary>
public sealed record AdminSecondShrinkPointDto(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>管理用の拠点候補地。</summary>
public sealed record AdminBasePointDto(int Id, string MapId, string? NameJa, decimal X, decimal Y, bool IsHidden);
