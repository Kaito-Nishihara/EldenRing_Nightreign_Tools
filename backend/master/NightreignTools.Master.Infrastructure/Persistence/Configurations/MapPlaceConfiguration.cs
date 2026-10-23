using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence.Seed;

namespace NightreignTools.Master.Infrastructure.Persistence.Configurations;

// マップの上の場所。管理の画面で座標を登録、修正し、非表示にする(マスタの管理 5 章)

internal sealed class SpawnPointConfiguration : IEntityTypeConfiguration<SpawnPoint>
{
    public void Configure(EntityTypeBuilder<SpawnPoint> builder)
    {
        const string table = "spawn_points";
        builder.ToTable(table);
        builder.HasMapPlaceDefaults();
        builder.Property(p => p.NameJa).HasMaxLength(100);
        builder.HasCoordinate(table, p => p.X, "x");
        builder.HasCoordinate(table, p => p.Y, "y");
        builder.HasData(MapPlaceSeed.SpawnPoints);
    }
}

internal sealed class ShrinkZoneConfiguration : IEntityTypeConfiguration<ShrinkZone>
{
    public void Configure(EntityTypeBuilder<ShrinkZone> builder)
    {
        const string table = "shrink_zones";
        builder.ToTable(table);
        builder.HasMapPlaceDefaults();
        builder.Property(z => z.NameJa).HasMaxLength(100);
        builder.Property(z => z.IconPath).HasMaxLength(200);
        builder.HasCoordinate(table, z => z.CenterX, "center_x");
        builder.HasCoordinate(table, z => z.CenterY, "center_y");
        builder.HasCoordinate(table, z => z.Radius, "radius");
        builder.HasData(MapPlaceSeed.ShrinkZones);
    }
}

internal sealed class SecondShrinkPointConfiguration : IEntityTypeConfiguration<SecondShrinkPoint>
{
    public void Configure(EntityTypeBuilder<SecondShrinkPoint> builder)
    {
        const string table = "second_shrink_points";
        builder.ToTable(table);
        builder.HasMapPlaceDefaults();
        builder.Property(p => p.NameJa).HasMaxLength(100);
        builder.HasCoordinate(table, p => p.X, "x");
        builder.HasCoordinate(table, p => p.Y, "y");
        builder.HasData(MapPlaceSeed.SecondShrinkPoints);
    }
}

internal sealed class BasePointConfiguration : IEntityTypeConfiguration<BasePoint>
{
    public void Configure(EntityTypeBuilder<BasePoint> builder)
    {
        const string table = "base_points";
        builder.ToTable(table);
        builder.HasMapPlaceDefaults();
        builder.Property(p => p.MapId).HasMaxLength(50).HasDefaultValue(BasePoint.NormalMapId);
        // 種類を足す前からある行は、どれも大拠点だった(決定の記録 0018)
        builder.Property(p => p.Kind).HasMaxLength(20).HasDefaultValue(BasePointKinds.Major);
        builder.ToTable(table, t => t.HasCheckConstraint(
            $"ck_{table}_kind", $"kind IN ({string.Join(", ", BasePointKinds.All.Select(k => $"'{k}'"))})"));
        builder.Property(p => p.NameJa).HasMaxLength(100);
        builder.HasCoordinate(table, p => p.X, "x");
        builder.HasCoordinate(table, p => p.Y, "y");
        // マップごとに取り出す(GET /api/v1/base-points?mapId=)
        builder.HasIndex(p => p.MapId);
        builder.HasData(MapPlaceSeed.BasePoints);
    }
}
