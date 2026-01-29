using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for RecallDto
/// </summary>
public class RecallDtoValidator : AbstractValidator<RecallDto>
{
    public RecallDtoValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do thu hồi không được để trống")
            .MinimumLength(10).WithMessage("Lý do thu hồi phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Lý do thu hồi không được vượt quá 1000 ký tự");
    }
}
