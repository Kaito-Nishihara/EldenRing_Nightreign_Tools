using System.ComponentModel.DataAnnotations;
using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Application.PlayRecords;

// プレイ記録の保存の入力と出力(プレイ記録 4 章)。
// 形の誤りは注釈で、記録の中の規則はエンティティで、マスタにあるかどうかはアプリケーションサービスで確かめる

/// <summary>
/// 記録の保存の入力。
/// </summary>
/// <param name="BossId">夜の王の識別子。必須。50 文字以内。</param>
/// <param name="JihenId">地変の識別子。50 文字以内。null は地変なし。</param>
/// <param name="SpawnPointId">開始地点の識別子。必須。</param>
/// <param name="Day1NightBossId">1 日目の夜ボスの識別子。null は記録しない。</param>
/// <param name="Day2NightBossId">2 日目の夜ボスの識別子。null は記録しない。</param>
/// <param name="Note">備考。前後の空白を除いて 500 文字以内。</param>
/// <param name="Entries">収縮先。1 件以上 4 件以下。</param>
public sealed record CreatePlayRecordRequest(
    [property: Required(ErrorMessage = "bossId は必須です")]
    [property: MaxLength(CreatePlayRecordRequest.IdMaxLength, ErrorMessage = "bossId は 50 文字以内です")]
    string BossId,
    [property: MaxLength(CreatePlayRecordRequest.IdMaxLength, ErrorMessage = "jihenId は 50 文字以内です")]
    string? JihenId,
    [property: Required(ErrorMessage = "spawnPointId は必須です")]
    int? SpawnPointId,
    int? Day1NightBossId,
    int? Day2NightBossId,
    // 備考は前後の空白を除いて 500 文字まで(エンティティで確かめる)。ここでは大きすぎる入力だけを止める
    [property: MaxLength(CreatePlayRecordRequest.RawNoteMaxLength, ErrorMessage = "note は 500 文字以内です")]
    string? Note,
    [property: Required(ErrorMessage = "entries は 1 件以上 4 件以下です")]
    [property: MinLength(PlayRecord.MinEntries, ErrorMessage = "entries は 1 件以上 4 件以下です")]
    [property: MaxLength(PlayRecord.MaxEntries, ErrorMessage = "entries は 1 件以上 4 件以下です")]
    IReadOnlyList<ShrinkEntryRequest> Entries)
{
    public const int IdMaxLength = 50;
    public const int RawNoteMaxLength = 2000;
}

/// <summary>
/// 1 つの収縮先。
/// 第 1 次は zoneId、第 2 次は secondShrinkPointId で指定する。
/// </summary>
/// <param name="Day">日。1 か 2。</param>
/// <param name="Phase">段階。1 か 2。</param>
/// <param name="ZoneId">第 1 次の収縮候補地の識別子。第 2 次では null にする。</param>
/// <param name="SecondShrinkPointId">第 2 次の第二収縮地点の識別子。第 1 次では null にする。</param>
public sealed record ShrinkEntryRequest(
    [property: Range(1, 2, ErrorMessage = "day は 1 か 2 です")]
    int Day,
    [property: Range(1, 2, ErrorMessage = "phase は 1 か 2 です")]
    int Phase,
    int? ZoneId,
    int? SecondShrinkPointId);

/// <summary>
/// 記録の保存の出力(201)。
/// </summary>
/// <param name="Id">保存した記録の識別子。</param>
/// <param name="CreatedAt">保存した日時(UTC)。</param>
public sealed record CreatePlayRecordResponse(int Id, DateTime CreatedAt);
