using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebAppApi.Common;
using WebAppApi.Data;
using WebAppApi.Dto;
using WebAppApi.IService;
using WebAppApi.Entities;
using WebAppApi.Services;
using Xunit;
using FluentAssertions;

namespace WebAppApi.Tests.Services;

public class AuthServiceTests
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _jwtSettings = new JwtSettings
        {
            Issuer = "webapp-client",
            Audience = "webapp-backend",
            Key = "04b83dfe0e9dadbc32f4354525b521412f0527beff39812c0dcf602aa1b5a648",
            ExpiresInMinutes = 30
        };
        _authService = new AuthService(_context, Options.Create(_jwtSettings), NullLogger<AuthService>.Instance);
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ReturnsSuccess()
    {
        var registerDto = new UserRegisterDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123"
        };

        var result = await _authService.RegisterUser(registerDto);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("User registered successfully.");
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ReturnsFailure()
    {
        var registerDto = new UserRegisterDto
        {
            Name = "Test User",
            Email = "duplicate@example.com",
            Password = "Password123"
        };

        await _authService.RegisterUser(registerDto);
        var result = await _authService.RegisterUser(registerDto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task RegisterUser_WithNullDto_ReturnsFailure()
    {
        // Act
        var result = await _authService.RegisterUser(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Please provide all required details.");
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsToken()
    {
        var passwordHasher = new PasswordHasher<string>();
        await _context.AccountUsers.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "login@example.com",
            Password = passwordHasher.HashPassword(string.Empty, "Password123")
        });
        await _context.SaveChangesAsync();

        var loginDto = new UserLoginDto
        {
            Email = "login@example.com",
            Password = "Password123"
        };
        var result = await _authService.LoginUser(loginDto);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrWhiteSpace();
        result.Message.Should().Be("Login successful.");
    }

    [Fact]
    public async Task LoginUser_WithWrongPassword_ReturnsFailure()
    {
        var passwordHasher = new PasswordHasher<string>();
        await _context.AccountUsers.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "login@example.com",
            Password = passwordHasher.HashPassword(string.Empty, "Password123")
        });
        await _context.SaveChangesAsync();

        var loginDto = new UserLoginDto
        {
            Email = "login@example.com",
            Password = "WrongPassword123"
        };

        var result = await _authService.LoginUser(loginDto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Invalid password.");
    }
}
