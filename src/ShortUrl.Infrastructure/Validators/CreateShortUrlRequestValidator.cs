using FluentValidation;
using ShortUrl.Common;
using ShortUrl.Core.Models;
using ShortUrl.Infrastructure.ApiKeyManager;

namespace ShortUrl.Infrastructure.Validators
{
    internal class CreateShortUrlRequestValidator : AbstractValidator<CreateShortUrlRequest>
    {
        public CreateShortUrlRequestValidator(IApiKeyContextAccessor apiKeyContext)
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("The original URL must not be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("The provided long URL is not valid.");

            RuleFor(x => x.Expire)
                .Must(expire => !expire.HasValue || expire.Value > DateTime.UtcNow)
                .WithMessage("The expiration date must be in the future.");

            When(x => x.ShortCode != null, () =>
            {
                RuleFor(x => x.ShortCode)
                    .Must(_ => apiKeyContext.Current.CanSetCustomShortCodes)
                    .WithMessage("Custom short URL codes are not allowed for your API key.")
                    .Must(shortCode => ShortUrlGenerator.IsValidShortCode(shortCode))
                    .WithMessage("The provided short URL code is not valid.");
            });
        }
    }
}
