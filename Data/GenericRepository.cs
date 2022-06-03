using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectX.Models;

namespace ProjectX.Data
{
    public class GenericRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int pageNumber = 1,
            int pageSize = 10)
        {
            IQueryable<TEntity> query = _dbSet;
            
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            
            //Pagination logic:
            int skip = pageSize * (pageNumber - 1);
            int count = query.Count();
            int lastPage = (int)Math.Ceiling((double)count / pageSize);
            if (pageNumber > lastPage)
            {
                return null;
            }
            else
            {
                return orderBy == null ? await query.Skip(skip).Take(pageSize).ToListAsync()
                    : await orderBy(query).Skip(skip).Take(pageSize).ToListAsync();
            }
        }

        public virtual async Task<TEntity> GetById(Guid? id, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }
        
        public virtual void Insert(TEntity entity)
            => _dbSet.Add(entity);
        
        public virtual async Task Delete(object id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Edit(TEntity entityToEdit)
        {
            _dbSet.Attach(entityToEdit);
            _context.Entry(entityToEdit).State = EntityState.Modified;
        }
        
        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();
        }
    }
}