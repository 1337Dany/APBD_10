using System.Text;

namespace APBD_10.Services;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        // Простое хеширование (НЕ использовать в продакшене)
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
    }

    public static bool Verify(string password, string hash)
    {
        return Hash(password) == hash;
    }
}
