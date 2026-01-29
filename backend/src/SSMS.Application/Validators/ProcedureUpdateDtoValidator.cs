using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for ProcedureUpdateDto
/// </summary>
public class ProcedureUpdateDtoValidator : AbstractValidator<ProcedureUpdateDto>
{
    private static readonly string[] ValidStates = { "Draft", "Submitted", "Approved", "Rejected", "Archived" };

    public ProcedureUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên quy trình không được để trống")
            .MaximumLength(500).WithMessage("Tên quy trình không được vượt quá 500 ký tự");

        RuleFor(x => x.Version)
            .MaximumLength(20).WithMessage("Phiên bản không được vượt quá 20 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Version));

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Trạng thái không được để trống")
            .Must(state => ValidStates.Contains(state))
            .WithMessage($"Trạng thái phải là một trong: {string.Join(", ", ValidStates)}");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Mô tả không được vượt quá 5000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.OwnerUserId)
            .GreaterThan(0).WithMessage("Owner User ID phải lớn hơn 0")
            .When(x => x.OwnerUserId.HasValue);

        RuleFor(x => x.ApproverUserId)
            .GreaterThan(0).WithMessage("Approver User ID phải lớn hơn 0")
            .When(x => x.ApproverUserId.HasValue);

        RuleFor(x => x.ReleasedDate)
            .LessThanOrEqualTo(DateTime.Now.AddYears(1))
            .WithMessage("Ngày phát hành không được vượt quá 1 năm từ hiện tại")
            .When(x => x.ReleasedDate.HasValue);
    }
}
