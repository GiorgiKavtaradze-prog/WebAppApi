using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebAppApi.Dto;
using WebAppApi.GenericResponse;
using WebAppApi.IService;

namespace WebAppApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    [ProducesResponseType(typeof(ResponseResult<TokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<TokenDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseResult<TokenDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var result = await _authService.LoginUser(userLoginDto);
        var message = result.Message ?? "Request could not be completed.";

        if (!result.IsSuccess)
        {
            if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(ResponseResult<TokenDto>.Failure(result.Data, message));
            }

            return BadRequest(ResponseResult<TokenDto>.Failure(result.Data, message));
        }

        return Ok(ResponseResult<TokenDto>.Success(result.Data, message));
    }

    [HttpPost("Register")]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        var result = await _authService.RegisterUser(userRegisterDto);
        var message = result.Message ?? "Request could not be completed.";

        if (!result.IsSuccess)
        {
            return BadRequest(ResponseResult<string>.Failure(null, message));
        }

        return Ok(ResponseResult<string>.Success(null, message));
    }
}
