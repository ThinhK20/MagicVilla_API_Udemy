using MagicVilla_Web.Models;

namespace MagicVilla_Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse responsModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
