using CastleLordTycoon.Server.Domain.Enemies;
using CastleLordTycoon.Server.Services.Metadata;
using Microsoft.Extensions.Logging;

namespace CastleLordTycoon.Server.Services.Encounters;

public sealed class EncounterConfigService : IEncounterConfigService
{
    private readonly IGameMetadataProvider _metadataProvider;
    private readonly ILogger<EncounterConfigService> _logger;
    private readonly Lazy<IReadOnlyList<EncounterConfig>> _cache;

    public EncounterConfigService(IGameMetadataProvider metadataProvider, ILogger<EncounterConfigService> logger)
    {
        _metadataProvider = metadataProvider;
        _logger = logger;
        _cache = new Lazy<IReadOnlyList<EncounterConfig>>(BuildCache, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public IReadOnlyList<EncounterConfig> GetAll() => _cache.Value;

    private IReadOnlyList<EncounterConfig> BuildCache()
    {
        var snapshot = _metadataProvider.GetSnapshot();
        var list = new List<EncounterConfig>(snapshot.EnemyEncounters.Count);

        foreach (var encounter in snapshot.EnemyEncounters)
        {
            if (!snapshot.EnemyTemplates.TryGetValue(encounter.EnemyId, out var template))
            {
                _logger.LogWarning("Encounter {FieldId}/{EnemyId}는 유효한 적 템플릿을 찾을 수 없습니다.", encounter.FieldId, encounter.EnemyId);
                continue;
            }

            if (!snapshot.EnemyStatModifiers.TryGetValue(encounter.ModifierId, out var modifier))
            {
                _logger.LogWarning("Encounter {FieldId}/{ModifierId}는 유효한 스탯 배율을 찾을 수 없습니다.", encounter.FieldId, encounter.ModifierId);
                continue;
            }

            snapshot.EnemyDropTables.TryGetValue(encounter.DropTableId, out var dropEntries);
            dropEntries ??= Array.Empty<EnemyDropTableEntry>();

            list.Add(new EncounterConfig(
                encounter.FieldId,
                encounter.FieldNameKo,
                encounter.FieldTier,
                encounter.EncounterType,
                template,
                modifier,
                dropEntries,
                encounter));
        }

        _logger.LogInformation("Encounter configuration cache built: {Count} entries", list.Count);
        return list;
    }
}
