namespace SSMS.Application.DTOs;

/// <summary>
/// DTO cho Document đính kèm
/// </summary>
public class ProcedureDocumentDto
{
    public int Id { get; set; }
    public int ProcedureId { get; set; }
    public string? DocVersion { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// DTO cho upload document
/// </summary>
public class DocumentUploadDto
{
    public string? DocVersion { get; set; }
}
