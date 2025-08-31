using System.Linq.Expressions;

namespace ProductManagement.Db;

public interface IGenericDbOperation<T> where T : BaseEntity
{
    void save(T entity);
    void Update(T entity);
    T? FindById(Guid id, params Expression<Func<T,object>>[] includes);
    IList<T> Find(Expression<Func<T, bool>> predicate);
    ICollection<T> findAll(ICollection<int> idList);
    void  Delete(T entity);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);

}