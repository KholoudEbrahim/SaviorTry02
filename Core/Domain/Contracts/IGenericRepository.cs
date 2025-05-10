using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{ 
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate); 
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> GetQueryable();

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
       Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);


        Task<T?> FirstOrDefaultAsync(
      Expression<Func<T, bool>> predicate,
      params Expression<Func<T, object>>[] includes);

        Task<T?> FirstOrDefaultWithIncludesAsync(
    Expression<Func<T, bool>> predicate,
    Func<IQueryable<T>, IQueryable<T>> includes);

    }

}
