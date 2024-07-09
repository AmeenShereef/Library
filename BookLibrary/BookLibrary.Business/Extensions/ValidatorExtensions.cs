using FluentValidation.Results;
using BookLibrary.Infrastructure.Exceptions;
using System.Reflection;

namespace BookLibrary.Business.Extensions
{
    public static class ValidatorExtensions
    {
        public class GenericValidation<TRequest, TValidator>
        {
            public bool Validate(TRequest request)
            {
                if (request != null)
                {
                    object[] args = new object[] { request };
                    Type type = typeof(TValidator);
                    MethodInfo? method = type.GetMethods().FirstOrDefault(x => x.Name.Equals("Validate", StringComparison.OrdinalIgnoreCase));
                    var instance = Activator.CreateInstance(type);
                    ValidationResult? result = method != null ? method.Invoke(instance, args) as ValidationResult : null;

                    if (result != null && result.IsValid == false)
                    {
                        var fields = result.Errors.Select(property => new
                        {
                            Name = property.PropertyName,
                            ValueProvided = property.AttemptedValue,
                            Error = property.ErrorMessage,
                        });

                        throw new ValidationException("Validation Error", fields);
                    }
                }

                return false;
            }
        }
    }
}
