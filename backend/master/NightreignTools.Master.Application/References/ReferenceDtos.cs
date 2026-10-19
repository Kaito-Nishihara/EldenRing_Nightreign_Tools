using System.ComponentModel.DataAnnotations;

namespace NightreignTools.Master.Application.References;

// マスタの参照の解決の入力と出力(マスタの管理 4.4)。record-service が、記録の保存とパターン検索のときに呼ぶ

/// <summary>
/// 引きたい識別子。どの配列も省ける。1 つの配列は <see cref="MaxIds"/> 件まで。
/// </summary>
/// <param name="BossIds">夜の王の識別子。</param>
/// <param name="JihenIds">地変の識別子。</param>
/// <param name="SpawnPointIds">開始地点の識別子。</param>
/// <param name="ZoneIds">収縮候補地の識別子。</param>
/// <param name="SecondShrinkPointIds">第二収縮地点の識別子。</param>
/// <param name="NightBossIds">夜ボスの識別子。</param>
public sealed record ResolveReferencesRequest(
    [property: MaxLength(ResolveReferencesRequest.MaxIds, ErrorMessage = "bossIds は 100 件までです")]
    IReadOnlyList<string>? BossIds = null,
    [property: MaxLength(ResolveReferencesRequest.MaxIds, ErrorMessage = "jihenIds は 100 件までです")]
    IReadOnlyList<string>? JihenIds = null,
    [property: MaxLength(ResolveReferencesRequest.MaxIds, ErrorMessage = "spawnPointIds は 100 件までです")]
    IReadOnlyList<int>? SpawnPointIds = null,
    [property: MaxLength(ResolveReferencesRequest.MaxIds, ErrorMessage = "zoneIds は 100 件までです")]
    IReadOnlyList<int>? ZoneIds = null,
    [property: MaxLength(ResolveReferencesRequest.MaxIds, ErrorMessage = "secondShrinkPointIds は 100 件までです")]
    IReadOnlyList<int>? SecondShrinkPointIds = null,
    [property: MaxLength(ResolveReferencesRequest.MaxIds, ErrorMessage = "nightBossIds は 100 件までです")]
    IReadOnlyList<int>? NightBossIds = null)
{
    public const int MaxIds = 100;
}

/// <summary>
/// 見つかった行だけを持つ。
/// 見つからない識別子は抜く。
/// </summary>
/// <param name="Bosses">見つかった夜の王。</param>
/// <param name="Jihen">見つかった地変。</param>
/// <param name="SpawnPoints">見つかった開始地点。</param>
/// <param name="Zones">見つかった収縮候補地。</param>
/// <param name="SecondShrinkPoints">見つかった第二収縮地点。</param>
/// <param name="NightBosses">見つかった夜ボス。</param>
public sealed record ResolvedReferences(
    IReadOnlyList<ResolvedBoss> Bosses,
    IReadOnlyList<ResolvedJihen> Jihen,
    IReadOnlyList<ResolvedPoint> SpawnPoints,
    IReadOnlyList<ResolvedZone> Zones,
    IReadOnlyList<ResolvedPoint> SecondShrinkPoints,
    IReadOnlyList<ResolvedNightBoss> NightBosses);

public sealed record ResolvedBoss(string Id, string NameJa);

public sealed record ResolvedJihen(string Id, string NameJa);

/// <summary>
/// 開始地点か第二収縮地点。
/// 非表示の行も返し、<paramref name="IsHidden"/> で分かるようにする。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。</param>
public sealed record ResolvedPoint(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>
/// 収縮候補地。
/// 非表示の行も返す。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="CenterX">中心の X 座標(1024 四方の座標空間)。</param>
/// <param name="CenterY">中心の Y 座標(1024 四方の座標空間)。</param>
/// <param name="Radius">半径(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。</param>
public sealed record ResolvedZone(int Id, string NameJa, decimal CenterX, decimal CenterY, decimal Radius, bool IsHidden);

/// <summary>
/// 夜ボス。
/// <paramref name="Day"/> は出る日(1 か 2)。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="Day">出る日。1 か 2。</param>
public sealed record ResolvedNightBoss(int Id, string NameJa, int Day);
