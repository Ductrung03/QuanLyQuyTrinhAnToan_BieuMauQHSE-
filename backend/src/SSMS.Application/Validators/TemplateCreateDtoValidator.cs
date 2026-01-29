using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for TemplateCreateDto
/// </summary>
public class TemplateCreateDtoValidator : AbstractValidator<TemplateCreateDto>
{
    private static readonly string[] ValidTemplateTypes = { "Form", "Checklist", "Report", "Other" };

    public TemplateCreateDtoValidator()
    {
        RuleFor(x => x.ProcedureId)
            .GreaterThan(0).WithMessage("Phải chọn quy trình");

        RuleFor(x => x.TemplateNo)
            .MaximumLength(100).WithMessage("Số hiệu biểu mẫu không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.TemplateNo));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên biểu mẫu không được để trống")
            .MaximumLength(500).WithMessage("Tên biểu mẫu không được vượt quá 500 ký tự");

        RuleFor(x => x.TemplateType)
            .NotEmpty().WithMessage("Loại biểu mẫu không được để trống")
            .Must(type => ValidTemplateTypes.Contains(type))
            .WithMessage($"Loại biểu mẫu phải là một trong: {string.Join(", ", ValidTemplateTypes)}");

        RuleFor(x => x.TemplateKey)
            .MaximumLength(50).WithMessage("Template key không được vượt quá 50 ký tự")
            .When(x => !string.IsNullOrEmpty(x.TemplateKey));
    }
}
