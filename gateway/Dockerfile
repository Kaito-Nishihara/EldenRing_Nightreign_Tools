# ビルドステージ
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# プロジェクトファイルをコピーしてリストア（レイヤーキャッシュ活用）
COPY *.csproj .
RUN dotnet restore

# ソースコードをコピーしてパブリッシュ
COPY . .
RUN dotnet publish -c Release -o /out

# ランタイムステージ
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .

# ASP.NET Core のデフォルトポートを8080に設定
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Gateway.dll"]
