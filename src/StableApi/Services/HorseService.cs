namespace StableApi.Services;

using StableApi.Models;

public class HorseService : IHorseService
{
    private readonly Dictionary<int, Horse> _horses = new();
    private int _nextId = 1;

    public HorseService()
    {
        Create(new CreateHorseRequest("Moonbeam", "moonbeam@enchantedstables.com", "Thoroughbred"));
        Create(new CreateHorseRequest("Thunderhoof", "thunderhoof@enchantedstables.com", "Arabian"));
        Create(new CreateHorseRequest("Clover", "clover@enchantedstables.com", "Clydesdale"));
        Create(new CreateHorseRequest("Pippin", "pippin@enchantedstables.com", "Shetland"));
        Create(new CreateHorseRequest("Solstice", "solstice@enchantedstables.com", "Andalusian"));
    }

    public IEnumerable<Horse> GetAll() => _horses.Values;

    public Horse? GetById(int id) => _horses.GetValueOrDefault(id);

    public Horse Create(CreateHorseRequest request)
    {
        var horse = new Horse
        {
            Id = _nextId++,
            Name = request.Name,
            OwnerEmail = request.OwnerEmail,
            Breed = request.Breed,
            RegisteredAt = DateTime.UtcNow,
            IsActive = true,
        };
        _horses[horse.Id] = horse;
        return horse;
    }

    public Horse? Update(int id, UpdateHorseRequest request)
    {
        if (!_horses.TryGetValue(id, out var horse))
        {
            return null;
        }

        horse.Name = request.Name;
        horse.OwnerEmail = request.OwnerEmail;
        horse.Breed = request.Breed;
        return horse;
    }

    public bool Delete(int id)
    {
        if (!_horses.TryGetValue(id, out var horse))
        {
            return false;
        }

        horse.IsActive = false;
        return true;
    }

    /// <summary>Returns horses for the specified 1-based page number.</summary>
    public IEnumerable<Horse> GetPaged(int page, int pageSize)
    {
        return _horses.Values
            .OrderBy(h => h.Id)
            .Skip(page * pageSize)
            .Take(pageSize);
    }
}
