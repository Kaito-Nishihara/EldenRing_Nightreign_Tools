# ビルドステージ
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# プロジェクトファイルだけを先にコピーしてリストアする(レイヤーキャッシュ活用)
COPY NightreignTools.Backend/NightreignTools.Backend.csproj NightreignTools.Backend/
RUN dotnet restore NightreignTools.Backend/NightreignTools.Backend.csproj

COPY NightreignTools.Backend/ NightreignTools.Backend/
RUN dotnet publish NightreignTools.Backend/NightreignTools.Backend.csproj -c Release -o /out --no-restore

# ランタイムステージ
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# compose の稼働の確認(/health)に使う
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /out .

# コンテナの間だけに公開する(実行環境 5.1)
ENV ASPNETCORE_URLS=http://+:8081
EXPOSE 8081

# root で動かさない。APP_UID は .NET のイメージが持つ一般の利用者
USER $APP_UID

ENTRYPOINT ["dotnet", "NightreignTools.Backend.dll"]
