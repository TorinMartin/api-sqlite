using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicApi.Injection;
using MusicApi.Model;

namespace MusicApi.Data;

public interface IRepository<T>
{
    public Task<List<T>> GetAsync(params Expression<Func<T, object>>[] includes);
    public Task<T?> GetAsync(int id, params Expression<Func<T, object>>[] includes);
    public Task<long> InsertAsync(T entity);
    public Task<long> DeleteAsync(int id);
    public Task<long> UpdateAsync(int id, Action<T> del);
}

[Injectable(ServiceLifetime.Scoped, typeof(IRepository<Artist>), typeof(IRepository<Song>), typeof(IRepository<Album>))]
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DatabaseContext _dbContext;
    
    public Repository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<T>> GetAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    public async Task<T?> GetAsync(int id, params Expression<Func<T, object>>[] includes) => await GetByIdAsync(id, includes);
    
    public async Task<long> InsertAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return await _dbContext.SaveChangesAsync();
    }
    
    public async Task<long> DeleteAsync(int id)
    {
        var deleted = 0;
            
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            _dbContext.Set<T>().Remove(entity);
            deleted = await _dbContext.SaveChangesAsync();
        }

        return deleted;
    }

    /// <summary>
    /// Updates an entity that matches the provided ID
    /// </summary>
    /// <param name="id">Entity ID to update</param>
    /// <param name="del">Delegate to update properties of Entity</param>
    /// <returns></returns>
    public async Task<long> UpdateAsync(int id, Action<T> del)
    {
        var updated = 0;
            
        var entityToUpdate = await GetByIdAsync(id);
        if (entityToUpdate is not null)
        {
            del(entityToUpdate);
            entityToUpdate.LastModified = DateTime.UtcNow;
                
            updated = await _dbContext.SaveChangesAsync();
        }

        return updated;
    }

    private async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }
}