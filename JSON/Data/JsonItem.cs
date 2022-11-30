using Newtonsoft.Json;
using System.Collections.Generic;

namespace JSONLoader_BPH.JSON.Data;

public class JsonItem
{
    internal string _path;
    
    internal bool _autoLoadedSprite;
    
    public class ReferencedCreateEffect
    {
        public Item2.Trigger trigger;
        public Item2.CreateEffect.CreateType createType;
        public List<Item2.Area> areasToCreateTheItem;
        public List<string> itemsToCreateReference;
        public List<Item2.ItemType> typesToCreate;
        public List<Item2.Rarity> raritiesToCreate;
        public string descriptor;
        public int luckBonus;

        public enum CreateType
        {
            set,
            replace,
            inOrder,
            trueRandom,
            byType,
            inOpenSpace,
            inOpenSpaceByType,
            createCurse,
        }
    }

    public class ReferencedItemStatusEffect
    {
        public enum Type
        {
            disabled,
            enflamed,
            locked,
            heavy,
            conductive,
            buoyant,
            projectile,
            AddToManaCost,
            AddToEnergyCost,
            canBePlayedOverOtherItems,
            ductTape,
            canBeUsedByCR8Directly,
            runsAutomaticallyOnCoreUse,
            strengthBasedOnDistance,
            cannotBeFoundInStores,
            AddToGoldCost,
            piercing,
            cantBeSold,
            spawnsEvent,
            allowsItemsInIllusorySpaces,
            CR8ChargesReverseWhenOffGrid,
            canBeForged,
            cannotBeRotated,
            isActivated,
            canBeMovedInCombat,
            canBeMovedInCombatButReturnsToOriginalPosition,
            allowsFreeMove,
            petsAreSummonedBehindPochette,
            canOnlyBeHeldByPochette,
            effigy
        }

        public enum Length
        {
            whileActive,
            turns,
            combats,
            permanent
        }

        public Type type;

        public Length length;

        public int num;

        public List<string> prefabReference = new();
    }

    public class ReferencedEffect
    {
    public enum Type
        {
            Damage,
            Block,
            HP,
            AP,
            MaxHP,
            Luck,
            Poison,
            Regen,
            Spikes,
            Haste,
            Slow,
            Rage,
            Weak,
            resetUses,
            resetUsesPerCombat,
            resetUsesPerTurn,
            ToughHide,
            xxxOLD2xxx,
            xxxOLD3xxx,
            Destroy,
            Activate,
            AddToStack,
            ModifierMultiplier,
            Vampire,
            ItemStatusEffect,
            Mana,
            Dodge,
            AddCharge,
            ResetCharges,
            AllStatusEffects,
            AddDamageToScratch,
            AddToMaxMana,
            DrawToteCarvings,
            xxxOldxxx,
            DiscardCarving,
            Burn,
            BanishCarving,
            ChangeCostOfReorganize,
            Charm,
            Sleep,
            Freeze,
            GetGold,
            MaxHPPassive,
            SummonPet,
            RevivePets
        }

        public enum Target
        {
            unspecified,
            player,
            enemy,
            allEnemies,
            everyone,
            reactiveEnemy,
            allFriendlies,
            adjacentFriendlies,
            friendliesInFront,
            friendliesBehind,
            statusFromItem
        }

        public enum MathematicalType
        {
            summative,
            multiplicative
        }

        public Type type;

        public float value;

        public Target target;

        public MathematicalType mathematicalType;
        
        public List<ReferencedItemStatusEffect> itemStatusEffects = new();
    }

    public class ReferencedCombatEffects
    {
        public Item2.Trigger trigger;
        public ReferencedEffect effect;
    }

    public class ReferencedModifier
    {
        public enum Length
        {
            whileActive,
            forTurn,
            forCombat,
            untilRotate,
            untilUse,
            permanent,
            whileComboing,
            twoTurns
        }

        public string descriptionKey = "";

        public Item2.Trigger trigger;

        public List<ReferencedEffect> effects;

        public List<Item2.ItemType> typesToModify;

        public List<Item2.Area> areasToModify;

        public Item2.AreaDistance areaDistance;

        public Length length;

        public string name = "";

        public bool stackable;
    }
    
    public class ReferencedAddModifier
    {
        public string descriptionKey;

        public Item2.Trigger trigger;

        public List<Item2.ItemType> typesToModify;

        public List<Item2.Area> areasToModify;

        public Item2.AreaDistance areaDistance;

        public ReferencedModifier modifier;

    }
    
    public class BoxColliderString
    {
        public string size;
        public string offset;
    }
 
    public bool isAvailableInDemo = true;

    public bool isStandard = true;

    public bool isInAtlas = true;
    
    public List<Item2.ItemType> itemType = new();

    public Item2.Rarity rarity = Item2.Rarity.Common;

    public Item2.PlayType playType = Item2.PlayType.Active;
    
    public List<Item2.Cost> costs = new();
    
    public int charges = -999;
    
    public Item2.PlayerAnimation playerAnimation = Item2.PlayerAnimation.Attack;
    
    public Item2.SoundEffect soundEffect = Item2.SoundEffect.None;
    
    public List<string> descriptions = new();
    
    public Item2.Area moveArea = Item2.Area.self;
    
    public Item2.ItemType mustBePlacedOnItemType = Item2.ItemType.Grid;
    
    public Item2.ItemType mustBePlacedOnItemTypeInCombat = Item2.ItemType.Grid;
    
    public Item2.AreaDistance moveDistance = Item2.AreaDistance.all;
        
    [JsonProperty("name")]
    public string Name { get; set; }

    public List<DungeonLevel.Zone> validForZones = new(){ DungeonLevel.Zone.dungeon };
    
    public List<Character.CharacterName> validForCharacters = new(){ Character.CharacterName.Any };
    
    public List<ReferencedCombatEffects> combatEffects = new();
    
    public List<Item2.LimitedUses> usesLimits = new();
    
    public List<ReferencedCreateEffect> createEffects = new();
    
    public List<ReferencedModifier> modifiers = new();
    
    public List<ReferencedAddModifier> addModifiers = new();
    
    public List<Item2.MovementEffect> movementEffects = new();
    
    public List<ReferencedItemStatusEffect> activeItemStatusEffects = new();
    
    public List<ContextMenuButton.ContextMenuButtonAndCost> contextMenuOptions = new();
    
    public float spriteScale = 1f;
    
    public string Sprite = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAIAAABLbSncAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAFiUAABYlAUlSJPAAAAAbSURBVBhXY/jP8B+OGJABFSWQAU5FlEgw/AcA6TA/wcc3lCAAAAAASUVORK5CYII=";
    
    public List<BoxColliderString> Shape = new() { new BoxColliderString() { size = "1x1", offset = "0,0" } };
}