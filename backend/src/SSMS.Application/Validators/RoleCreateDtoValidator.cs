using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for RoleCreateDto - Validates role creation requests
/// </summary>
public class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên vai trò không được để trống")
            .MaximumLength(100).WithMessage("Tên vai trò không được vượt quá 100 ký tự")
            .Matches(@"^[\p{L}\p{N}\s\-_]+$").WithMessage("Tên vai trò chỉ được chứa chữ, số, khoảng trắng, dấu gạch ngang và gạch dưới");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Mã vai trò không được vượt quá 50 ký tự")
            .Matches(@"^[A-Z][A-Z0-9_]*$").WithMessage("Mã vai trò phải viết hoa, bắt đầu bằng chữ cái, chỉ chứa chữ hoa, số và dấu gạch dưới (VD: ADMIN, SHIP_CAPTAIN)")
            .Must(NotBeSystemRoleCode).WithMessage("Mã vai trò '{PropertyValue}' là mã hệ thống, không được sử dụng")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Mô tả không được vượt quá 500 ký tự");
    }

    private bool NotBeSystemRoleCode(string code)
    {
        var systemRoleCodes = new[] { "ADMIN", "MANAGER", "USER", "CAPTAIN", "SAFETY_OFFICER" };
        return !systemRoleCodes.Contains(code.ToUpperInvariant());
    }
}
