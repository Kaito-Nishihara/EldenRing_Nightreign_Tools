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
    /// <typeparam name="T">エンティティの型。</typeparam>
    /// <param name="builder">エンティティの設定。</param>
    /// <param name="key">識別子のプロパティ。</param>
    /// <returns>識別子の列の設定。</returns>
    public static PropertyBuilder<int> HasSeededIdentity<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, int>> key)
        where T : class =>
        builder.Property(key)
            .UseIdentityByDefaultColumn()
            .HasIdentityOptions(startValue: SeedData.IdLimit + 1);

    /// <summary>
    /// 座標か半径の列。
    /// numeric(7,2) で、0 以上 1024 以下の検査の制約を付ける(データベースの共通仕様 5.3)。
    /// </summary>
    /// <typeparam name="T">エンティティの型。</typeparam>
    /// <param name="builder">エンティティの設定。</param>
    /// <param name="table">テーブルの名前。検査の制約の名前に使う。</param>
    /// <param name="property">座標か半径のプロパティ。</param>
    /// <param name="column">列の名前(スネークケース)。検査の制約の式に使う。</param>
    /// <returns>座標か半径の列の設定。</returns>
    public static PropertyBuilder<decimal> HasCoordinate<T>(this EntityTypeBuilder<T> builder, string table, Expression<Func<T, decimal>> property, string column)
        where T : class
    {
        builder.ToTable(table, t => t.HasCheckConstraint($"ck_{table}_{column}", $"{column} BETWEEN 0 AND 1024"));
        return builder.Property(property).HasPrecision(7, 2);
    }

    /// <summary>
    /// マップの上の場所に共通の設定。
    /// 識別子の採番と、非表示の日時(決定の記録 0013)。
    /// </summary>
    /// <typeparam name="T">マップの上の場所のエンティティの型。</typeparam>
    /// <param name="builder">エンティティの設定。</param>
    public static void HasMapPlaceDefaults<T>(this EntityTypeBuilder<T> builder)
        where T : MapPlace
    {
        builder.HasSeededIdentity(p => p.Id);
        builder.Property(p => p.HiddenAt);
        builder.Ignore(p => p.IsHidden);
    }
}
