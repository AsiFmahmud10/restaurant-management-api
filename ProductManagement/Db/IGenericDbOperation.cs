using System.Linq.Expressions;

namespace ProductManagement.Db;

public interface IGenericDbOperation<T> where T : BaseEntity
{
    void save(T entity);
    void Update(T entity);
    void SaveChanges();
    bool Exist(Expression<Func<T, bool>> predicate);
    T? FindById(Guid id, params Expression<Func<T, object?>>[] includes);
    IList<T> Find(Expression<Func<T, bool>> predicate);
    List<T> FindAll(List<Guid> idList);
    void Delete(T entity);
    public ICollection<T> GetAll();
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);
}