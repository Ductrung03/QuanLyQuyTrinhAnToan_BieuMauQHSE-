namespace SSMS.Core.Entities;

/// <summary>
/// Đơn vị tổ chức (Phòng ban, Chi nhánh, Tàu...)
/// </summary>
public class Unit : BaseEntity
{
    /// <summary>
    /// Mã đơn vị (VD: "UNIT001", "SHIP001")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Tên đơn vị
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Loại đơn vị (Department, Branch, Ship, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// ID đơn vị cha (nếu có cấu trúc phân cấp)
    /// </summary>
    public int? ParentUnitId { get; set; }

    /// <summary>
    /// Đơn vị cha
    /// </summary>
    public Unit? ParentUnit { get; set; }

    /// <summary>
    /// Các đơn vị con
    /// </summary>
    public ICollection<Unit> ChildUnits { get; set; } = new List<Unit>();

    /// <summary>
    /// Người dùng thuộc đơn vị này
    /// </summary>
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;
}
