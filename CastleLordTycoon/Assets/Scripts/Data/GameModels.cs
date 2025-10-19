using System;
using System.Collections.Generic;

namespace CastleLordTycoon.Data
{
    #region Enums

    /// <summary>
    /// 속성 (물 > 불 > 땅 > 물)
    /// </summary>
    public enum Element
    {
        Water,
        Fire,
        Earth,
        None
    }

    /// <summary>
    /// 장비 등급
    /// </summary>
    public enum Grade
    {
        C,      // 흰색
        UC,     // 연두색
        R,      // 노란색
        H,      // 보라색
        L       // 연한주황색
    }

    /// <summary>
    /// 장비 타입
    /// </summary>
    public enum EquipmentType
    {
        Weapon,
        Armor,
        Ring,
        Neckless,
        Belt
    }

    /// <summary>
    /// 효과 타입
    /// </summary>
    public enum EffectType
    {
        Offensive,
        Defensive,
        Utility
    }

    #endregion

    #region Hero

    /// <summary>
    /// 영웅
    /// </summary>
    [Serializable]
    public class Hero
    {
        // 식별
        public string id;
        public string templateId;
        public string type;

        // 등급
        public int starGrade;           // 1-6

        // 기본 정보
        public string name;
        public Element element;

        // 레벨 & 경험치
        public int level;
        public int currentExp;

        // 스탯
        public HeroStats stats;

        // 성장
        public Stats baseStats;
        public Stats growthRates;
        public int rebirths;

        // 효과 (★4+)
        public List<Effect> uniqueEffects;

        // 상태
        public bool isDead;
        public bool isInParty;
        public EquippedItems equippedItems;
    }

    /// <summary>
    /// 영웅 스탯
    /// </summary>
    [Serializable]
    public class HeroStats
    {
        public float hp;
        public float maxHp;
        public float attack;
        public float defense;
        public float criticalRate;
        public float criticalDamage;
        public float blockRate;
    }

    /// <summary>
    /// 기본 스탯 (성장용)
    /// </summary>
    [Serializable]
    public class Stats
    {
        public float hp;
        public float attack;
        public float defense;
    }

    /// <summary>
    /// 장착 아이템
    /// </summary>
    [Serializable]
    public class EquippedItems
    {
        public string weapon;
        public string armor;
        public string accessory1;
        public string accessory2;
    }

    /// <summary>
    /// 효과 (고유 효과)
    /// </summary>
    [Serializable]
    public class Effect
    {
        public string id;
        public string name;
        public string description;
        public EffectType type;
        public float value;
    }

    #endregion

    #region Equipment

    /// <summary>
    /// 장비
    /// </summary>
    [Serializable]
    public class Equipment
    {
        // 식별
        public string id;
        public string templateId;

        // 기본 정보
        public string name;
        public Grade grade;
        public EquipmentType type;
        public int requiredLevel;

        // 스탯
        public BaseStat baseStat;

        // 옵션
        public List<EquipmentOption> options;

        // 상태
        public bool equipped;
        public string equippedBy;       // hero ID
    }

    /// <summary>
    /// 기본 스탯
    /// </summary>
    [Serializable]
    public class BaseStat
    {
        public string type;             // "attack" | "defense" | "hp"
        public float value;
    }

    /// <summary>
    /// 장비 옵션
    /// </summary>
    [Serializable]
    public class EquipmentOption
    {
        public string type;
        public float value;
    }

    #endregion

    #region Player

    /// <summary>
    /// 플레이어 정보
    /// </summary>
    [Serializable]
    public class PlayerProfile
    {
        public string userId;
        public string username;
        public int level;
        public long gold;
        public Position position;
        public string rank;
        public int territoryCount;
        public int townCount;
    }

    /// <summary>
    /// 위치
    /// </summary>
    [Serializable]
    public class Position
    {
        public int x;
        public int y;
    }

    #endregion

    #region Combat

    /// <summary>
    /// 전투 시작 요청
    /// </summary>
    [Serializable]
    public class StartBattleRequest
    {
        public string[] partyHeroIds;   // 최대 4명
        public string encounterType;
        public string targetId;
    }

    /// <summary>
    /// 전투 결과
    /// </summary>
    [Serializable]
    public class BattleResult
    {
        public string battleId;
        public bool victory;
        public int totalRounds;
        public BattleReward[] rewards;
        public HeroExpGain[] expGains;
        public BattleLog[] logs;
    }

    /// <summary>
    /// 전투 보상
    /// </summary>
    [Serializable]
    public class BattleReward
    {
        public string type;             // "gold" | "item" | "equipment"
        public string itemId;
        public int quantity;
    }

    /// <summary>
    /// 영웅 경험치 획득
    /// </summary>
    [Serializable]
    public class HeroExpGain
    {
        public string heroId;
        public int expGained;
        public int newLevel;
        public bool leveledUp;
    }

    /// <summary>
    /// 전투 로그
    /// </summary>
    [Serializable]
    public class BattleLog
    {
        public int round;
        public string actorId;
        public string action;
        public string targetId;
        public float damage;
        public bool critical;
    }

    #endregion

    #region Territory

    /// <summary>
    /// 깃발 설치 요청
    /// </summary>
    [Serializable]
    public class PlaceFlagRequest
    {
        public Position position;
        public string flagSize;         // "S" | "M" | "L"
    }

    /// <summary>
    /// 영토 정보
    /// </summary>
    [Serializable]
    public class Territory
    {
        public string id;
        public string ownerId;
        public Position flagPosition;
        public string flagSize;
        public Position[] controlledTiles;
        public string createdAt;
    }

    #endregion

    #region Town

    /// <summary>
    /// 마을 정보
    /// </summary>
    [Serializable]
    public class Town
    {
        public string id;
        public string name;
        public Position position;
        public string ownerId;
        public string capturedAt;
        public string[] availableServices;     // "shop" | "heal" | "storage"
    }

    #endregion
}
