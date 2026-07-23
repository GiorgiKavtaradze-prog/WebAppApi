using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAppApi.Data;
using WebAppApi.Dto;
using WebAppApi.IService;
using WebAppApi.Common;

namespace WebAppApi.Services;

public class AuthService : IAuthService
{
    private const string InvalidRequestMessage = "Please provide all required details.";
    private const string RegistrationSuccessMessage = "User registered successfully.";
    private const string LoginSuccessMessage = "Login successful.";

    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<Result<TokenDto>> LoginUser(UserLoginDto dto)
    {
        try
        {
            if (dto is null
                || string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Result<TokenDto>.Failure(InvalidRequestMessage);
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var existingUser = await _context.AccountUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

            if (existingUser == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", normalizedEmail);
                return Result<TokenDto>.Failure("User not found. Please check your credentials.");
            }

            var passwordHasher = new PasswordHasher<string>();
            var verifyPassword = passwordHasher.VerifyHashedPassword(
                normalizedEmail,
                existingUser.Password ?? string.Empty,
                dto.Password);

            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", normalizedEmail);
                return Result<TokenDto>.Failure("Invalid password.");
            }

            var token = GenerateJwtToken(existingUser);
            var tokenDto = new TokenDto
            {
                Token = token,
                Message = LoginSuccessMessage
            };

            _logger.LogInformation("User logged in successfully: {Email}", normalizedEmail);
            return Result<TokenDto>.Success(tokenDto, LoginSuccessMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", dto?.Email);
            throw;
        }
    }

    public async Task<Result> RegisterUser(UserRegisterDto dto)
    {
        try
        {
            if (dto is null
                || string.IsNullOrWhiteSpace(dto.Name)
                || string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Result.Failure(InvalidRequestMessage);
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var existingUser = await _context.AccountUsers
                .AsNoTracking()
                .AnyAsync(x => x.Email == normalizedEmail);

            if (existingUser)
            {
                return Result.Failure("User already exists. Please use a different email.");
            }

            var user = new Entities.User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Email = normalizedEmail,
                Password = HashPassword(dto.Password)
            };

            await _context.AccountUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Email}", normalizedEmail);
            return Result.Success(RegistrationSuccessMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", dto?.Email);
            throw;
        }
    }

    private string GenerateJwtToken(Entities.User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<string>();
        return passwordHasher.HashPassword(string.Empty, password);
    }
}
