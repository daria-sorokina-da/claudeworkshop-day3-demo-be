namespace StableApi.Services;

using StableApi.Models;

public interface IHorseService
{
    IEnumerable<Horse> GetAll();
    Horse? GetById(int id);
    Horse Create(CreateHorseRequest request);
    Horse? Update(int id, UpdateHorseRequest request);
    bool Delete(int id);

    /// <summary>Returns a page of horses. Page is 1-based.</summary>
    IEnumerable<Horse> GetPaged(int page, int pageSize);
}
