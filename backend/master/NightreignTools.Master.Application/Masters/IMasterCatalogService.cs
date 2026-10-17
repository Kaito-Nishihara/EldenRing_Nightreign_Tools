namespace NightreignTools.Master.Application.Masters;

/// <summary>プレイヤー向けのマスタの取得(マスタの管理 4.1)。</summary>
public interface IMasterCatalogService
{
    Task<IReadOnlyList<BossDto>> GetBossesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<JihenDto>> GetJihenAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<SpawnPointDto>> GetSpawnPointsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ShrinkZoneDto>> GetShrinkZonesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<SecondShrinkPointDto>> GetSecondShrinkPointsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// マップごとの拠点候補地。<paramref name="mapId"/> を省くと通常のマップ。
    /// 通常でも地変の識別子でもなければ <see cref="SharedKernel.NotFoundException"/> を投げる。
    /// </summary>
    Task<IReadOnlyList<BasePointDto>> GetBasePointsAsync(string? mapId, CancellationToken cancellationToken);

    Task<IReadOnlyList<NightBossDto>> GetNightBossesAsync(CancellationToken cancellationToken);
}
