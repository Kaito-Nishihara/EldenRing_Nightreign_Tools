using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NightreignTools.Record.Domain.Entities;

namespace NightreignTools.Record.Infrastructure.Persistence.Configurations;

// プレイ記録と収縮先(プレイ記録 5 章)。
// マスタの識別子の列は nightreign_master の行を指すだけで、外部キーを持たない(決定の記録 0012)

internal sealed class PlayRecordConfiguration : IEntityTypeConfiguration<PlayRecord>
{
    public void Configure(EntityTypeBuilder<PlayRecord> builder)
    {
        builder.ToTable("play_records");
        builder.Property(r => r.BossId).HasMaxLength(50);
        builder.Property(r => r.JihenId).HasMaxLength(50);
        builder.Property(r => r.Day1NightBossId).HasColumnName("day1_night_boss_id");
        builder.Property(r => r.Day2NightBossId).HasColumnName("day2_night_boss_id");
        builder.Property(r => r.Note).HasMaxLength(PlayRecord.NoteMaxLength);

        // パターン検索は夜の王、開始地点、地変で絞る(データベースの共通仕様 7 章)
        builder.HasIndex(r => new { r.BossId, r.SpawnPointId, r.JihenId });

        builder.HasMany(r => r.Entries)
            .WithOne()
            .HasForeignKey(e => e.PlayRecordId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Entries).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class ShrinkEntryConfiguration : IEntityTypeConfiguration<ShrinkEntry>
{
    public void Configure(EntityTypeBuilder<ShrinkEntry> builder)
    {
        const string table = "shrink_entries";
        // 規則はエンティティで確かめる。データベースの制約は、規則を破った行が入らないための最後の守り(プレイ記録 5 章)
        builder.ToTable(table, t =>
        {
            t.HasCheckConstraint($"ck_{table}_day", "day IN (1, 2)");
            t.HasCheckConstraint($"ck_{table}_phase", "phase IN (1, 2)");
            t.HasCheckConstraint(
                $"ck_{table}_target",
                "(phase = 1 AND zone_id IS NOT NULL AND second_shrink_point_id IS NULL)"
                + " OR (phase = 2 AND zone_id IS NULL AND second_shrink_point_id IS NOT NULL)");
        });
        builder.HasIndex(e => new { e.PlayRecordId, e.Day, e.Phase }).IsUnique();
    }
}
