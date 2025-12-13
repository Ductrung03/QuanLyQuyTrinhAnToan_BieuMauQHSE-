using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

public interface IApprovalService
{
    /// <summary>
    /// Lấy danh sách task cần phê duyệt của User
    /// </summary>
    Task<IEnumerable<SubmissionDto>> GetPendingApprovalsAsync(int userId);

    /// <summary>
    /// Phê duyệt biểu mẫu
    /// </summary>
    Task ApproveAsync(int submissionId, int userId, string? note);

    /// <summary>
    /// Từ chối biểu mẫu
    /// </summary>
    Task RejectAsync(int submissionId, int userId, string? note);
}
