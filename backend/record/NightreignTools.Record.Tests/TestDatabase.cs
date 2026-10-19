using Npgsql;

namespace NightreignTools.Record.Tests;

/// <summary>
/// テスト用のデータベース(サービスの共通構成 5.9)。InMemory を使わず、本物の PostgreSQL に対して走らせる。
/// サーバーと管理者の接続文字列は環境変数 <c>ConnectionStrings__Test</c> から読み、データベースの名前はこのサービスのものにする。
/// </summary>
internal static class TestDatabase
{
    public const string Name = "nightreign_record_test";

    private static readonly Lazy<bool> ResetOnce = new(() =>
    {
        Drop(Name);
        return true;
    });

    /// <summary>
    /// テスト用のデータベースへの接続文字列。
    /// </summary>
    public static string ConnectionString => ForDatabase(Name);

    /// <summary>
    /// テストの始めに 1 回だけ、前の実行で残ったデータベースを消す。
    /// 作り直しは起動のときのマイグレーションが行う。
    /// </summary>
    public static void EnsureFresh() => _ = ResetOnce.Value;

    /// <summary>
    /// 同じサーバーの、指定した名前のデータベースへの接続文字列。
    /// </summary>
    /// <param name="database">つなぐデータベースの名前。</param>
    /// <returns>接続文字列。</returns>
    public static string ForDatabase(string database)
    {
        var server = Environment.GetEnvironmentVariable("ConnectionStrings__Test")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__Test がありません。docker compose run --rm backend-sdk dotnet test で走らせてください");
        return new NpgsqlConnectionStringBuilder(server) { Database = database }.ToString();
    }

    /// <summary>
    /// データベースを消す。
    /// つないでいる接続があっても消す。
    /// </summary>
    /// <param name="database">消すデータベースの名前。ないときは何もしない。</param>
    public static void Drop(string database)
    {
        using var connection = new NpgsqlConnection(ForDatabase("postgres"));
        connection.Open();
        using var command = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{database}\" WITH (FORCE)", connection);
        command.ExecuteNonQuery();

        // 消したデータベースへの接続が、接続の使い回しの置き場に残っていると、次のテストが切れた接続を使ってしまう
        NpgsqlConnection.ClearAllPools();
    }

    /// <summary>
    /// SQL を流し、1 列目の値を並べて返す。
    /// </summary>
    /// <param name="sql">流す SQL。テストの中で書いた定数だけを渡す。</param>
    /// <returns>1 列目の値を文字にしたもの。</returns>
    public static async Task<List<string>> QueryAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        var values = new List<string>();
        while (await reader.ReadAsync())
        {
            values.Add(Convert.ToString(reader.GetValue(0), System.Globalization.CultureInfo.InvariantCulture) ?? "");
        }

        return values;
    }

    /// <summary>
    /// SQL を流す。
    /// 結果は読まない。
    /// </summary>
    /// <param name="sql">流す SQL。テストの中で書いた定数だけを渡す。</param>
    public static async Task ExecuteAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
