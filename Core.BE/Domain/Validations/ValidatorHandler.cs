using Emeint.Core.BE.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace Emeint.Core.BE.Domain.Validations
{
    public class ValidatorHandler
    {
        public static void Validate<M>(M model, AbstractValidator<M> validator) where M : class
        {
            ValidationResult validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                ThrowValidationException(validationResult);
        }
        private static void ThrowValidationException(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                throw new InvalidParameterValidationException(error.ErrorMessage);
            }
        }
    }
}
