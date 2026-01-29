using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for SubmissionCreateDto
/// </summary>
public class SubmissionCreateDtoValidator : AbstractValidator<SubmissionCreateDto>
{
    public SubmissionCreateDtoValidator()
    {
        RuleFor(x => x.ProcedureId)
            .GreaterThan(0).WithMessage("Phải chọn quy trình");

        RuleFor(x => x.TemplateId)
            .GreaterThan(0).WithMessage("Phải chọn biểu mẫu")
            .When(x => x.TemplateId.HasValue);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống")
            .MaximumLength(500).WithMessage("Tiêu đề không được vượt quá 500 ký tự");

        RuleFor(x => x.Content)
            .MaximumLength(10000).WithMessage("Nội dung không được vượt quá 10000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Content));

        RuleFor(x => x.RecipientUserIds)
            .NotEmpty().WithMessage("Phải chọn ít nhất một người nhận")
            .When(x => x.RecipientUserIds != null);

        RuleForEach(x => x.RecipientUserIds)
            .GreaterThan(0).WithMessage("Recipient User ID phải lớn hơn 0")
            .When(x => x.RecipientUserIds != null);
    }
}
