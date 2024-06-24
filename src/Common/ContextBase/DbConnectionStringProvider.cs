namespace Common.ContextBase
{
    public class DbConnectionStringProvider : IDbConnectionStringProvider
    {
        public string ConnectionString { get; private set; }
        public string ReadOnlyConnectionString { get; private set; }

        public DbConnectionStringProvider(string connectionString, string readOnlyConnectionString)
        {
            ConnectionString = connectionString;
            ReadOnlyConnectionString = readOnlyConnectionString;
        }
    }
}
