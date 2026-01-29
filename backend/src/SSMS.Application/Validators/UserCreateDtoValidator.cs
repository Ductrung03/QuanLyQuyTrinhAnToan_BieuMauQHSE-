using FluentValidation;
using SSMS.Application.DTOs;

namespace SSMS.Application.Validators;

/// <summary>
/// Validator for UserCreateDto
/// </summary>
public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    private static readonly string[] ValidRoles = { "Admin", "Manager", "User", "Viewer" };

    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự")
            .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự")
            .Matches(@"^[a-zA-Z0-9_\.]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ, số, dấu gạch dưới và dấu chấm");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
            .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự");

        RuleFor(x => x.FullName)
            .MaximumLength(200).WithMessage("Họ tên không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email không đúng định dạng")
            .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^[0-9\+\-\(\)\s]+$").WithMessage("Số điện thoại không đúng định dạng")
            .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Position)
            .MaximumLength(100).WithMessage("Chức vụ không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Position));

        RuleFor(x => x.Role)
            .Must(role => ValidRoles.Contains(role))
            .WithMessage($"Vai trò phải là một trong: {string.Join(", ", ValidRoles)}")
            .When(x => !string.IsNullOrEmpty(x.Role));

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit ID phải lớn hơn 0")
            .When(x => x.UnitId.HasValue);
    }
}
