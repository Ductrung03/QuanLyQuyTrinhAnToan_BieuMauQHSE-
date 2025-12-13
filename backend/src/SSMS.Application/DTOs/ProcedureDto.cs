namespace SSMS.Application.DTOs;

/// <summary>
/// DTO cho Procedure (đầy đủ thông tin)
/// </summary>
public class ProcedureDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string State { get; set; } = "Draft";
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ReleasedDate { get; set; }
    
    // Owner info
    public int? OwnerUserId { get; set; }
    public string? OwnerUserName { get; set; }
    
    // Author info
    public int? AuthorUserId { get; set; }
    public string? AuthorUserName { get; set; }
    
    // Approver info
    public int? ApproverUserId { get; set; }
    public string? ApproverUserName { get; set; }
    
    // Collections
    public List<ProcedureDocumentDto> Documents { get; set; } = new();
    public List<TemplateDto> Templates { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO cho tạo mới Procedure
/// </summary>
public class ProcedureCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Description { get; set; }
    public int? OwnerUserId { get; set; }
    public int? AuthorUserId { get; set; }
}

/// <summary>
/// DTO cho cập nhật Procedure
/// </summary>
public class ProcedureUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string State { get; set; } = "Draft";
    public string? Description { get; set; }
    public int? OwnerUserId { get; set; }
    public int? ApproverUserId { get; set; }
    public DateTime? ReleasedDate { get; set; }
}

/// <summary>
/// DTO cho danh sách Procedure (rút gọn)
/// </summary>
public class ProcedureListDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string State { get; set; } = "Draft";
    public string? OwnerUserName { get; set; }
    public DateTime? ReleasedDate { get; set; }
    public int DocumentCount { get; set; }
    public int TemplateCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
