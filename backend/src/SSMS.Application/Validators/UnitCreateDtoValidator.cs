using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for UnitCreateDto
/// </summary>
public class UnitCreateDtoValidator : AbstractValidator<UnitCreateDto>
{
    private static readonly string[] ValidUnitTypes = { "Ship", "Department", "Office", "Section", "Team" };

    public UnitCreateDtoValidator()
    {
        RuleFor(x => x.UnitCode)
            .MaximumLength(50).WithMessage("Mã đơn vị không được vượt quá 50 ký tự")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Mã đơn vị chỉ được chứa chữ hoa, số và dấu gạch ngang")
            .When(x => !string.IsNullOrWhiteSpace(x.UnitCode));

        RuleFor(x => x.UnitName)
            .NotEmpty().WithMessage("Tên đơn vị không được để trống")
            .MaximumLength(200).WithMessage("Tên đơn vị không được vượt quá 200 ký tự");

        RuleFor(x => x.UnitType)
            .NotEmpty().WithMessage("Loại đơn vị không được để trống")
            .Must(type => ValidUnitTypes.Contains(type))
            .WithMessage($"Loại đơn vị phải là một trong: {string.Join(", ", ValidUnitTypes)}");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ParentUnitId)
            .GreaterThan(0).WithMessage("Parent Unit ID phải lớn hơn 0")
            .When(x => x.ParentUnitId.HasValue);
    }
}
