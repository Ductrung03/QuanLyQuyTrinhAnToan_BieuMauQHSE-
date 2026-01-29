using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SSMS.API.Helpers;
using SSMS.Application.Services;
using SSMS.Infrastructure.Identity;

namespace SSMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, IAuditLogService auditLogService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    [HttpGet("users")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _authService.GetUsersAsync();
            return Ok(new
            {
                Success = true,
                Data = users
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách người dùng"
            });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                });
            }

            _logger.LogInformation("User {Username} logged in successfully", result.Username);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Login",
                targetType: "Auth",
                targetId: result.UserId,
                targetName: result.Username,
                userIdOverride: result.UserId,
                userNameOverride: result.Username);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Token = result.Token,
                    User = new
                    {
                        Id = result.UserId,
                        UserName = result.Username,
                        FullName = result.FullName,
                        Email = result.Email,
                        Role = result.Role,
                        UnitId = result.UnitId,
                        UnitName = result.UnitName
                    }
                },
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

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Không xác định được người dùng" });
            }

            var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Mật khẩu hiện tại không đúng"
                });
            }

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "ChangePassword",
                targetType: "User",
                targetId: userId);

            return Ok(new
            {
                Success = true,
                Message = "Đổi mật khẩu thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi đổi mật khẩu"
            });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            int? userId = null;
            if (int.TryParse(userIdClaim, out var parsedId))
            {
                userId = parsedId;
            }

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Logout",
                targetType: "Auth",
                targetId: userId,
                targetName: username,
                userIdOverride: userId,
                userNameOverride: username);

            return Ok(new
            {
                Success = true,
                Message = "Đăng xuất thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi đăng xuất"
            });
        }
    }

    [HttpPost("reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var success = await _authService.ResetPasswordAsync(request.UserId, request.NewPassword);

            if (!success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng"
                });
            }

            _logger.LogInformation("Password reset for user {UserId}", request.UserId);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "ResetPassword",
                targetType: "User",
                targetId: request.UserId);

            return Ok(new
            {
                Success = true,
                Message = "Đặt lại mật khẩu thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi đặt lại mật khẩu"
            });
        }
    }

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

    [HttpPost("seed-passwords")]
    [AllowAnonymous]
    public async Task<IActionResult> SeedPasswords([FromBody] SeedPasswordsRequest request)
    {
        if (request.SecretKey != "SSMS-SEED-2024")
        {
            return BadRequest(new { Success = false, Message = "Invalid secret key" });
        }

        try
        {
            var users = await _authService.GetUsersAsync();
            var count = 0;

            foreach (var user in users)
            {
                var success = await _authService.ResetPasswordAsync(user.Id, request.DefaultPassword);
                if (success) count++;
            }

            _logger.LogInformation("Seeded passwords for {Count} users", count);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "SeedPasswords",
                targetType: "User",
                detail: $"UsersUpdated={count}");

            return Ok(new
            {
                Success = true,
                Message = $"Đã đặt mật khẩu cho {count} người dùng",
                Data = new { UsersUpdated = count }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding passwords");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi đặt mật khẩu"
            });
        }
    }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

public class SeedPasswordsRequest
{
    public string SecretKey { get; set; } = string.Empty;
    public string DefaultPassword { get; set; } = "Admin@123";
}
