using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSMS.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SSMS.Infrastructure.Identity;

/// <summary>
/// Mock Authentication Service
/// Sử dụng cho giai đoạn phát triển, không yêu cầu password
/// </summary>
public class MockAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public MockAuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    /// <summary>
    /// Mock login - chỉ cần UserId
    /// </summary>
    public async Task<LoginResponse?> LoginAsync(MockLoginRequest request)
    {
        // Tìm user theo ID
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => 
            u.Id == request.UserId && u.IsActive);

        if (user == null)
        {
            return null;
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
            Role = (user.Id == 2) ? "Manager" : (user.Role ?? "User"),
            UnitId = user.UnitId!.Value,
            UnitName = unit.Name ?? "",
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "480")
            )
        };
    }

    /// <summary>
    /// Lấy danh sách users để hiển thị dropdown login
    /// </summary>
    public async Task<IEnumerable<UserInfo>> GetAvailableUsersAsync()
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
                        Role = (user.Id == 2) ? "Manager" : (user.Role ?? "User"),
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
            Console.WriteLine("CRITICAL ERROR IN GetAvailableUsersAsync: " + ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Generate JWT token
    /// </summary>
    private string GenerateJwtToken(Core.Entities.AppUser user, Core.Entities.Unit unit)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "SSMS-Default-Secret-Key";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.GivenName, user.FullName ?? ""),
            new Claim(ClaimTypes.Role, (user.Id == 2) ? "Manager" : (user.Role ?? "User")),
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
