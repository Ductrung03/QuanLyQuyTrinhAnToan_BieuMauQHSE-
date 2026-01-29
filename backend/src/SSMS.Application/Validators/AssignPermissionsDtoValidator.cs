using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for AssignPermissionsDto - Validates permission assignment requests
/// </summary>
public class AssignPermissionsDtoValidator : AbstractValidator<AssignPermissionsDto>
{
    public AssignPermissionsDtoValidator()
    {
        RuleFor(x => x.PermissionIds)
            .NotNull().WithMessage("Danh sách quyền không được null")
            .NotEmpty().WithMessage("Danh sách quyền không được rỗng")
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("Tất cả ID quyền phải lớn hơn 0")
            .Must(ids => ids.Distinct().Count() == ids.Count())
            .WithMessage("Danh sách quyền không được chứa ID trùng lặp");
    }
}
