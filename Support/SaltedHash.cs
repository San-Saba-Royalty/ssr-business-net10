using System.Security.Cryptography;
using System.Text;

namespace SSRBusiness.Support;

/// <summary>
/// Handles password hashing with salt for secure storage.
/// Converted from VB.NET SaltedHash class.
/// 
/// NOTE: Original used SHA1 - consider upgrading to SHA256 for new passwords.
/// This implementation uses SHA256 by default but can use SHA1 for backward compatibility.
/// </summary>
public class SaltedHash
{
    private const int SaltLength = 10;
    private readonly bool _useSha1ForCompatibility;

    public string Salt { get; }
    public string Hash { get; }

    private SaltedHash(string salt, string hash, bool useSha1 = false)
    {
        Salt = salt;
        Hash = hash;
        _useSha1ForCompatibility = useSha1;
    }

    /// <summary>
    /// Creates a new salted hash from a plain text password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="useSha1ForCompatibility">Use SHA1 for backward compatibility with old VB system</param>
    /// <returns>SaltedHash instance</returns>
    public static SaltedHash Create(string password, bool useSha1ForCompatibility = true)
    {
        var salt = CreateSalt();
        var hash = CalculateHash(salt, password, useSha1ForCompatibility);
        return new SaltedHash(salt, hash, useSha1ForCompatibility);
    }

    /// <summary>
    /// Creates a SaltedHash from existing salt and hash values
    /// </summary>
    /// <param name="salt">Existing salt</param>
    /// <param name="hash">Existing hash</param>
    /// <param name="useSha1ForCompatibility">Use SHA1 for backward compatibility</param>
    /// <returns>SaltedHash instance</returns>
    public static SaltedHash Create(string salt, string hash, bool useSha1ForCompatibility = true)
    {
        return new SaltedHash(salt, hash, useSha1ForCompatibility);
    }

    /// <summary>
    /// Verifies a password against the stored hash
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <returns>True if password matches, false otherwise</returns>
    public bool Verify(string password)
    {
        var calculatedHash = CalculateHash(Salt, password, _useSha1ForCompatibility);
        return Hash.Equals(calculatedHash, StringComparison.Ordinal);
    }

    private static string CreateSalt()
    {
        var randomBytes = CreateRandomBytes(SaltLength);
        return Convert.ToBase64String(randomBytes);
    }

    private static byte[] CreateRandomBytes(int length)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }

    private static string CalculateHash(string salt, string password, bool useSha1)
    {
        var data = ToByteArray(salt + password);
        var hashBytes = CalculateHashBytes(data, useSha1);
        return Convert.ToBase64String(hashBytes);
    }

    private static byte[] CalculateHashBytes(byte[] data, bool useSha1)
    {
        if (useSha1)
        {
            // Use SHA1 for backward compatibility with old VB.NET system
            #pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
            using var sha1 = SHA1.Create();
            return sha1.ComputeHash(data);
            #pragma warning restore CA5350
        }
        else
        {
            // Use SHA256 for new implementations (more secure)
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(data);
        }
    }

    private static byte[] ToByteArray(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}
