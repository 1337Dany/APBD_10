using APBD_10.Data;
using APBD_10.DTOs;
using APBD_10.Models;
using APBD_10.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(InitialsDto dto)
    {
        if (_context.Users.Any(u => u.Username == dto.Username))
            return BadRequest("User already exists.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(InitialsDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);
        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return Unauthorized();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new { accessToken, refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Unauthorized();

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
    }
}
