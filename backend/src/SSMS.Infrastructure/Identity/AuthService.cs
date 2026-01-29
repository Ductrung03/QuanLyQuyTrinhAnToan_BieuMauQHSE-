using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSMS.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SSMS.Infrastructure.Identity;

/// <summary>
/// Authentication Service - Proper implementation with BCrypt password hashing
/// </summary>
public class AuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    /// <summary>
    /// Login với Email/Username và Password
    /// </summary>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LoginName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        // Tìm user theo email hoặc username
        var users = await _unitOfWork.Users.FindAsync(u => 
            u.IsActive && 
            (u.Email == request.LoginName || u.Username == request.LoginName));
        
        var user = users.FirstOrDefault();

        if (user == null)
        {
            return null;
        }

        // Kiểm tra password với BCrypt
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return null; // User chưa có password
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null; // Sai mật khẩu
        }

        // Load Unit information
        if (!user.UnitId.HasValue)
        {
            return null;
        }
        var unit = await _unitOfWork.Units.GetByIdAsync(user.UnitId.Value);
        if (unit == null)
        {
            return null;
        }

        // Generate JWT token
        var token = GenerateJwtToken(user, unit);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            Role = user.Role ?? "User",
            UnitId = user.UnitId!.Value,
            UnitName = unit.Name ?? "",
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "480")
            )
        };
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
        {
            return false;
        }

        // Verify current password
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }
        }

        // Hash new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Reset mật khẩu (Admin only)
    /// </summary>
    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Hash mật khẩu với BCrypt (static helper)
    /// </summary>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verify mật khẩu với BCrypt (static helper)
    /// </summary>
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    /// <summary>
    /// Lấy danh sách users (cho admin)
    /// </summary>
    public async Task<IEnumerable<UserInfo>> GetUsersAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.IsActive);
            var units = await _unitOfWork.Units.GetAllAsync();
            var unitDict = units.ToDictionary(u => u.Id);
            
            var userList = new List<UserInfo>();

            foreach (var user in users)
            {
                if (!user.UnitId.HasValue) continue;

                if (unitDict.TryGetValue(user.UnitId.Value, out var unit))
                {
                    userList.Add(new UserInfo
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email ?? "",
                        FullName = user.FullName ?? "",
                        Role = user.Role ?? "User",
                        Position = user.Position,
                        UnitId = user.UnitId!.Value,
                        UnitCode = unit.Code,
                        UnitName = unit.Name
                    });
                }
            }

            return userList.OrderBy(u => u.Role).ThenBy(u => u.FullName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR IN GetUsersAsync: " + ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Generate JWT token
    /// </summary>
    private string GenerateJwtToken(Core.Entities.AppUser user, Core.Entities.Unit unit)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "SSMS-Default-Secret-Key-Must-Be-At-Least-32-Characters-Long";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.GivenName, user.FullName ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "User"),
            new Claim("UnitId", user.UnitId?.ToString() ?? ""),
            new Claim("UnitCode", unit.Code ?? ""),
            new Claim("UnitName", unit.Name ?? ""),
            new Claim("Position", user.Position ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "SSMS-API",
            audience: jwtSettings["Audience"] ?? "SSMS-Client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(jwtSettings["ExpirationMinutes"] ?? "480")
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
