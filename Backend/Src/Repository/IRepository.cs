using System.Linq.Expressions;

namespace Backend.Src.Repository
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);
        Task InsertAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
    }
}
