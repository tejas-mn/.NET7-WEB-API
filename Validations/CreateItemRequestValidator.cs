using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Respository;
using FluentValidation;
using FluentValidation.Results;

public class CreateItemRequestValidator : BaseValidator<CreateItemRequestDto>
{
    private readonly IUnitOfWork _uow;

    public CreateItemRequestValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(item => item.Id).NotEqual(0);
        RuleFor(item => item.Id)
            .Must(ItemWithThisIdMustNotExist)
            .WithMessage("Item with the provided ID {PropertyValue} already exist.");;
        RuleFor(item => item.Description).NotEmpty().MinimumLength(5);
        RuleFor(item => item.Sku).NotEmpty().MinimumLength(2);
        RuleFor(item => item.Name).NotEmpty().MinimumLength(2);
        RuleFor(item => item.Price).GreaterThan(0);
        RuleFor(item => item.CategoryId)
            .Must(CategoryWithThisIdMustExist)
            .WithMessage("Category with the provided CategoryId {PropertyValue} does not exist.");
    }

    private bool ItemWithThisIdMustNotExist(int Id) => _uow.ItemsRepository.GetById(Id) == null;
    private bool CategoryWithThisIdMustExist(int categoryId) => _uow.CategoryRepository.GetById(categoryId) != null;

}