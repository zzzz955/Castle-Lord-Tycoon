using CastleLordTycoon.Server.Domain.Enemies;
using CastleLordTycoon.Server.Domain.Encounters;

namespace CastleLordTycoon.Server.Services.Encounters;

public sealed record EncounterConfig(
    string FieldId,
    string FieldNameKo,
    int FieldTier,
    string EncounterType,
    EnemyTemplate Enemy,
    EnemyStatModifier StatModifier,
    IReadOnlyList<EnemyDropTableEntry> DropTableEntries,
    EnemyEncounter RawEncounter);
