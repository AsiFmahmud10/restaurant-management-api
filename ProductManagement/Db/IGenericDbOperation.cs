using System.Linq.Expressions;

namespace ProductManagement.Db;

public interface IGenericDbOperation<T> where T : class
{
    void save(T entity);
    void Update(T entity);
    T? FindById(Guid id);
    ICollection<T> find(Expression<Func<T, bool>> predicate);
    ICollection<T> findAll(ICollection<int> idList);
    void  Delete(T entity);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);

}