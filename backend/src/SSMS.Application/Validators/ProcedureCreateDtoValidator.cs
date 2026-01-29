using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for ProcedureCreateDto
/// </summary>
public class ProcedureCreateDtoValidator : AbstractValidator<ProcedureCreateDto>
{
    public ProcedureCreateDtoValidator()
    {
        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Mã quy trình không được vượt quá 50 ký tự")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Mã quy trình chỉ được chứa chữ hoa, số và dấu gạch ngang")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên quy trình không được để trống")
            .MaximumLength(500).WithMessage("Tên quy trình không được vượt quá 500 ký tự");

        RuleFor(x => x.Version)
            .MaximumLength(20).WithMessage("Phiên bản không được vượt quá 20 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Version));

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Mô tả không được vượt quá 5000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.OwnerUserId)
            .GreaterThan(0).WithMessage("Owner User ID phải lớn hơn 0")
            .When(x => x.OwnerUserId.HasValue);

        RuleFor(x => x.AuthorUserId)
            .GreaterThan(0).WithMessage("Author User ID phải lớn hơn 0")
            .When(x => x.AuthorUserId.HasValue);
    }
}
