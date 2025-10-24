using CastleLordTycoon.Server.Domain.Encounters;
using CastleLordTycoon.Server.Domain.Enemies;
using CastleLordTycoon.Server.Domain.Equipment;
using CastleLordTycoon.Server.Domain.Forge;
using CastleLordTycoon.Server.Domain.Gems;
using CastleLordTycoon.Server.Domain.Heroes;
using CastleLordTycoon.Server.Domain.Npc;
using CastleLordTycoon.Server.Domain.Progression;
using CastleLordTycoon.Server.Domain.Settlements;
using CastleLordTycoon.Server.Domain.Shop;
using CastleLordTycoon.Server.Domain.Tavern;
using CastleLordTycoon.Server.Domain.Territory;

namespace CastleLordTycoon.Server.Services.Metadata;

public sealed record GameMetadataSnapshot(
    IReadOnlyDictionary<string, EnemyTemplate> EnemyTemplates,
    IReadOnlyDictionary<string, EnemyStatModifier> EnemyStatModifiers,
    IReadOnlyDictionary<string, IReadOnlyList<EnemyDropTableEntry>> EnemyDropTables,
    IReadOnlyList<EnemyEncounter> EnemyEncounters,

    IReadOnlyList<EquipmentBaseStat> EquipmentBaseStats,
    IReadOnlyDictionary<string, EquipmentGradeRule> EquipmentGradeRules,
    IReadOnlyList<EquipmentSubstatRange> EquipmentSubstatRanges,
    IReadOnlyList<EquipmentSocketUnlock> EquipmentSocketUnlocks,
    IReadOnlyList<EquipmentUpgradeCost> EquipmentUpgradeCosts,

    IReadOnlyList<ForgeUpgradeRule> ForgeUpgradeRules,
    IReadOnlyList<ForgeSynthesisRule> ForgeSynthesisRules,
    IReadOnlyList<ForgeDecomposeRule> ForgeDecomposeRules,
    IReadOnlyList<ForgeOptionChangeRule> ForgeOptionChangeRules,
    IReadOnlyList<ForgeSocketUnlock> ForgeSocketUnlocks,

    IReadOnlyDictionary<string, GemTemplate> GemTemplates,
    IReadOnlyDictionary<string, IReadOnlyList<GemDropTableEntry>> GemDropTables,
    IReadOnlyList<GemCombinationRule> GemCombinationRules,

    IReadOnlyDictionary<string, HeroTemplate> HeroTemplates,
    IReadOnlyDictionary<string, HeroUniqueEffect> HeroUniqueEffects,
    IReadOnlyList<HeroGrowth> HeroGrowthTable,
    IReadOnlyList<HeroRebirthRule> HeroRebirthRules,
    IReadOnlyList<HeroReleaseReward> HeroReleaseRewards,
    IReadOnlyList<HeroPoolExpansionRule> HeroPoolExpansionRules,
    IReadOnlyDictionary<string, IReadOnlyList<HeroDropTableEntry>> HeroDropTables,

    IReadOnlyDictionary<string, TavernRecruitmentTier> TavernRecruitmentTiers,
    IReadOnlyDictionary<string, TavernCouponPrice> TavernCouponPrices,

    IReadOnlyDictionary<string, NpcService> NpcServices,
    IReadOnlyList<SettlementNpcMapEntry> SettlementNpcMap,
    IReadOnlyDictionary<string, SettlementTemplate> SettlementTemplates,

    IReadOnlyDictionary<string, RankRequirement> RankRequirements,
    IReadOnlyDictionary<string, FlagTemplate> FlagTemplates,
    IReadOnlyList<ForbiddenTile> ForbiddenTiles,

    IReadOnlyList<ShopEquipmentPoolEntry> ShopEquipmentPool,
    IReadOnlyList<ShopEquipmentPrice> ShopEquipmentPrices,
    IReadOnlyList<ShopSharedPoolEntry> ShopSharedPool,
    IReadOnlyDictionary<string, ShopSharedPrice> ShopSharedPrices,
    IReadOnlyList<ShopFlagInventoryEntry> ShopFlagInventory,
    IReadOnlyList<ShopRefreshScheduleEntry> ShopRefreshSchedule);
