using DiscountCodeSystem.Core.Entity;
namespace DiscountCodeSystem.Core.Utils
{
    public static class Validator
    {
        public static ValidationError? ValidateGenerateRequest(GenerateRequest request)
        {
            if (request.Length < 7 || request.Length > 8)
                return new ValidationError(nameof(request.Length), "Length must be between 7 and 8 characters.");

            if (request.Count > 2000)
                return new ValidationError(nameof(request.Count), "Cannot generate more than 2000 codes in a single request.");

            return null;
        }
    }
}