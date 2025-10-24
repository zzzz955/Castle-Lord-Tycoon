namespace CastleLordTycoon.Server.Services.Encounters;

public interface IEncounterConfigService
{
    IReadOnlyList<EncounterConfig> GetAll();
}
