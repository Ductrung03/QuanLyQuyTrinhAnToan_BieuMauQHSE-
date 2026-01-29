using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for TemplateUpdateDto
/// </summary>
public class TemplateUpdateDtoValidator : AbstractValidator<TemplateUpdateDto>
{
    private static readonly string[] ValidTemplateTypes = { "Form", "Checklist", "Report", "Other" };
    private static readonly string[] ValidStates = { "Draft", "Active", "Obsolete" };

    public TemplateUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên biểu mẫu không được để trống")
            .MaximumLength(500).WithMessage("Tên biểu mẫu không được vượt quá 500 ký tự");

        RuleFor(x => x.TemplateNo)
            .MaximumLength(100).WithMessage("Số hiệu biểu mẫu không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.TemplateNo));

        RuleFor(x => x.TemplateType)
            .NotEmpty().WithMessage("Loại biểu mẫu không được để trống")
            .Must(type => ValidTemplateTypes.Contains(type))
            .WithMessage($"Loại biểu mẫu phải là một trong: {string.Join(", ", ValidTemplateTypes)}");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Trạng thái không được để trống")
            .Must(state => ValidStates.Contains(state))
            .WithMessage($"Trạng thái phải là một trong: {string.Join(", ", ValidStates)}");
    }
}
