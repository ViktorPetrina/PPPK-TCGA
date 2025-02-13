namespace DataLayer.Repository
{
    public interface ISimpleRepository<T> where T : class
    {
        Task Insert(T entity);
        Task<IEnumerable<T>> ReadAll();
    }
}
