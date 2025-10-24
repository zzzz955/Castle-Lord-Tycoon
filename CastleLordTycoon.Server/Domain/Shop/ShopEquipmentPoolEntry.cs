using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Shop;

public sealed class ShopEquipmentPoolEntry
{
    [Name("settlement_tier")]
    public string SettlementTier { get; init; } = string.Empty;

    [Name("settlement_name_ko")]
    public string SettlementNameKo { get; init; } = string.Empty;

    [Name("equipment_slot_id")]
    public string EquipmentSlotId { get; init; } = string.Empty;

    [Name("equipment_slot_ko")]
    public string EquipmentSlotKo { get; init; } = string.Empty;

    [Name("grade_c_bps")]
    public int GradeCBasisPoints { get; init; }

    [Name("grade_uc_bps")]
    public int GradeUcBasisPoints { get; init; }

    [Name("grade_r_bps")]
    public int GradeRBasisPoints { get; init; }

    [Name("grade_h_bps")]
    public int GradeHBasisPoints { get; init; }

    [Name("level_min")]
    public int LevelMin { get; init; }

    [Name("level_max")]
    public int LevelMax { get; init; }

    [Name("price_multiplier_pct")]
    public int PriceMultiplierPct { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
