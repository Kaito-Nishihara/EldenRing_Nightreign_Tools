using NightreignTools.Master.Domain.Entities;

namespace NightreignTools.Master.Application.Admin;

/// <summary>
/// 管理の画面でマップの上の場所を出し入れするためのポート。実装は Infrastructure に置く(サービスの共通構成 5.3)。
/// 4 つの種類(開始地点、収縮候補地、第二収縮地点、拠点候補地)を同じ形で扱う。行を消す操作は持たない(決定の記録 0013)。
/// </summary>
public interface IMapPlaceAdminRepository
{
    /// <summary>
    /// 非表示の行も含めて、識別子の順に返す。
    /// </summary>
    /// <typeparam name="TPlace">場所の種類。</typeparam>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>その種類のすべての行。</returns>
    Task<IReadOnlyList<TPlace>> ListAsync<TPlace>(CancellationToken cancellationToken) where TPlace : MapPlace;

    /// <summary>
    /// 直すために、変更を追跡する状態で 1 行を返す。
    /// なければ null。
    /// </summary>
    /// <typeparam name="TPlace">場所の種類。</typeparam>
    /// <param name="id">識別子。</param>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>見つかった行。なければ null。</returns>
    Task<TPlace?> FindAsync<TPlace>(int id, CancellationToken cancellationToken) where TPlace : MapPlace;

    /// <summary>
    /// 新しい行を足す。
    /// 保存は <see cref="SaveChangesAsync"/> で行う。
    /// </summary>
    /// <typeparam name="TPlace">場所の種類。</typeparam>
    /// <param name="place">足す行。識別子は保存のときに振られる。</param>
    void Add<TPlace>(TPlace place) where TPlace : MapPlace;

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 地変の識別子があるか。
    /// 拠点候補地のマップの確かめに使う。
    /// </summary>
    /// <param name="jihenId">確かめる地変の識別子。</param>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>あれば true。</returns>
    Task<bool> JihenExistsAsync(string jihenId, CancellationToken cancellationToken);
}
