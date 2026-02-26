using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TradeFlow.Identity.Domain.Interfaces;

namespace TradeFlow.Identity.Infrastructure.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32);

        var result = new byte[48];
        Buffer.BlockCopy(salt, 0, result, 0, 16);
        Buffer.BlockCopy(hash, 0, result, 16, 32);
        return Convert.ToBase64String(result);
    }

    public bool Verify(string password, string hashedPassword)
    {
        var decoded = Convert.FromBase64String(hashedPassword);
        if (decoded.Length != 48) return false;

        var salt = new byte[16];
        Buffer.BlockCopy(decoded, 0, salt, 0, 16);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32);

        for (int i = 0; i < 32; i++)
            if (decoded[i + 16] != hash[i]) return false;

        return true;
    }
}
