namespace NightreignTools.Master.Application.Masters;

// プレイヤー向けのマスタの取得の出力(マスタの管理 4.1)。エンティティを応答に返さず、これに詰め替える

/// <summary>
/// 夜の王。
/// <paramref name="Weaknesses"/> は弱点の属性の並び。
/// </summary>
/// <param name="Id">識別子。例: gladius。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="NameEn">英語の名前。</param>
/// <param name="ImagePath">画像のパス。null は画像なし。</param>
/// <param name="Weaknesses">弱点の属性。</param>
/// <param name="IsDlc">ダウンロードコンテンツで足された夜の王か。</param>
/// <param name="UnlockCondition">戦えるようになる条件。null は最初から戦える。</param>
public sealed record BossDto(
    string Id, string NameJa, string? NameEn, string? ImagePath, IReadOnlyList<string> Weaknesses, bool IsDlc, string? UnlockCondition);

/// <summary>
/// 地変。
/// 中心は 1024 四方の座標空間の数。
/// </summary>
/// <param name="Id">識別子。例: crater。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="ImagePath">地変のマップ画像のパス。null は画像なし。</param>
/// <param name="Area">地変が起きる方角。例: north、center、all。</param>
/// <param name="CenterX">中心の X 座標(1024 四方の座標空間)。</param>
/// <param name="CenterY">中心の Y 座標(1024 四方の座標空間)。</param>
/// <param name="IsDlc">ダウンロードコンテンツで足された地変か。</param>
/// <param name="UnlockCondition">起きるようになる条件。null は最初から起きる。</param>
public sealed record JihenDto(
    string Id, string NameJa, string? ImagePath, string Area, decimal CenterX, decimal CenterY, bool IsDlc, string? UnlockCondition);

/// <summary>
/// 開始地点。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
public sealed record SpawnPointDto(int Id, string NameJa, decimal X, decimal Y);

/// <summary>
/// 収縮候補地。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="IconPath">アイコンのパス。null はアイコンなし。</param>
/// <param name="CenterX">中心の X 座標(1024 四方の座標空間)。</param>
/// <param name="CenterY">中心の Y 座標(1024 四方の座標空間)。</param>
/// <param name="Radius">半径(1024 四方の座標空間)。</param>
public sealed record ShrinkZoneDto(int Id, string NameJa, string? IconPath, decimal CenterX, decimal CenterY, decimal Radius);

/// <summary>
/// 第二収縮地点。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
public sealed record SecondShrinkPointDto(int Id, string NameJa, decimal X, decimal Y);

/// <summary>
/// 拠点候補地。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="MapId">マップの識別子。normal か地変の識別子。</param>
/// <param name="NameJa">日本語の名前。null は名前なし。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
public sealed record BasePointDto(int Id, string MapId, string? NameJa, decimal X, decimal Y);

/// <summary>
/// 夜ボス。
/// <paramref name="Day"/> は出る日(1 か 2)。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="Day">出る日。1 か 2。</param>
public sealed record NightBossDto(int Id, string NameJa, int Day);
