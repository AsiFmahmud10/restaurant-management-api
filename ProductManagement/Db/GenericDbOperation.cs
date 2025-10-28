using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Db;

public class GenericDbOperation<T>(ApplicationDbContext dbContext) :  IGenericDbOperation<T> where T : BaseEntity
{
    public void save(T entity)
    {
        dbContext.Set<T>().Add(entity);
        dbContext.SaveChanges();
    }
    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
        dbContext.SaveChanges();
    }

    public T? FindById(Guid id, params Expression<Func<T,object>>[] includes )
    {
        IQueryable<T> query = dbContext.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
       
        return query.FirstOrDefault(t => t.Id.Equals(id) );
    }
    

    public IList<T> Find(Expression<Func<T, bool>> predicate)
    {
        return dbContext.Set<T>().Where(predicate).ToList();
    }

    public T? FirstOrDefault( Expression<Func<T,bool>> predicate)
    {
        return dbContext.Set<T>().FirstOrDefault(predicate);
    }

    public ICollection<T> findAll(ICollection<int> idList)
    {
        return null;
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        dbContext.SaveChanges();
    }
}