using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Encounters;

public sealed class EnemyEncounter
{
    [Name("field_id")]
    public string FieldId { get; init; } = string.Empty;

    [Name("field_name_ko")]
    public string FieldNameKo { get; init; } = string.Empty;

    [Name("field_tier")]
    public int FieldTier { get; init; }

    [Name("encounter_type")]
    public string EncounterType { get; init; } = string.Empty;

    [Name("enemy_id")]
    public string EnemyId { get; init; } = string.Empty;

    [Name("enemy_name_ko")]
    public string EnemyNameKo { get; init; } = string.Empty;

    [Name("level")]
    public int Level { get; init; }

    [Name("modifier_id")]
    public string ModifierId { get; init; } = string.Empty;

    [Name("spawn_rate_bps")]
    public int SpawnRateBasisPoints { get; init; }

    [Name("drop_table_id")]
    public string DropTableId { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
