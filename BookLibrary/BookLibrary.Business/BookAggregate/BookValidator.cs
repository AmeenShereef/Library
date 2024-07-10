using BookLibrary.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Business.BookAggregate
{
    public class BorrowBookValidator : AbstractValidator<BorrowBookReq>
    {
        public BorrowBookValidator()
        {
            RuleFor(x => x.BookCopyId).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}
