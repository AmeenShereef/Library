using BookLibrary.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Business.AutenticateAggregate
{
    public class AuthenticationValidator : AbstractValidator<AuthenticateRequest>
    {
        public AuthenticationValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
    public class UserAddValidator : AbstractValidator<UserAdd>
    {
        public UserAddValidator()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.RegistrationCode).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}
