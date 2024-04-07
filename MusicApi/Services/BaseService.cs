using System.Linq.Expressions;
using MusicApi.Data;
using MusicApi.Model;

namespace MusicApi.Services;

public enum Action
{
    Inserted = 0,
    Deleted = 1,
    Updated = 2
}

public record ServiceResult<TResult>(bool HasError = false, string? Error = null, TResult? Result = default);
public record CountResult(string Action, long Processed);

public interface IBaseService<T>
{
    public Task<ServiceResult<List<T>>> GetAsync(params Expression<Func<T, object>>[] includes);
    public Task<ServiceResult<T>> GetAsync(int id, params Expression<Func<T, object>>[] includes);
    public Task<ServiceResult<CountResult>> InsertAsync(T entity);
    public Task<ServiceResult<CountResult>> DeleteAsync(int id);
    public Task<ServiceResult<CountResult>> UpdateAsync(int id, Action<T> del);
}

/// <summary>
/// Generic service that will handle CRUD operations for Artist, Album and Song entities
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
{
    private readonly IRepository<T> _repository;
    
    public BaseService(IRepository<T> repo)
    {
        _repository = repo;
    }

    public async Task<ServiceResult<List<T>>> GetAsync(params Expression<Func<T, object>>[] includes) => await ExecuteSafelyAsync(async () => await _repository.GetAsync(includes));

    public async Task<ServiceResult<T>> GetAsync(int id, params Expression<Func<T, object>>[] includes) =>
        await ExecuteSafelyAsync(async () => await _repository.GetAsync(id, includes) ?? throw new Exception("Query yielded no results"));
    
    public async Task<ServiceResult<CountResult>> InsertAsync(T entity)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var inserted = await _repository.InsertAsync(entity);
            return new CountResult(Action.Inserted.ToString(), inserted);
        });
    }
    
    public async Task<ServiceResult<CountResult>> DeleteAsync(int id)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var deleted = await _repository.DeleteAsync(id);
            return new CountResult(Action.Deleted.ToString(), deleted);
        });
    }

    /// <summary>
    /// Updates an entity that matches the provided ID
    /// </summary>
    /// <param name="id">Entity ID to update</param>
    /// <param name="del">Delegate to update properties of Entity</param>
    /// <returns></returns>
    public async Task<ServiceResult<CountResult>> UpdateAsync(int id, Action<T> del)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var updated = await _repository.UpdateAsync(id, del);
            return new CountResult(Action.Updated.ToString(), updated);
        });
    }
    
    private async Task<ServiceResult<TResult>> ExecuteSafelyAsync<TResult>(Func<Task<TResult>> del)
    {
        try
        {
            var result = await del();
            return new ServiceResult<TResult>(Result: result);
        }
        catch (Exception ex)
        {
            return await HandleServiceError<TResult>(ex.Message);
        }
    }
    
    // Allows a custom message rather than exception msg
    protected ValueTask<ServiceResult<TResult>> HandleServiceError<TResult>(string message) => new (new ServiceResult<TResult>(true, message));
}