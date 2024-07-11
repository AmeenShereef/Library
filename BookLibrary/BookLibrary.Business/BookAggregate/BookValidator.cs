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

    public class BookValidator : AbstractValidator<BookAdd>
    {
        public BookValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.Author).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.Genre).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.ISBN).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.PublicationYear).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
    public class BookCopyValidator : AbstractValidator<BookCopyAdd>
    {
        public BookCopyValidator()
        {
            RuleFor(x => x.BookId).NotEmpty().WithMessage("The {PropertyName} is required.");
            RuleFor(x => x.CopyNumber).NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}
