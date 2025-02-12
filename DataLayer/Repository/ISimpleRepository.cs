namespace DataLayer.Repository
{
    public interface ISimpleFileRepository
    {
        Task Create(string objectName, string filePath, string contentType);
        Task<IEnumerable<string>> ReadAll();
    }
}
