using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class GenericRepositoryAsync<T> : IRepositoryAsync<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _entities;

    public GenericRepositoryAsync(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _entities.FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _entities.ToListAsync();
    }

    public virtual async Task<List<T>> GetAllWithIncludeAsync(List<string> properties)
    {
        var query = _entities.AsQueryable();

        foreach (var property in properties)
        {
            query = query.Include(property);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _entities.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<List<T>> GetAllWithFiltersAsync(Expression<Func<T, bool>> filter)
    {
        return await _entities.Where(filter).ToListAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        return await _entities.AnyAsync(filter);
    }
}
