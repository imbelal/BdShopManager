namespace Domain.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        string CreateHash(string password);
        (bool Verified, bool NeedsUpgrade) VerifyPassword(string hash, string password);
    }
}
