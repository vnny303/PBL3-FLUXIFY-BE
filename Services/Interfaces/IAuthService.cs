using FluxifyAPI.DTOs;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Services;

namespace FluxifyAPI.IServices
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> RegisterMerchantAsync(RegisterMerchantRequest request);
        Task<ServiceResult<object>> LoginMerchantAsync(LoginRequest request);
        Task<ServiceResult<object>> RegisterCustomerAsync(string subdomain, RegisterCustomerRequest request);
        Task<ServiceResult<object>> LoginCustomerAsync(string subdomain, LoginRequest request);
        Task<ServiceResult<object>> UpdateCustomerAsync(string subdomain, Guid customerId, UpdateCustomerRequestDto request);
    }
}
