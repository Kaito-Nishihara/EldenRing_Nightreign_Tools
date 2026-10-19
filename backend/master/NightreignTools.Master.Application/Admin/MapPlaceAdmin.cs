using NightreignTools.Master.Domain.Entities;
using NightreignTools.SharedKernel;

namespace NightreignTools.Master.Application.Admin;

/// <summary>
/// 1 つの種類のマップの上の場所の、管理用の一覧、登録、修正(マスタの管理 4.2)。
/// エンドポイントは種類を知らずに、この形だけを使う。
/// </summary>
/// <typeparam name="TRequest">登録と修正の入力。</typeparam>
/// <typeparam name="TDto">管理用の出力。</typeparam>
public interface IMapPlaceAdmin<TRequest, TDto>
{
    /// <summary>
    /// 非表示の行も含めた一覧
    /// </summary>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>識別子の順の行。</returns>
    Task<IReadOnlyList<TDto>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 登録し、作った行を返す
    /// </summary>
    /// <param name="request">登録の入力。</param>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>作った行。</returns>
    Task<TDto> CreateAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// 修正、非表示、表示に戻すを行い、直した行を返す。
    /// 行がなければ 404 にする
    /// </summary>
    /// <param name="id">直す行の識別子。</param>
    /// <param name="request">修正の入力。</param>
    /// <param name="cancellationToken">取り消しの合図。</param>
    /// <returns>直した行。</returns>
    Task<TDto> UpdateAsync(int id, TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// 4 つの種類に共通の流れ。種類ごとの違い(作り方、直し方、出力の形)だけを派生のクラスに書く。
/// 種類を足すときは、このクラスを継いだクラスを 1 つ足して登録する。
/// </summary>
/// <typeparam name="TPlace">場所の種類のエンティティ。</typeparam>
/// <typeparam name="TRequest">登録と修正の入力。</typeparam>
/// <typeparam name="TDto">管理用の出力。</typeparam>
/// <param name="repository">場所を出し入れする。</param>
/// <param name="clock">非表示にした日時を決める。</param>
internal abstract class MapPlaceAdmin<TPlace, TRequest, TDto>(IMapPlaceAdminRepository repository, TimeProvider clock)
    : IMapPlaceAdmin<TRequest, TDto>
    where TPlace : MapPlace
{
    /// <summary>
    /// 404 の文言に使う種類の名前。
    /// 例: 開始地点
    /// </summary>
    protected abstract string Label { get; }

    /// <summary>
    /// 入力の決まりを確かめて作る。
    /// 決まりに反するときは BusinessRuleException を投げる
    /// </summary>
    /// <param name="request">登録の入力。</param>
    /// <returns>保存の前の行。</returns>
    protected abstract TPlace Create(TRequest request);

    /// <summary>
    /// 入力の決まりを確かめて直す
    /// </summary>
    /// <param name="place">直す行。</param>
    /// <param name="request">修正の入力。</param>
    protected abstract void Update(TPlace place, TRequest request);

    /// <summary>
    /// 入力の isHidden。
    /// 省いたときは null
    /// </summary>
    /// <param name="request">登録か修正の入力。</param>
    /// <returns>入力の isHidden。省いたときは null。</returns>
    protected abstract bool? IsHidden(TRequest request);

    protected abstract TDto ToDto(TPlace place);

    /// <summary>
    /// データベースを見る必要のある確かめ。
    /// 要らなければ何もしない
    /// </summary>
    /// <param name="request">登録か修正の入力。</param>
    /// <param name="cancellationToken">取り消しの合図。</param>
    protected virtual Task ValidateAsync(TRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

    protected IMapPlaceAdminRepository Repository => repository;

    /// <summary>
    /// 必須の数を取り出す。API の入力の検証で省いたものは止めているが、ほかの呼び出し元からの null も 400 にする
    /// </summary>
    /// <param name="value">入力の数。</param>
    /// <param name="name">例外の文言に入れる項目の名前。</param>
    /// <returns>null でない数。</returns>
    protected static decimal Required(decimal? value, string name) =>
        value ?? throw new BusinessRuleException($"{name} は必須です");

    public async Task<IReadOnlyList<TDto>> ListAsync(CancellationToken cancellationToken) =>
        (await repository.ListAsync<TPlace>(cancellationToken)).Select(ToDto).ToList();

    public async Task<TDto> CreateAsync(TRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken);
        var place = Create(request);
        place.SetHidden(IsHidden(request) ?? false, clock.GetUtcNow().UtcDateTime);
        repository.Add(place);
        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(place);
    }

    public async Task<TDto> UpdateAsync(int id, TRequest request, CancellationToken cancellationToken)
    {
        var place = await repository.FindAsync<TPlace>(id, cancellationToken)
            ?? throw new NotFoundException($"{Label} '{id}' は存在しません");
        await ValidateAsync(request, cancellationToken);
        Update(place, request);
        if (IsHidden(request) is { } hidden)
        {
            place.SetHidden(hidden, clock.GetUtcNow().UtcDateTime);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(place);
    }
}
