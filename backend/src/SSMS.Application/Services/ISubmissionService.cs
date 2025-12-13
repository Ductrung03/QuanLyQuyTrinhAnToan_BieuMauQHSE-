using Microsoft.AspNetCore.Http;
using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Interface cho Submission Service
/// </summary>
public interface ISubmissionService
{
    /// <summary>
    /// Lấy danh sách biểu mẫu của người dùng hiện tại
    /// </summary>
    Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(int userId);

    /// <summary>
    /// Lấy chi tiết biểu mẫu theo ID
    /// </summary>
    Task<SubmissionDto?> GetByIdAsync(int id);

    /// <summary>
    /// Tạo mới biểu mẫu
    /// </summary>
    Task<SubmissionDto> CreateAsync(SubmissionCreateDto dto, int userId, List<IFormFile>? files);

    /// <summary>
    /// Thu hồi biểu mẫu
    /// </summary>
    Task<bool> RecallAsync(int id, int userId, string reason);

    /// <summary>
    /// Kiểm tra có thể thu hồi không
    /// </summary>
    Task<bool> CanRecallAsync(int id, int userId);
}
