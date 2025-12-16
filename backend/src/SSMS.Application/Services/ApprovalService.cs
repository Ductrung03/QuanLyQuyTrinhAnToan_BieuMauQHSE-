using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

public class ApprovalService : IApprovalService
{
    private readonly IUnitOfWork _unitOfWork;

    public ApprovalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SubmissionDto>> GetPendingApprovalsAsync(int userId)
    {
        // Find tasks where user is Approver AND status is Submitted
        // Using navigation property query
        // Find all submitted tasks
        var submissions = await _unitOfWork.Submissions.FindAsync(s => s.Status == "Submitted");

        var result = new List<SubmissionDto>();
        foreach (var sub in submissions)
        {
            // Map manually
            var procedure = await _unitOfWork.Procedures.GetByIdAsync(sub.ProcedureId);
            
            // Check if current user is the approver
            if (procedure == null || procedure.ApproverUserId != userId) continue;

            var submittedByUser = await _unitOfWork.Users.GetByIdAsync(sub.SubmittedByUserId);
            
            // We optimize by not loading files/recipients for the list view if performance is key, 
            // but SubmissionDto expects them? 
            // SubmissionDto has List<SubmissionFileDto> Files. 
            // Let's load them to be consistent or return empty if not needed.
            // For list, usually not needed. But let's check DTO usage.
            // I'll leave them empty for the list to save N+1 queries if list is long,
            // or I can do a basic mapping.
            
            result.Add(new SubmissionDto
            {
                Id = sub.Id,
                ProcedureId = sub.ProcedureId,
                ProcedureName = procedure.Name,
                ProcedureCode = procedure.Code,
                Title = sub.Title,
                Content = sub.Content, // Maybe summary?
                Status = sub.Status,
                SubmittedAt = sub.SubmittedAt,
                SubmittedByUserId = sub.SubmittedByUserId,
                SubmittedByUserName = submittedByUser?.FullName ?? "",
                RecalledAt = sub.RecalledAt,
                RecallReason = sub.RecallReason,
                Files = new List<SubmissionFileDto>(), // Skip for list
                Recipients = new List<SubmissionRecipientDto>(), // Skip for list
                ApproverUserId = userId,
                ApproverUserName = "", // Current user
                CanApprove = true
            });
        }

        return result.OrderByDescending(s => s.SubmittedAt);
    }

    public async Task ApproveAsync(int submissionId, int userId, string? note)
    {
        await ProcessApprovalAsync(submissionId, userId, "Approved", note);
    }

    public async Task RejectAsync(int submissionId, int userId, string? note)
    {
        await ProcessApprovalAsync(submissionId, userId, "Rejected", note);
    }

    private async Task ProcessApprovalAsync(int submissionId, int userId, string action, string? note)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(submissionId);
        if (submission == null)
            throw new KeyNotFoundException("Không tìm thấy biểu mẫu");

        if (submission.Status != "Submitted")
            throw new InvalidOperationException($"Không thể {action} biểu mẫu đang ở trạng thái {submission.Status}");

        // Verify Approver
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(submission.ProcedureId);
        if (procedure == null)
            throw new InvalidOperationException("Không tìm thấy quy trình");

        if (procedure.ApproverUserId != userId)
            throw new UnauthorizedAccessException("Bạn không có quyền phê duyệt biểu mẫu này");

        // Update Submission
        submission.Status = action; // "Approved" or "Rejected"
        _unitOfWork.Submissions.Update(submission);

        // Create Approval Record
        var approval = new OpsApproval
        {
            SubmissionId = submissionId,
            ApproverUserId = userId,
            Action = action,
            Note = note,
            ActionDate = DateTime.UtcNow
        };
        await _unitOfWork.Approvals.AddAsync(approval);

        await _unitOfWork.SaveChangesAsync();
    }
}
