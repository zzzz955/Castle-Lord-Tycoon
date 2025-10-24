namespace CastleLordTycoon.Server.Services.Metadata;

public interface IGameMetadataProvider
{
    GameMetadataSnapshot GetSnapshot();
}
