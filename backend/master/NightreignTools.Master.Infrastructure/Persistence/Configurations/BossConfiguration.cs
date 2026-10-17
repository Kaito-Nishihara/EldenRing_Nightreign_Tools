using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence.Seed;

namespace NightreignTools.Master.Infrastructure.Persistence.Configurations;

// 初期データだけで管理するマスタ(夜の王、弱点、地変、夜ボス)

internal sealed class BossConfiguration : IEntityTypeConfiguration<Boss>
{
    public void Configure(EntityTypeBuilder<Boss> builder)
    {
        builder.ToTable("bosses");
        builder.Property(b => b.Id).HasMaxLength(50);
        builder.Property(b => b.NameJa).HasMaxLength(100);
        builder.Property(b => b.NameEn).HasMaxLength(100);
        builder.Property(b => b.ImagePath).HasMaxLength(200);
        builder.Property(b => b.UnlockCondition).HasMaxLength(200);
        builder.Property(b => b.IsDlc).HasDefaultValue(false);

        // 夜の王を消すと弱点も要らなくなる(データベースの共通仕様 5.4)
        builder.HasMany(b => b.Weaknesses).WithOne().HasForeignKey(w => w.BossId).OnDelete(DeleteBehavior.Cascade);

        builder.HasData(BossSeed.Bosses);
    }
}

internal sealed class BossWeaknessConfiguration : IEntityTypeConfiguration<BossWeakness>
{
    public void Configure(EntityTypeBuilder<BossWeakness> builder)
    {
        builder.ToTable("boss_weaknesses");
        builder.HasSeededIdentity(w => w.Id);
        builder.Property(w => w.Attribute).HasMaxLength(30);
        builder.HasData(BossSeed.Weaknesses);
    }
}

internal sealed class JihenConfiguration : IEntityTypeConfiguration<Jihen>
{
    public void Configure(EntityTypeBuilder<Jihen> builder)
    {
        const string table = "jihen";
        builder.ToTable(table);
        builder.Property(j => j.Id).HasMaxLength(50);
        builder.Property(j => j.NameJa).HasMaxLength(100);
        builder.Property(j => j.ImagePath).HasMaxLength(200);
        builder.Property(j => j.Area).HasMaxLength(50);
        builder.Property(j => j.UnlockCondition).HasMaxLength(200);
        builder.Property(j => j.IsDlc).HasDefaultValue(false);
        builder.HasCoordinate(table, j => j.CenterX, "center_x");
        builder.HasCoordinate(table, j => j.CenterY, "center_y");
        builder.HasData(JihenSeed.Rows);
    }
}

internal sealed class NightBossConfiguration : IEntityTypeConfiguration<NightBoss>
{
    public void Configure(EntityTypeBuilder<NightBoss> builder)
    {
        builder.ToTable("night_bosses", t => t.HasCheckConstraint("ck_night_bosses_day", "day IN (1, 2)"));
        builder.HasSeededIdentity(n => n.Id);
        builder.Property(n => n.NameJa).HasMaxLength(100);
        builder.HasData(NightBossSeed.Rows);
    }
}
