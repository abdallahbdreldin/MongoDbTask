using System.Linq.Expressions;

namespace TodayWebAPi.DAL.Repos.Generic
{
    public interface IGenericRepo<T> where T : class
    {
        Task<T> GetByIdAsync(string id); 
        Task<IReadOnlyList<T>> GetAllAsync();
        Task AddAsync(T item);
        Task DeleteAsync(string id);
        Task UpdateAsync(T item);

        Task<T> FindOneAsync(Expression<Func<T, bool>> filter);
        Task<IReadOnlyList<T>> FindAllAsync(Expression<Func<T, bool>> filter);
    }

}
