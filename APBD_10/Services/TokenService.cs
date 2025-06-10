using System.Text;
using APBD_10.Models;

namespace APBD_10.Services;

public class TokenService : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        // Простая имитация токена (НЕ использовать в продакшене)
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Username}:{DateTime.UtcNow}"));
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}