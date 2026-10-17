# record-service。ビルドの文脈は backend/(共通の部品 shared/ を使うため。実行環境 5.1)

# ビルドステージ
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# プロジェクトファイルだけを先にコピーしてリストアする(レイヤーキャッシュ活用)
COPY Directory.Build.props Directory.Packages.props ./
COPY shared/NightreignTools.SharedKernel/*.csproj shared/NightreignTools.SharedKernel/
COPY shared/NightreignTools.ServiceDefaults/*.csproj shared/NightreignTools.ServiceDefaults/
COPY record/NightreignTools.Record.Domain/*.csproj record/NightreignTools.Record.Domain/
COPY record/NightreignTools.Record.Application/*.csproj record/NightreignTools.Record.Application/
COPY record/NightreignTools.Record.Infrastructure/*.csproj record/NightreignTools.Record.Infrastructure/
COPY record/NightreignTools.Record.Api/*.csproj record/NightreignTools.Record.Api/
RUN dotnet restore record/NightreignTools.Record.Api/NightreignTools.Record.Api.csproj

COPY shared/ shared/
COPY record/ record/
RUN dotnet publish record/NightreignTools.Record.Api/NightreignTools.Record.Api.csproj -c Release -o /out --no-restore

# ランタイムステージ
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# compose の稼働の確認(/health)に使う
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /out .

# コンテナの間だけに公開する(実行環境 5.1)
ENV ASPNETCORE_URLS=http://+:8082
EXPOSE 8082

# root で動かさない。APP_UID は .NET のイメージが持つ一般の利用者
USER $APP_UID

ENTRYPOINT ["dotnet", "NightreignTools.Record.Api.dll"]
