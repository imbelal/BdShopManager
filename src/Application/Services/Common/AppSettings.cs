using Application.Services.Auth.Implementations;

namespace Application.Services.Common
{
    public class AppSettings
    {
        public int Iterations { get; set; }
        public string DefaultPassword { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public AzureStorageSettings AzureStorage { get; set; }
    }

    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; }
        public string ProductPhotoContainer { get; set; } = "product-photos";
        public string BaseUrl { get; set; }
    }
}
