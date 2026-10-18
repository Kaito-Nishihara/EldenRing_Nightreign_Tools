using Microsoft.EntityFrameworkCore;
using NightreignTools.Master.Application.Admin;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence;

namespace NightreignTools.Master.Infrastructure.Repositories;

/// <summary>
/// 管理の画面でマップの上の場所を出し入れする。4 つの種類を、DbContext の型ごとの集まり(Set)で同じに扱う。
/// 行を消す操作は持たない(決定の記録 0013)。
/// </summary>
internal sealed class MapPlaceAdminRepository(MasterDbContext db) : IMapPlaceAdminRepository
{
    public async Task<IReadOnlyList<TPlace>> ListAsync<TPlace>(CancellationToken cancellationToken) where TPlace : MapPlace =>
        await db.Set<TPlace>().AsNoTracking().OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public Task<TPlace?> FindAsync<TPlace>(int id, CancellationToken cancellationToken) where TPlace : MapPlace =>
        db.Set<TPlace>().SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

    public void Add<TPlace>(TPlace place) where TPlace : MapPlace => db.Set<TPlace>().Add(place);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    public Task<bool> JihenExistsAsync(string jihenId, CancellationToken cancellationToken) =>
        db.Jihen.AnyAsync(j => j.Id == jihenId, cancellationToken);
}
