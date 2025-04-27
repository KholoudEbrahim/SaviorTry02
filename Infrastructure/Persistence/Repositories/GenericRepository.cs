using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SaviorDbContext _context;
                protected readonly DbSet<T> _dbSet;

        public GenericRepository(SaviorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        //public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        //{
        //    return await _context.Set<T>().Where(predicate).ToListAsync();
        //}
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public IQueryable<T> GetQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                                 .Where(predicate)
                                 .ToListAsync();
        }
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
    }
}
