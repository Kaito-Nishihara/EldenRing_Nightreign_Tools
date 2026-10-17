using System.ComponentModel.DataAnnotations;
using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Application.PlayRecords;

// プレイ記録の保存の入力と出力(プレイ記録 4 章)。
// 形の誤りは注釈で、記録の中の規則はエンティティで、マスタにあるかどうかはアプリケーションサービスで確かめる

/// <summary>記録の保存の入力。</summary>
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

/// <summary>1 つの収縮先。第 1 次は zoneId、第 2 次は secondShrinkPointId で指定する。</summary>
public sealed record ShrinkEntryRequest(
    [property: Range(1, 2, ErrorMessage = "day は 1 か 2 です")]
    int Day,
    [property: Range(1, 2, ErrorMessage = "phase は 1 か 2 です")]
    int Phase,
    int? ZoneId,
    int? SecondShrinkPointId);

/// <summary>記録の保存の出力(201)。</summary>
public sealed record CreatePlayRecordResponse(int Id, DateTime CreatedAt);
