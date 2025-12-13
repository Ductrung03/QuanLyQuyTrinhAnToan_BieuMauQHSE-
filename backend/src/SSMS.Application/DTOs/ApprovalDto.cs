namespace SSMS.Application.DTOs;

public class ApprovalActionDto
{
    public string? Note { get; set; }
}

public class ApprovalLogDto
{
    public int Id { get; set; }
    public int ApproverUserId { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime ActionDate { get; set; }
}
