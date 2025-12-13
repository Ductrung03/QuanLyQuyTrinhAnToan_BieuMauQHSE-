namespace SSMS.Core.Entities;

/// <summary>
/// Biểu mẫu/Checklist gắn với quy trình
/// </summary>
public class OpsTemplate : BaseEntity
{
    /// <summary>
    /// Template Key (ID trong JSON: 'T1', 'T6ihr'...)
    /// </summary>
    public string? TemplateKey { get; set; }

    /// <summary>
    /// ID quy trình
    /// </summary>
    public int ProcedureId { get; set; }

    /// <summary>
    /// Quy trình
    /// </summary>
    public OpsProcedure Procedure { get; set; } = null!;

    /// <summary>
    /// Số hiệu biểu mẫu (VD: "FM-OPS-01", "SOF 05-04-01")
    /// </summary>
    public string? TemplateNo { get; set; }

    /// <summary>
    /// Tên biểu mẫu
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Loại biểu mẫu (Form, Checklist)
    /// </summary>
    public string TemplateType { get; set; } = "Form";

    /// <summary>
    /// Trạng thái (Draft, Submitted, Approved, Rejected)
    /// </summary>
    public string State { get; set; } = "Draft";

    /// <summary>
    /// Tên file mẫu (DOCX/XLSX/PDF)
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Đường dẫn file mẫu
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Kích thước file (bytes)
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Content type (MIME type)
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;
}
