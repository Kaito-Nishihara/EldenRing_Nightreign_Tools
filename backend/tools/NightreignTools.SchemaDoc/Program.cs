using NightreignTools.SchemaDoc;

// テーブルの定義書を作る(データベースの共通仕様 5.8)。
// 使い方: dotnet run --project tools/NightreignTools.SchemaDoc -- <書き出す先のディレクトリ>
if (args.Length != 1)
{
    Console.Error.WriteLine("書き出す先のディレクトリを 1 つ渡してください。例: /src/docs/database");
    return 1;
}

var outputDirectory = args[0];
Directory.CreateDirectory(outputDirectory);

foreach (var schema in ServiceSchema.All)
{
    var path = Path.Combine(outputDirectory, $"{schema.Service}.md");
    await File.WriteAllTextAsync(path, SchemaDocumentWriter.Write(schema));
    Console.WriteLine($"{path} を書いた");
}

return 0;
