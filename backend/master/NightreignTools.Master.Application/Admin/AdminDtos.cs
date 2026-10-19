namespace NightreignTools.Master.Application.Admin;

// 管理用の API の入力と出力(マスタの管理 4.2)。
// 出力はプレイヤー向けの出力に isHidden を足した形、入力はそこから id を除いた形とする。
// 数の項目は、省いたときに 0 として通らないよう null を許す型で受け、MapPlaceAdmin が必須として確かめる。
// 必須と値の範囲と文言は、アプリケーションサービスとエンティティの決まり(マスタの管理 3.4)だけで確かめる。
// 注釈でも同じことを書くと、決まりと文言が 2 か所に分かれるので、注釈は付けない。

/// <summary>
/// 開始地点の登録と修正の入力。
/// <paramref name="IsHidden"/> を省くと、登録では表示中、修正では今のまま。
/// </summary>
/// <param name="NameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
/// <param name="X">X 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="Y">Y 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="IsHidden">非表示にするか。省くと、登録では表示中、修正では今のまま。</param>
public sealed record SaveSpawnPointRequest(
    string? NameJa,
    decimal? X,
    decimal? Y,
    bool? IsHidden);

/// <summary>
/// 収縮候補地の登録と修正の入力。
/// </summary>
/// <param name="NameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
/// <param name="IconPath">アイコンのパス。200 文字まで。空なら null にする。</param>
/// <param name="CenterX">中心の X 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="CenterY">中心の Y 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="Radius">半径。必須。10 以上 512 以下で、小数は 2 桁まで。</param>
/// <param name="IsHidden">非表示にするか。省くと、登録では表示中、修正では今のまま。</param>
public sealed record SaveShrinkZoneRequest(
    string? NameJa,
    string? IconPath,
    decimal? CenterX,
    decimal? CenterY,
    decimal? Radius,
    bool? IsHidden);

/// <summary>
/// 第二収縮地点の登録と修正の入力。
/// </summary>
/// <param name="NameJa">日本語の名前。必須。前後の空白を除いて 100 文字まで。</param>
/// <param name="X">X 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="Y">Y 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="IsHidden">非表示にするか。省くと、登録では表示中、修正では今のまま。</param>
public sealed record SaveSecondShrinkPointRequest(
    string? NameJa,
    decimal? X,
    decimal? Y,
    bool? IsHidden);

/// <summary>
/// 拠点候補地の登録と修正の入力。
/// <paramref name="MapId"/> を省くと通常のマップ。
/// </summary>
/// <param name="MapId">マップの識別子。normal か地変の識別子。</param>
/// <param name="NameJa">日本語の名前。任意。前後の空白を除いて 100 文字まで。</param>
/// <param name="X">X 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="Y">Y 座標。必須。0 以上 1024 以下で、小数は 2 桁まで。</param>
/// <param name="IsHidden">非表示にするか。省くと、登録では表示中、修正では今のまま。</param>
public sealed record SaveBasePointRequest(
    string? MapId,
    string? NameJa,
    decimal? X,
    decimal? Y,
    bool? IsHidden);

/// <summary>
/// 管理用の開始地点。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。</param>
public sealed record AdminSpawnPointDto(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>
/// 管理用の収縮候補地。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="IconPath">アイコンのパス。null はアイコンなし。</param>
/// <param name="CenterX">中心の X 座標(1024 四方の座標空間)。</param>
/// <param name="CenterY">中心の Y 座標(1024 四方の座標空間)。</param>
/// <param name="Radius">半径(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。</param>
public sealed record AdminShrinkZoneDto(int Id, string NameJa, string? IconPath, decimal CenterX, decimal CenterY, decimal Radius, bool IsHidden);

/// <summary>
/// 管理用の第二収縮地点。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。</param>
public sealed record AdminSecondShrinkPointDto(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>
/// 管理用の拠点候補地。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="MapId">マップの識別子。normal か地変の識別子。</param>
/// <param name="NameJa">日本語の名前。null は名前なし。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。</param>
public sealed record AdminBasePointDto(int Id, string MapId, string? NameJa, decimal X, decimal Y, bool IsHidden);
