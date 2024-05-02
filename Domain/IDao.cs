namespace Domain;

public interface IDao<T> : IDisposable where T : class
{
    IEnumerable<T> GetAll();
    
    IEnumerable<T> GetAllCopies();

    Task<T?> TryGetByKey(params object[] keyValues);
    
    //Task<T?> TryGetCopyByKey(params object[] keyValues);

    Task SaveChanges();
}