using System.Linq.Expressions;

namespace ProductManagement.db;

public class GenericDbOperation<T>(ApplicationDbContext dbContext) :  IGenericDbOperation<T> where T : class
{
    public void save(T entity)
    {
        dbContext.Set<T>().Add(entity);
        dbContext.SaveChanges();
    }

    public T? findById(int id)
    {
        return dbContext.Set<T>().Find(id);
    }

    public ICollection<T> find(Expression<Func<T, bool>> predicate)
    {
        return dbContext.Set<T>().Where(predicate).ToList();
    }

    public ICollection<T> findAll(ICollection<int> idList)
    {
        return null;
    }

    public void delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        dbContext.SaveChanges();
    }
}