using System.Linq.Expressions;

namespace RealEstateApp.Domain.Interfaces;

public interface IRepositoryAsync<T> where T : class
{
    Task<T?> GetByIdAsync(int id); 
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllWithIncludeAsync(List<string> properties);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<List<T>> GetAllWithFiltersAsync(Expression<Func<T, bool>> filter);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
}
