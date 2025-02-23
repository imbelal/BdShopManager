using Common.ContextBase;

namespace IntegrationTest.Helpers
{
    internal class TestDbConnectionProvider : IDbConnectionStringProvider
    {
        public string ConnectionString { get; set; }
        public string ReadOnlyConnectionString { get; set; }

        public TestDbConnectionProvider(string connectionString)
        {
            ConnectionString = connectionString;
            ReadOnlyConnectionString = connectionString;
        }
    }
}
