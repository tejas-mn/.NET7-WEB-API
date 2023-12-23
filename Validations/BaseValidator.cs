using FluentValidation;
using FluentValidation.Results;

public class BaseValidator<T> : AbstractValidator<T>
{
    public ValidationResult ValidateAndThrow(T request)
    {
        ValidationResult validationResult = Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return validationResult;
    }
}
