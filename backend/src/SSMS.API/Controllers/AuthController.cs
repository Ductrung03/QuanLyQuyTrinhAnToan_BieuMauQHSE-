using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Infrastructure.Identity;

namespace SSMS.API.Controllers;

/// <summary>
/// Authentication Controller - Mock Login
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MockAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(MockAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách users để hiển thị dropdown
    /// </summary>
    [HttpGet("users")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableUsers()
    {
        try
        {
            var users = await _authService.GetAvailableUsersAsync();
            return Ok(new
            {
                Success = true,
                Data = users
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available users");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách người dùng"
            });
        }
    }

    /// <summary>
    /// Mock Login - chọn user từ dropdown
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] MockLoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Người dùng không tồn tại hoặc đã bị vô hiệu hóa"
                });
            }

            _logger.LogInformation("User {Username} logged in successfully", result.Username);

            return Ok(new
            {
                Success = true,
                Data = result,
                Message = "Đăng nhập thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi đăng nhập"
            });
        }
    }

    /// <summary>
    /// Login với Email/Username và Password
    /// </summary>
    [HttpPost("login-credentials")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithCredentials([FromBody] LoginCredentialsRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.EmailOrUsername) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Vui lòng nhập email/tên đăng nhập và mật khẩu"
                });
            }

            var result = await _authService.LoginWithCredentialsAsync(request);

            if (result == null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email/tên đăng nhập hoặc mật khẩu không đúng"
                });
            }

            _logger.LogInformation("User {Username} logged in with credentials successfully", result.Username);

            return Ok(new
            {
                Success = true,
                Data = result,
                Message = "Đăng nhập thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login with credentials");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi đăng nhập"
            });
        }
    }

    /// <summary>
    /// Lấy thông tin user hiện tại
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var unitId = User.FindFirst("UnitId")?.Value;
            var unitName = User.FindFirst("UnitName")?.Value;
            var position = User.FindFirst("Position")?.Value;

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    FullName = fullName,
                    Role = role,
                    UnitId = unitId,
                    UnitName = unitName,
                    Position = position
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy thông tin người dùng"
            });
        }
    }
}
