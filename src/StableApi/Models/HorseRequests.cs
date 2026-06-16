namespace StableApi.Models;

public record CreateHorseRequest(string Name, string OwnerEmail, string Breed);

public record UpdateHorseRequest(string Name, string OwnerEmail, string Breed);
