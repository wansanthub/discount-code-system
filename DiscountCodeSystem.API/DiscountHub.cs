using DiscountCodeSystem.Core.Entity;
using DiscountCodeSystem.Infrastructure.Service;
using Microsoft.AspNetCore.SignalR;

namespace DiscountCodeSystem.API
{
    public class DiscountHub : Hub
    {
        private readonly DiscountCodeService _service;

        public DiscountHub(DiscountCodeService service)
        {
            _service = service;
        }

        public async Task<GenerateResponse> GenerateCodes(GenerateRequest request)
        {
        var (response, error) = await _service.GenerateCodesAsync(request);

        if (error != null)
        {
            throw new HubException(error.Message);
        }

        return response;
        }

        public async Task<UseCodeResponse> UseCodeAsync(UseCodeRequest request)
        {
            return await _service.UseCodeAsync(request.Code);
        }
    }
}