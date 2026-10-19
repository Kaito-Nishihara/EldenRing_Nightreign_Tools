using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NightreignTools.Relic.Domain.Entities;
using NightreignTools.Relic.Infrastructure.Persistence.Seed;

namespace NightreignTools.Relic.Infrastructure.Persistence.Configurations;

// 初期データだけで管理するマスタ(遺物の効果の分類、効果、ランクごとの値。ビルドシミュレーター 5 章)

internal sealed class RelicEffectCategoryConfiguration : IEntityTypeConfiguration<RelicEffectCategory>
{
    public void Configure(EntityTypeBuilder<RelicEffectCategory> builder)
    {
        builder.ToTable("relic_effect_categories");
        builder.Property(c => c.Id).HasMaxLength(50);
        builder.Property(c => c.NameJa).HasMaxLength(100);
        builder.HasData(RelicEffectSeed.Categories);
    }
}

internal sealed class RelicEffectConfiguration : IEntityTypeConfiguration<RelicEffect>
{
    public void Configure(EntityTypeBuilder<RelicEffect> builder)
    {
        const string table = "relic_effects";
        builder.ToTable(table, t =>
        {
            t.HasCheckConstraint($"ck_{table}_effect_sign", InList("effect_sign", RelicEffectSigns.All));
            t.HasCheckConstraint($"ck_{table}_calc_type", InList("calc_type", RelicCalcTypes.All));
            t.HasCheckConstraint($"ck_{table}_stackable", InList("stackable", RelicStackables.All));
            t.HasCheckConstraint($"ck_{table}_rank", "rank_min >= 0 AND rank_max >= rank_min");
        });
        builder.Property(e => e.Id).UseIdentityByDefaultColumn().HasIdentityOptions(startValue: SeedData.IdLimit + 1);
        builder.Property(e => e.CategoryId).HasMaxLength(50);
        builder.Property(e => e.NameJa).HasMaxLength(100);
        builder.Property(e => e.IsDeep).HasDefaultValue(false);
        builder.Property(e => e.EffectSign).HasMaxLength(20).HasDefaultValue(RelicEffectSigns.Positive);
        builder.Property(e => e.CalcType).HasMaxLength(20);
        builder.Property(e => e.Attribute).HasMaxLength(50);
        builder.Property(e => e.Stackable).HasMaxLength(20).HasDefaultValue(RelicStackables.Yes);
        builder.Property(e => e.ConditionJa).HasMaxLength(500);
        builder.Property(e => e.NotesJa).HasMaxLength(500);

        // 分類は効果から参照されている間は消せない。効果を消すとランクごとの値も要らなくなる(データベースの共通仕様 5.4)
        builder.HasOne<RelicEffectCategory>().WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Ranks).WithOne().HasForeignKey(r => r.EffectId).OnDelete(DeleteBehavior.Cascade);

        builder.HasData(RelicEffectSeed.Effects);
    }

    /// <summary>決まった値の一覧の検査の制約の式(データベースの共通仕様 5.3)。</summary>
    internal static string InList(string column, IEnumerable<string> values) =>
        $"{column} IN ({string.Join(", ", values.Select(v => $"'{v}'"))})";
}

internal sealed class RelicEffectRankConfiguration : IEntityTypeConfiguration<RelicEffectRank>
{
    public void Configure(EntityTypeBuilder<RelicEffectRank> builder)
    {
        const string table = "relic_effect_ranks";
        builder.ToTable(table, t =>
        {
            t.HasCheckConstraint($"ck_{table}_unit", RelicEffectConfiguration.InList("unit", RelicValueUnits.All));
            t.HasCheckConstraint($"ck_{table}_rank", "rank >= 0");
        });
        builder.Property(r => r.Id).UseIdentityByDefaultColumn().HasIdentityOptions(startValue: SeedData.IdLimit + 1);
        builder.Property(r => r.Value).HasPrecision(10, 4);
        builder.Property(r => r.Unit).HasMaxLength(20);
        builder.Property(r => r.HasDemerit).HasDefaultValue(false);

        // 1 つの効果の同じランクは 1 行だけ(ビルドシミュレーター 5 章)
        builder.HasIndex(r => new { r.EffectId, r.Rank }).IsUnique();

        builder.HasData(RelicEffectSeed.Ranks);
    }
}
