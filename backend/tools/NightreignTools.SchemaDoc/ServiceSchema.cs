using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Master.Infrastructure.Persistence;
using NightreignTools.Record.Infrastructure.Persistence;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.SchemaDoc;

/// <summary>
/// 定義書を作る 1 つのサービス。サービスを足すときは <see cref="All"/> に 1 行足す。
/// </summary>
/// <param name="Service">サービスの名前。定義書のファイルの名前にも使う。例: master</param>
/// <param name="Database">データベースの名前。例: nightreign_master</param>
/// <param name="LoadModel">EF Core の設計時のモデルを組み立てる処理</param>
internal sealed record ServiceSchema(string Service, string Database, Func<IModel> LoadModel)
{
    public static IReadOnlyList<ServiceSchema> All { get; } =
    [
        Create<MasterDbContext>("master", "nightreign_master", options => new(options)),
        Create<RecordDbContext>("record", "nightreign_record", options => new(options)),
    ];

    /// <summary>
    /// マイグレーションを作るときと同じ設定でモデルを組み立てる。
    /// 説明(コメント)は設計時のモデルにだけ残るので、そちらを使う。接続文字列は仮のもので、データベースにはつながない。
    /// </summary>
    private static ServiceSchema Create<TContext>(string service, string database, Func<DbContextOptions<TContext>, TContext> create)
        where TContext : DbContext =>
        new(service, database, () =>
        {
            var options = new DbContextOptionsBuilder<TContext>();
            DatabaseServiceCollectionExtensions.UseServiceDatabase(options, $"Host=localhost;Database={database}");
            using var context = create(options.Options);
            return context.GetService<IDesignTimeModel>().Model;
        });
}
