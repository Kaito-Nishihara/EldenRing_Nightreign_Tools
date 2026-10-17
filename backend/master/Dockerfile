# master-service。ビルドの文脈は backend/(共通の部品 shared/ を使うため。実行環境 5.1)

# ビルドステージ
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# プロジェクトファイルだけを先にコピーしてリストアする(レイヤーキャッシュ活用)
COPY Directory.Build.props Directory.Packages.props ./
COPY shared/NightreignTools.SharedKernel/*.csproj shared/NightreignTools.SharedKernel/
COPY shared/NightreignTools.ServiceDefaults/*.csproj shared/NightreignTools.ServiceDefaults/
COPY master/NightreignTools.Master.Domain/*.csproj master/NightreignTools.Master.Domain/
COPY master/NightreignTools.Master.Application/*.csproj master/NightreignTools.Master.Application/
COPY master/NightreignTools.Master.Infrastructure/*.csproj master/NightreignTools.Master.Infrastructure/
COPY master/NightreignTools.Master.Api/*.csproj master/NightreignTools.Master.Api/
RUN dotnet restore master/NightreignTools.Master.Api/NightreignTools.Master.Api.csproj

COPY shared/ shared/
COPY master/ master/
RUN dotnet publish master/NightreignTools.Master.Api/NightreignTools.Master.Api.csproj -c Release -o /out --no-restore

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

ENTRYPOINT ["dotnet", "NightreignTools.Master.Api.dll"]
