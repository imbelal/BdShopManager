using Application.Services.Auth.Implementations;

namespace Application.Services.Common
{
    public class AppSettings
    {
        public int Iterations { get; set; }
        public string DefaultPassword { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }
}
