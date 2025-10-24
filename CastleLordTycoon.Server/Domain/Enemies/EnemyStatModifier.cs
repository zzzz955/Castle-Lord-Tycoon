using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Enemies;

public sealed class EnemyStatModifier
{
    [Name("modifier_id")]
    public string ModifierId { get; init; } = string.Empty;

    [Name("modifier_name_ko")]
    public string ModifierNameKo { get; init; } = string.Empty;

    [Name("attack_scale_pct")]
    public int AttackScalePct { get; init; }

    [Name("defense_scale_pct")]
    public int DefenseScalePct { get; init; }

    [Name("hp_scale_pct")]
    public int HpScalePct { get; init; }

    [Name("crit_rate_bp")]
    public int CritRateBasisPoints { get; init; }

    [Name("crit_damage_pct")]
    public int CritDamagePct { get; init; }

    [Name("armor_penetration_pct")]
    public int ArmorPenetrationPct { get; init; }

    [Name("damage_reduction_pct")]
    public int DamageReductionPct { get; init; }

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
