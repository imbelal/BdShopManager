namespace Common.ContextBase
{
    public interface IDbConnectionStringProvider
    {
        string ConnectionString { get; }
        string ReadOnlyConnectionString { get; }
    }
}
