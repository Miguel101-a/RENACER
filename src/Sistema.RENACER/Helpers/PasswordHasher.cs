using System.Security.Cryptography;
using System.Text;

namespace Sistema.RENACER.Helpers;

public static class PasswordHasher
{
    public static bool IsBCryptHash(string hash)
        => hash != null && (hash.StartsWith("$2a$") || hash.StartsWith("$2b$"));

    public static string HashBCrypt(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public static bool VerifyBCrypt(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);

    /// <summary>
    /// Reproduce HASHBYTES('SHA2_256', password) de SQL Server: hex UPPERCASE con encoding Unicode.
    /// </summary>
    public static string HashSha256Legacy(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.Unicode.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
    }

    public static bool VerifyLegacySha256(string password, string hash)
        => HashSha256Legacy(password) == hash;

    public static bool Verify(string password, string storedHash)
    {
        if (IsBCryptHash(storedHash))
            return VerifyBCrypt(password, storedHash);
        return VerifyLegacySha256(password, storedHash);
    }
}
