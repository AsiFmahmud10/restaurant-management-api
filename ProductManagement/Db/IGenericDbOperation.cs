using System.Linq.Expressions;

namespace ProductManagement.db;

public interface IGenericDbOperation<T> where T : class
{
    void save(T entity);
    T? findById(int id);
    ICollection<T> find(Expression<Func<T, bool>> predicate);
    ICollection<T> findAll(ICollection<int> idList);
    void  delete(T entity);
}