using MagicVilla_VillaApi.Models;

namespace MagicVilla_VillaApi.Repository.IRepository
{
    public interface IVillaRepository
    {
        Task Create(Villa entity);
        Task Remove(Villa entity);
        Task Save();

    }
}
