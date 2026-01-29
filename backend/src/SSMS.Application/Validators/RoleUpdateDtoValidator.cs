using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for RoleUpdateDto - Validates role update requests
/// </summary>
public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
{
    public RoleUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên vai trò không được để trống")
            .MaximumLength(100).WithMessage("Tên vai trò không được vượt quá 100 ký tự")
            .Matches(@"^[\p{L}\p{N}\s\-_]+$").WithMessage("Tên vai trò chỉ được chứa chữ, số, khoảng trắng, dấu gạch ngang và gạch dưới");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Mô tả không được vượt quá 500 ký tự");
    }
}
