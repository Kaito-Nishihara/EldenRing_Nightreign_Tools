namespace NightreignTools.Record.Application.Masters;

// master-service の内部の API(マスタの管理 4.4)の入力と出力。
// master-service のプロジェクトを参照せず、record-service の側で形を持つ(サービスの間はデータの形だけでつなぐ)

/// <summary>
/// 引きたい識別子。
/// 空の配列は問い合わせない。
/// </summary>
/// <param name="BossIds">夜の王の識別子。</param>
/// <param name="JihenIds">地変の識別子。地変なしの記録では空。</param>
/// <param name="SpawnPointIds">開始地点の識別子。</param>
/// <param name="ZoneIds">収縮候補地の識別子。</param>
/// <param name="SecondShrinkPointIds">第二収縮地点の識別子。</param>
/// <param name="NightBossIds">夜ボスの識別子。</param>
public sealed record MasterReferenceQuery(
    IReadOnlyCollection<string> BossIds,
    IReadOnlyCollection<string> JihenIds,
    IReadOnlyCollection<int> SpawnPointIds,
    IReadOnlyCollection<int> ZoneIds,
    IReadOnlyCollection<int> SecondShrinkPointIds,
    IReadOnlyCollection<int> NightBossIds);

/// <summary>
/// 見つかった行だけを持つ。
/// 見つからない識別子は入らない。
/// 非表示の行も入る。
/// </summary>
/// <param name="Bosses">見つかった夜の王。</param>
/// <param name="Jihen">見つかった地変。</param>
/// <param name="SpawnPoints">見つかった開始地点。</param>
/// <param name="Zones">見つかった収縮候補地。</param>
/// <param name="SecondShrinkPoints">見つかった第二収縮地点。</param>
/// <param name="NightBosses">見つかった夜ボス。</param>
public sealed record MasterReferences(
    IReadOnlyList<MasterBoss> Bosses,
    IReadOnlyList<MasterJihen> Jihen,
    IReadOnlyList<MasterPoint> SpawnPoints,
    IReadOnlyList<MasterZone> Zones,
    IReadOnlyList<MasterPoint> SecondShrinkPoints,
    IReadOnlyList<MasterNightBoss> NightBosses);

public sealed record MasterBoss(string Id, string NameJa);

public sealed record MasterJihen(string Id, string NameJa);

/// <summary>
/// 開始地点か第二収縮地点。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="X">X 座標(1024 四方の座標空間)。</param>
/// <param name="Y">Y 座標(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。非表示の行は新しい記録に使えない。</param>
public sealed record MasterPoint(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>
/// 収縮候補地。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="CenterX">中心の X 座標(1024 四方の座標空間)。</param>
/// <param name="CenterY">中心の Y 座標(1024 四方の座標空間)。</param>
/// <param name="Radius">半径(1024 四方の座標空間)。</param>
/// <param name="IsHidden">非表示か。非表示の行は新しい記録に使えない。</param>
public sealed record MasterZone(int Id, string NameJa, decimal CenterX, decimal CenterY, decimal Radius, bool IsHidden);

/// <summary>
/// 夜ボス。
/// <paramref name="Day"/> は出る日(1 か 2)。
/// </summary>
/// <param name="Id">識別子。</param>
/// <param name="NameJa">日本語の名前。</param>
/// <param name="Day">出る日。1 か 2。</param>
public sealed record MasterNightBoss(int Id, string NameJa, int Day);
