using System.Linq.Expressions;

namespace ACB_Prueba_Tecnica.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByCondition(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAll();
        Task<List<T>> GetAllByCondition(Expression<Func<T, bool>> predicate);

        Task Add(T entity);
        Task Update(T entity);

        Task Delete(T entity);
        Task Deactivate(T entity);
    }
}
