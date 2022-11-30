using HarmonyLib;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JSONLoader_BPH.Managers;

[HarmonyPatch]
public class ItemPatcher
{
    // Since modded items are not prefabs, they start as inactive game objects and are activated when created.
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Object), "Instantiate", typeof(GameObject), typeof(Vector3), typeof(Quaternion))]
    public static void Object_Instantiate(Object __result)
    {
        if (__result is GameObject go && go != null && go.GetComponent<Item2>() != null)
        {
            go.SetActive(true);
        }
    }

    // This fixes modded items not having sprites when loaded from the save file
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Item2), "OnEnable")]
    public static void Item2_OnEnable(Item2 __instance)
    {
        if (__instance.gameObject.GetComponent<SpriteRenderer>().sprite == null)
        {
            var allItems = ItemManager._itemsLoadedFor.GetComponent<DebugItemManager>().items;
            var item = allItems.Find(
                x => x.name == __instance.gameObject.name || x.name + "(Clone)" == __instance.gameObject.name);
            if (item != null)
            {
                Plugin.Log.Msg($"Found item {item.name}");
                __instance.gameObject.GetComponent<SpriteRenderer>().sprite =
                    item.GetComponent<SpriteRenderer>().sprite;
                __instance.gameObject.transform.Find("MousePreview").GetComponent<SpriteRenderer>().sprite =
                    item.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    // Overrides strings starting with '/' to be literal
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LangaugeManager), "GetTextByKey")]
    public static bool LangaugeManager_GetTextByKey(ref string __result, string key)
    {
        if (key.StartsWith("/"))
        {
            __result = key.Substring(1);
            return false;
        }

        return true;
    }

    // Creates descriptions for modifiers that don't have valid localization keys
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Card), "GetModifierDescription")]
    public static void Card_GetModifierDescription(ref string __result, Item2.Modifier modifier, bool showOrigin)
    {
        if (__result == "")
        {
            var effect = modifier.effects[0];
            if (effect.value <= 0.0001) __result = $"{effect.type}";
            else if (effect.value <= 1.0001) __result = $"Apply {effect.type}";
            else __result = $"Apply {effect.value} {effect.type}";
            if (effect.target != Item2.Effect.Target.unspecified) __result += $" to {effect.target}";
            if (modifier.length != Item2.Modifier.Length.permanent &&
                modifier.length != Item2.Modifier.Length.whileActive) __result += $" for {modifier.length}";
            string areas = String.Join(", ", modifier.areasToModify);
            if (modifier.areasToModify.Count == 1 && modifier.areasToModify[0] == Item2.Area.self)
                areas = "";
            if (areas != "")
            {
                if (modifier.areasToModify.Count == 1)
                {
                    __result += $" in {areas}";
                }
                else
                {
                    __result += $" in ({areas})";
                }

                if (modifier.areaDistance == Item2.AreaDistance.adjacent)
                {
                    __result += " (adjacent)";
                }
                else if (modifier.areaDistance == Item2.AreaDistance.closest)
                {
                    __result += " (closest)";
                }
            }

            string types = String.Join(", ", modifier.typesToModify);
            if (modifier.typesToModify.Count == 1 && modifier.typesToModify[0] == Item2.ItemType.Any)
                types = "";
            if (types != "")
            {
                if (effect.value > 0.0001) __result += " on";
                if (modifier.typesToModify.Count == 1)
                {
                    __result += $" type {types}";
                }
                else
                {
                    __result += $" types ({types})";
                }
            }

            if (effect.mathematicalType == Item2.Effect.MathematicalType.multiplicative)
            {
                __result += " (multiplicative)";
            }

            if (modifier.effects.Count > 1)
            {
                __result += " and ";
                for (int i = 1; i < modifier.effects.Count; i++)
                {
                    effect = modifier.effects[i];
                    __result += $" apply {effect.value} {effect.type}";
                    if (effect.target != Item2.Effect.Target.unspecified) __result += $" to {effect.target}";
                    if (effect.mathematicalType == Item2.Effect.MathematicalType.multiplicative)
                    {
                        __result += " (multiplicative)";
                    }

                    if (i < modifier.effects.Count - 1)
                    {
                        __result += " and ";
                    }
                }
            }
        }
    }

    // Creates descriptions for triggers that don't have valid localization keys
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Card), "GetTriggerDescription")]
    public static void Card_GetTriggerDescription(ref string __result, Item2.Trigger trigger, bool plural)
    {
        if (__result == "")
        {
            __result = $"When {trigger.trigger} happens";
            string areas = String.Join(", ", trigger.areas);
            if (trigger.areas.Count == 1 && trigger.areas[0] == Item2.Area.self)
                areas = "";
            if (areas != "")
            {
                if (trigger.areas.Count == 1)
                {
                    __result += $" in {areas}";
                }
                else
                {
                    __result += $" in ({areas})";
                }

                if (trigger.areaDistance == Item2.AreaDistance.adjacent)
                {
                    __result += "(adjacent) ";
                }
                else if (trigger.areaDistance == Item2.AreaDistance.closest)
                {
                    __result += "(closest) ";
                }
            }

            string types = String.Join(", ", trigger.types);
            if (trigger.types.Count == 1 && trigger.types[0] == Item2.ItemType.Any)
                types = "";
            if (types != "")
            {
                if (trigger.types.Count == 1)
                {
                    __result += $" on type {types}";
                }
                else
                {
                    __result += $" on types ({types})";
                }
            }

            __result += ":";
        }
    }

    // Creates descriptions for effects that don't have valid localization keys
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Card), "GetEffectTotalDescription")]
    public static void Card_GetEffectTotalDescription(ref string __result, Item2.EffectTotal effectTotal)
    {
        if (__result == "")
        {
            Item2.Effect effect = effectTotal.effect;
            __result = $"Apply {effect.value} {effect.type}";
            if (effect.target != Item2.Effect.Target.unspecified) __result += $" to {effect.target}";
            if (effect.mathematicalType == Item2.Effect.MathematicalType.multiplicative)
            {
                __result += " (multiplicative)";
            }
        }
    }
}