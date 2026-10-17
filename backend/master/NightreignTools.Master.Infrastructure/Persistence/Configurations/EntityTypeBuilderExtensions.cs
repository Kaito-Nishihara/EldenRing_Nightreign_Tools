using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence.Seed;

namespace NightreignTools.Master.Infrastructure.Persistence.Configurations;

/// <summary>
/// 構成のクラスで繰り返す設定をまとめる。
/// </summary>
internal static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// 初期データを持つ表の識別子。初期データは決まった識別子で入れるので、自動の採番は初期データの後から始める。
    /// </summary>
    public static PropertyBuilder<int> HasSeededIdentity<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, int>> key)
        where T : class =>
        builder.Property(key)
            .UseIdentityByDefaultColumn()
            .HasIdentityOptions(startValue: SeedData.IdLimit + 1);

    /// <summary>座標か半径の列。numeric(7,2) で、0 以上 1024 以下の検査の制約を付ける(データベースの共通仕様 5.3)。</summary>
    public static PropertyBuilder<decimal> HasCoordinate<T>(this EntityTypeBuilder<T> builder, string table, Expression<Func<T, decimal>> property, string column)
        where T : class
    {
        builder.ToTable(table, t => t.HasCheckConstraint($"ck_{table}_{column}", $"{column} BETWEEN 0 AND 1024"));
        return builder.Property(property).HasPrecision(7, 2);
    }

    /// <summary>マップの上の場所に共通の設定。識別子の採番と、非表示の日時(決定の記録 0013)。</summary>
    public static void HasMapPlaceDefaults<T>(this EntityTypeBuilder<T> builder)
        where T : MapPlace
    {
        builder.HasSeededIdentity(p => p.Id);
        builder.Property(p => p.HiddenAt);
        builder.Ignore(p => p.IsHidden);
    }
}
