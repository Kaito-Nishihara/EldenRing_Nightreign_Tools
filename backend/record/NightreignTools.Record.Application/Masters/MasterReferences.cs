namespace NightreignTools.Record.Application.Masters;

// master-service の内部の API(マスタの管理 4.4)の入力と出力。
// master-service のプロジェクトを参照せず、record-service の側で形を持つ(サービスの間はデータの形だけでつなぐ)

/// <summary>引きたい識別子。空の配列は問い合わせない。</summary>
public sealed record MasterReferenceQuery(
    IReadOnlyCollection<string> BossIds,
    IReadOnlyCollection<string> JihenIds,
    IReadOnlyCollection<int> SpawnPointIds,
    IReadOnlyCollection<int> ZoneIds,
    IReadOnlyCollection<int> SecondShrinkPointIds,
    IReadOnlyCollection<int> NightBossIds);

/// <summary>見つかった行だけを持つ。見つからない識別子は入らない。非表示の行も入る。</summary>
public sealed record MasterReferences(
    IReadOnlyList<MasterBoss> Bosses,
    IReadOnlyList<MasterJihen> Jihen,
    IReadOnlyList<MasterPoint> SpawnPoints,
    IReadOnlyList<MasterZone> Zones,
    IReadOnlyList<MasterPoint> SecondShrinkPoints,
    IReadOnlyList<MasterNightBoss> NightBosses);

public sealed record MasterBoss(string Id, string NameJa);

public sealed record MasterJihen(string Id, string NameJa);

/// <summary>開始地点か第二収縮地点。</summary>
public sealed record MasterPoint(int Id, string NameJa, decimal X, decimal Y, bool IsHidden);

/// <summary>収縮候補地。</summary>
public sealed record MasterZone(int Id, string NameJa, decimal CenterX, decimal CenterY, decimal Radius, bool IsHidden);

/// <summary>夜ボス。<paramref name="Day"/> は出る日(1 か 2)。</summary>
public sealed record MasterNightBoss(int Id, string NameJa, int Day);
