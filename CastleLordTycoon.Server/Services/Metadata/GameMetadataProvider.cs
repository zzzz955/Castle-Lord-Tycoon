using System.Collections.ObjectModel;
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
using CastleLordTycoon.Server.Metadata;
using CastleLordTycoon.Server.Metadata.Exceptions;
using Microsoft.Extensions.Logging;

namespace CastleLordTycoon.Server.Services.Metadata;

public sealed class GameMetadataProvider : IGameMetadataProvider
{
    private readonly IMetadataFileResolver _resolver;
    private readonly ILogger<GameMetadataProvider> _logger;
    private readonly Lazy<GameMetadataSnapshot> _snapshot;

    public GameMetadataProvider(IMetadataFileResolver resolver, ILogger<GameMetadataProvider> logger)
    {
        _resolver = resolver;
        _logger = logger;
        _snapshot = new Lazy<GameMetadataSnapshot>(LoadSnapshot, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public GameMetadataSnapshot GetSnapshot() => _snapshot.Value;

    private GameMetadataSnapshot LoadSnapshot()
    {
        _logger.LogInformation("Loading game metadata from CSV files...");
        var errors = new List<string>();

        // Enemies
        var enemyTemplates = CsvMetadataReader.Load<EnemyTemplate>(_resolver.Resolve("enemy_templates.csv"));
        var enemyTemplateMap = ToDictionary(enemyTemplates, e => e.EnemyId, "enemy_templates.csv", "enemy_id", errors);

        var enemyStatModifiers = CsvMetadataReader.Load<EnemyStatModifier>(_resolver.Resolve("enemy_stat_modifiers.csv"));
        var enemyStatModifierMap = ToDictionary(enemyStatModifiers, m => m.ModifierId, "enemy_stat_modifiers.csv", "modifier_id", errors);

        var enemyDropEntries = CsvMetadataReader.Load<EnemyDropTableEntry>(_resolver.Resolve("enemy_drop_tables.csv"));
        var enemyDropTableMap = GroupByKey(enemyDropEntries, e => e.DropTableId, "enemy_drop_tables.csv", "drop_table_id", errors);

        var enemyEncounters = CsvMetadataReader.Load<EnemyEncounter>(_resolver.Resolve("enemy_encounters.csv"));
        foreach (var encounter in enemyEncounters)
        {
            if (!enemyTemplateMap.ContainsKey(encounter.EnemyId))
            {
                errors.Add($"enemy_encounters.csv: {encounter.FieldId}의 enemy_id {encounter.EnemyId}가 enemy_templates.csv에 존재하지 않습니다.");
            }

            if (!enemyStatModifierMap.ContainsKey(encounter.ModifierId))
            {
                errors.Add($"enemy_encounters.csv: {encounter.FieldId}의 modifier_id {encounter.ModifierId}가 enemy_stat_modifiers.csv에 존재하지 않습니다.");
            }

            if (!string.IsNullOrWhiteSpace(encounter.DropTableId) && !enemyDropTableMap.ContainsKey(encounter.DropTableId))
            {
                errors.Add($"enemy_encounters.csv: {encounter.FieldId}의 drop_table_id {encounter.DropTableId}가 enemy_drop_tables.csv에 존재하지 않습니다.");
            }

            if (encounter.SpawnRateBasisPoints <= 0)
            {
                errors.Add($"enemy_encounters.csv: {encounter.FieldId}/{encounter.EnemyId} spawn_rate_bps가 0 이하입니다.");
            }
        }

        foreach (var template in enemyTemplates)
        {
            foreach (var effectId in template.UniqueEffectIds)
            {
                if (string.IsNullOrWhiteSpace(effectId))
                {
                    continue;
                }

                // 허용: effectId가 hero_unique_effects에 정의되어 있다면 사용 가능
            }
        }

        // Equipment
        var equipmentBaseStats = CsvMetadataReader.Load<EquipmentBaseStat>(_resolver.Resolve("equipment_base_stats.csv"));
        var equipmentGradeRules = CsvMetadataReader.Load<EquipmentGradeRule>(_resolver.Resolve("equipment_grade_rules.csv"));
        var equipmentGradeRuleMap = ToDictionary(equipmentGradeRules, e => e.Grade, "equipment_grade_rules.csv", "grade", errors);
        var equipmentSubstatRanges = CsvMetadataReader.Load<EquipmentSubstatRange>(_resolver.Resolve("equipment_substat_ranges.csv"));
        var equipmentSocketUnlocks = CsvMetadataReader.Load<EquipmentSocketUnlock>(_resolver.Resolve("equipment_socket_unlock.csv"));
        var equipmentUpgradeCosts = CsvMetadataReader.Load<EquipmentUpgradeCost>(_resolver.Resolve("equipment_upgrade_cost.csv"));

        // Forge
        var forgeUpgradeRules = CsvMetadataReader.Load<ForgeUpgradeRule>(_resolver.Resolve("forge_upgrade_rules.csv"));
        var forgeSynthesisRules = CsvMetadataReader.Load<ForgeSynthesisRule>(_resolver.Resolve("forge_synthesis_rules.csv"));
        var forgeDecomposeRules = CsvMetadataReader.Load<ForgeDecomposeRule>(_resolver.Resolve("forge_decompose_rules.csv"));
        var forgeOptionChangeRules = CsvMetadataReader.Load<ForgeOptionChangeRule>(_resolver.Resolve("forge_option_change.csv"));
        var forgeSocketUnlocks = CsvMetadataReader.Load<ForgeSocketUnlock>(_resolver.Resolve("forge_socket_unlock.csv"));

        // Gems
        var gemTemplates = CsvMetadataReader.Load<GemTemplate>(_resolver.Resolve("gem_templates.csv"));
        var gemTemplateMap = ToDictionary(gemTemplates, g => g.GemId, "gem_templates.csv", "gem_id", errors);

        var gemDropTableEntries = CsvMetadataReader.Load<GemDropTableEntry>(_resolver.Resolve("gem_drop_tables.csv"));
        var gemDropTables = GroupByKey(gemDropTableEntries, g => g.DropId, "gem_drop_tables.csv", "drop_id", errors);
        foreach (var entry in gemDropTableEntries)
        {
            if (!gemTemplateMap.ContainsKey(entry.GemId))
            {
                errors.Add($"gem_drop_tables.csv: drop_id {entry.DropId}의 gem_id {entry.GemId}가 gem_templates.csv에 존재하지 않습니다.");
            }
        }

        var gemCombinationRules = CsvMetadataReader.Load<GemCombinationRule>(_resolver.Resolve("gem_combination_rules.csv"));

        // Heroes
        var heroTemplates = CsvMetadataReader.Load<HeroTemplate>(_resolver.Resolve("hero_templates.csv"));
        var heroTemplateMap = ToDictionary(heroTemplates, h => h.HeroId, "hero_templates.csv", "hero_id", errors);

        var heroUniqueEffects = CsvMetadataReader.Load<HeroUniqueEffect>(_resolver.Resolve("hero_unique_effects.csv"));
        var heroUniqueEffectMap = ToDictionary(heroUniqueEffects, e => e.EffectId, "hero_unique_effects.csv", "effect_id", errors);

        foreach (var template in heroTemplates)
        {
            foreach (var effectId in template.UniqueEffectIds)
            {
                if (!heroUniqueEffectMap.ContainsKey(effectId))
                {
                    errors.Add($"hero_templates.csv: hero_id {template.HeroId}의 unique_effect_id {effectId}가 hero_unique_effects.csv에 존재하지 않습니다.");
                }
            }
        }

        foreach (var template in enemyTemplates)
        {
            foreach (var effectId in template.UniqueEffectIds)
            {
                if (!string.IsNullOrWhiteSpace(effectId) && !heroUniqueEffectMap.ContainsKey(effectId))
                {
                    errors.Add($"enemy_templates.csv: enemy_id {template.EnemyId}의 unique_effect_id {effectId}가 hero_unique_effects.csv에 존재하지 않습니다.");
                }
            }
        }

        var heroGrowth = CsvMetadataReader.Load<HeroGrowth>(_resolver.Resolve("hero_growth.csv"));
        var heroRebirthRules = CsvMetadataReader.Load<HeroRebirthRule>(_resolver.Resolve("hero_rebirth_rules.csv"));
        var heroReleaseRewards = CsvMetadataReader.Load<HeroReleaseReward>(_resolver.Resolve("hero_release_rewards.csv"));
        var heroPoolExpansionRules = CsvMetadataReader.Load<HeroPoolExpansionRule>(_resolver.Resolve("hero_pool_expansion_rules.csv"));

        var heroDropTableEntries = CsvMetadataReader.Load<HeroDropTableEntry>(_resolver.Resolve("hero_drop_tables.csv"));
        var heroDropTables = GroupByKey(heroDropTableEntries, h => h.TableId, "hero_drop_tables.csv", "table_id", errors);
        foreach (var entry in heroDropTableEntries)
        {
            if (!heroTemplateMap.ContainsKey(entry.HeroId))
            {
                errors.Add($"hero_drop_tables.csv: table_id {entry.TableId}의 hero_id {entry.HeroId}가 hero_templates.csv에 존재하지 않습니다.");
            }
        }

        // Tavern
        var tavernRecruitmentTiers = CsvMetadataReader.Load<TavernRecruitmentTier>(_resolver.Resolve("tavern_recruitment_tiers.csv"));
        var tavernTierMap = ToDictionary(tavernRecruitmentTiers, t => t.TierId, "tavern_recruitment_tiers.csv", "tier_id", errors);

        var tavernCouponPrices = CsvMetadataReader.Load<TavernCouponPrice>(_resolver.Resolve("tavern_coupon_prices.csv"));
        var tavernCouponPriceMap = ToDictionary(tavernCouponPrices, c => c.TierId, "tavern_coupon_prices.csv", "tier_id", errors);
        foreach (var coupon in tavernCouponPrices)
        {
            if (!tavernTierMap.ContainsKey(coupon.TierId))
            {
                errors.Add($"tavern_coupon_prices.csv: tier_id {coupon.TierId}가 tavern_recruitment_tiers.csv에 존재하지 않습니다.");
            }
        }

        foreach (var hero in heroTemplates)
        {
            foreach (var tierId in hero.RecruitTierIds)
            {
                if (!tavernTierMap.ContainsKey(tierId))
                {
                    errors.Add($"hero_templates.csv: hero_id {hero.HeroId}의 recruit_tier_id {tierId}가 tavern_recruitment_tiers.csv에 존재하지 않습니다.");
                }
            }
        }

        // NPC & Settlements
        var npcServices = CsvMetadataReader.Load<NpcService>(_resolver.Resolve("npc_services.csv"));
        var npcServiceMap = ToDictionary(npcServices, s => s.ServiceId, "npc_services.csv", "service_id", errors);

        var settlementTemplates = CsvMetadataReader.Load<SettlementTemplate>(_resolver.Resolve("settlement_templates.csv"));
        var settlementTemplateMap = ToDictionary(settlementTemplates, s => s.SettlementId, "settlement_templates.csv", "settlement_id", errors);

        foreach (var settlement in settlementTemplates)
        {
            foreach (var serviceId in settlement.AvailableServices)
            {
                if (!npcServiceMap.ContainsKey(serviceId))
                {
                    errors.Add($"settlement_templates.csv: settlement_id {settlement.SettlementId}의 service_id {serviceId}가 npc_services.csv에 존재하지 않습니다.");
                }
            }
        }

        var settlementNpcMap = CsvMetadataReader.Load<SettlementNpcMapEntry>(_resolver.Resolve("settlement_npc_map.csv"));
        foreach (var map in settlementNpcMap)
        {
            if (!settlementTemplateMap.ContainsKey(map.SettlementId))
            {
                errors.Add($"settlement_npc_map.csv: settlement_id {map.SettlementId}가 settlement_templates.csv에 존재하지 않습니다.");
            }

            if (!npcServiceMap.ContainsKey(map.ServiceId))
            {
                errors.Add($"settlement_npc_map.csv: service_id {map.ServiceId}가 npc_services.csv에 존재하지 않습니다.");
            }
        }

        // Progression & Territory
        var rankRequirements = CsvMetadataReader.Load<RankRequirement>(_resolver.Resolve("rank_requirements.csv"));
        var rankRequirementMap = ToDictionary(rankRequirements, r => r.RankId, "rank_requirements.csv", "rank_id", errors);

        var flagTemplates = CsvMetadataReader.Load<FlagTemplate>(_resolver.Resolve("flag_templates.csv"));
        var flagTemplateMap = ToDictionary(flagTemplates, f => f.FlagId, "flag_templates.csv", "flag_id", errors);

        foreach (var flag in flagTemplates)
        {
            if (!string.IsNullOrWhiteSpace(flag.RankRequired) && !rankRequirementMap.ContainsKey(flag.RankRequired))
            {
                errors.Add($"flag_templates.csv: flag_id {flag.FlagId}의 rank_required {flag.RankRequired}가 rank_requirements.csv에 존재하지 않습니다.");
            }
        }

        var forbiddenTiles = CsvMetadataReader.Load<ForbiddenTile>(_resolver.Resolve("forbidden_tiles.csv"));

        // Shop
        var shopEquipmentPool = CsvMetadataReader.Load<ShopEquipmentPoolEntry>(_resolver.Resolve("shop_equipment_pool.csv"));
        var shopEquipmentPrices = CsvMetadataReader.Load<ShopEquipmentPrice>(_resolver.Resolve("shop_equipment_prices.csv"));
        var shopSharedPool = CsvMetadataReader.Load<ShopSharedPoolEntry>(_resolver.Resolve("shop_shared_pool.csv"));
        var shopSharedPrices = CsvMetadataReader.Load<ShopSharedPrice>(_resolver.Resolve("shop_shared_prices.csv"));
        var shopSharedPriceMap = ToDictionary(shopSharedPrices, s => s.ItemId, "shop_shared_prices.csv", "item_id", errors);
        var shopFlagInventory = CsvMetadataReader.Load<ShopFlagInventoryEntry>(_resolver.Resolve("shop_flag_inventory.csv"));
        var shopRefreshSchedule = CsvMetadataReader.Load<ShopRefreshScheduleEntry>(_resolver.Resolve("shop_refresh_schedule.csv"));

        foreach (var entry in shopFlagInventory)
        {
            if (!flagTemplateMap.ContainsKey(entry.FlagId))
            {
                errors.Add($"shop_flag_inventory.csv: flag_id {entry.FlagId}가 flag_templates.csv에 존재하지 않습니다.");
            }

            if (!string.IsNullOrWhiteSpace(entry.RankRequired) && !rankRequirementMap.ContainsKey(entry.RankRequired))
            {
                errors.Add($"shop_flag_inventory.csv: flag_id {entry.FlagId}의 rank_required {entry.RankRequired}가 rank_requirements.csv에 존재하지 않습니다.");
            }
        }

        foreach (var sharedItem in shopSharedPool)
        {
            if (!shopSharedPriceMap.ContainsKey(sharedItem.ItemId))
            {
                errors.Add($"shop_shared_pool.csv: item_id {sharedItem.ItemId}가 shop_shared_prices.csv에 존재하지 않습니다.");
            }
        }

        // Link hero unique effect check already done

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                _logger.LogError("Metadata validation error: {Error}", error);
            }

            _logger.LogError("Game metadata validation failed with {Count} errors", errors.Count);
            throw new MetadataValidationException(errors);
        }

        _logger.LogInformation("Game metadata loaded successfully.");

        return new GameMetadataSnapshot(
            enemyTemplateMap,
            enemyStatModifierMap,
            enemyDropTableMap,
            enemyEncounters,

            equipmentBaseStats,
            equipmentGradeRuleMap,
            equipmentSubstatRanges,
            equipmentSocketUnlocks,
            equipmentUpgradeCosts,

            forgeUpgradeRules,
            forgeSynthesisRules,
            forgeDecomposeRules,
            forgeOptionChangeRules,
            forgeSocketUnlocks,

            gemTemplateMap,
            gemDropTables,
            gemCombinationRules,

            heroTemplateMap,
            heroUniqueEffectMap,
            heroGrowth,
            heroRebirthRules,
            heroReleaseRewards,
            heroPoolExpansionRules,
            heroDropTables,

            tavernTierMap,
            tavernCouponPriceMap,

            npcServiceMap,
            settlementNpcMap,
            settlementTemplateMap,

            rankRequirementMap,
            flagTemplateMap,
            forbiddenTiles,

            shopEquipmentPool,
            shopEquipmentPrices,
            shopSharedPool,
            shopSharedPriceMap,
            shopFlagInventory,
            shopRefreshSchedule);
    }

    private static Dictionary<string, T> ToDictionary<T>(
        IEnumerable<T> source,
        Func<T, string> keySelector,
        string fileName,
        string keyName,
        List<string> errors)
        where T : class
    {
        var dictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (string.IsNullOrWhiteSpace(key))
            {
                errors.Add($"{fileName}: {keyName}가 비어 있습니다.");
                continue;
            }

            if (!dictionary.TryAdd(key, item))
            {
                errors.Add($"{fileName}: {keyName} {key}가 중복되었습니다.");
            }
        }

        return dictionary;
    }

    private static Dictionary<string, IReadOnlyList<T>> GroupByKey<T>(
        IEnumerable<T> source,
        Func<T, string> keySelector,
        string fileName,
        string keyName,
        List<string> errors)
    {
        var lookup = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (string.IsNullOrWhiteSpace(key))
            {
                errors.Add($"{fileName}: {keyName}가 비어 있습니다.");
                continue;
            }

            if (!lookup.TryGetValue(key, out var list))
            {
                list = new List<T>();
                lookup[key] = list;
            }

            list.Add(item);
        }

        return lookup.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyList<T>)new ReadOnlyCollection<T>(pair.Value),
            StringComparer.OrdinalIgnoreCase);
    }
}
