namespace SSMS.Application.DTOs;

/// <summary>
/// DTO cho Template
/// </summary>
public class TemplateDto
{
    public int Id { get; set; }
    public int ProcedureId { get; set; }
    public string? TemplateKey { get; set; }
    public string? TemplateNo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TemplateType { get; set; } = "Form";
    public string State { get; set; } = "Draft";
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO cho tạo mới Template
/// </summary>
public class TemplateCreateDto
{
    public int ProcedureId { get; set; }
    public string? TemplateKey { get; set; }
    public string? TemplateNo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TemplateType { get; set; } = "Form";
}

/// <summary>
/// DTO cho cập nhật Template
/// </summary>
public class TemplateUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? TemplateNo { get; set; }
    public string TemplateType { get; set; } = "Form";
    public string State { get; set; } = "Draft";
    public bool IsActive { get; set; }
}
