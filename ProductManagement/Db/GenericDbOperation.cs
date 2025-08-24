using System.Linq.Expressions;
namespace ProductManagement.Db;

public class GenericDbOperation<T>(ApplicationDbContext dbContext) :  IGenericDbOperation<T> where T : class
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

    public T? FindById(Guid id)
    {
        return dbContext.Set<T>().Find(id);
    }

    public ICollection<T> find(Expression<Func<T, bool>> predicate)
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