using FluentValidation;
using Grand.Domain.Customers;
using Grand.Core.Validators;
using Grand.Services.Localization;
using Grand.Web.Models.Customer;
using System.Collections.Generic;

namespace Grand.Web.Validators.Customer
{
    public class ChangePasswordValidator : BaseGrandValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator(
            IEnumerable<IValidatorConsumer<ChangePasswordModel>> validators,
            ILocalizationService localizationService, CustomerSettings customerSettings)
            : base(validators)
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.OldPassword.Required"));
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.NewPassword.Required"));
            RuleFor(x => x.NewPassword).Length(customerSettings.PasswordMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Account.ChangePassword.Fields.NewPassword.LengthValidation"), customerSettings.PasswordMinLength));
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.ConfirmNewPassword.Required"));
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.NewPassword.EnteredPasswordsDoNotMatch"));
        }}
}