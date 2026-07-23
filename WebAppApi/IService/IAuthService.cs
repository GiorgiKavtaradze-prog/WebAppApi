using WebAppApi.Common;
using WebAppApi.Dto;

namespace WebAppApi.IService;

public interface IAuthService
{    
    Task<Result<TokenDto>> LoginUser(UserLoginDto dto);
    Task<Result> RegisterUser(UserRegisterDto dto);
}
