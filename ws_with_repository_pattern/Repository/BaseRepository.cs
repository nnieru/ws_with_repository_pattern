using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ws_with_repository_pattern.Repository;

interface IBaseRepository<TEntity>
{
    public IEnumerable<TEntity> GetAll( Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    public TEntity GetById(object id);
    public void Insert(TEntity entity);
    public void Delete(object id);
    public void Delete(TEntity entity);
    public void Update(TEntity entity);
}

public class BaseRepository<T>: IBaseRepository<T> where T: class
{
    private Microsoft.EntityFrameworkCore.DbContext _context;
    private DbSet<T> _dbSet;

    public BaseRepository(Microsoft.EntityFrameworkCore.DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split
                     (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return orderBy(query).ToList();
        }
    
        return query.ToList();
    }

    public T GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public void Insert(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Delete(object id)
    {
        T entity = _dbSet.Find(id);
        Delete(entity);
    }

    public void Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}