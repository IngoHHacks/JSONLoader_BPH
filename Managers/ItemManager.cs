using HarmonyLib;
using JSONLoader_BPH.JSON;
using JSONLoader_BPH.JSON.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JSONLoader_BPH.Managers;

[HarmonyPatch]
public class ItemManager
{
    public static bool ItemsLoaded = false;
    internal static GameManager _itemsLoadedFor;

    public static List<JsonItem> ModdedItems = new();

    public static void AddItem(JsonItem item)
    {
        ModdedItems.Add(item);
    }

    private static Action LinkItems;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameManager), "BuildItemLists")]
    public static void GameManager_BuildItemLists(GameManager __instance)
    {
        LinkItems = () => { };
        if (!ItemsLoaded || __instance != _itemsLoadedFor)
        {
            DebugItemManager component = __instance.GetComponent<DebugItemManager>();
            List<GameObject> items = component.items;
            var templateItem = items[0];
            if (Plugin.DebugMode.Value)
            {
                items.Clear();
            }
            foreach (JsonItem jsonItem in ModdedItems)
            {
                GameObject item = Object.Instantiate(templateItem);
                Object.DontDestroyOnLoad(item);
                item.SetActive(false);
                Object.Destroy(item.GetComponent<Item2>());
                Item2 item2 = item.AddComponent<Item2>();
                item2.itemType = new();

                JsonConvert.PopulateObject(JsonConvert.SerializeObject(jsonItem, JsonManager.DefaultItemConversionSettings),
                    item2, JsonManager.DefaultItemConversionSettings);
                Texture2D texture2D = new(1, 1, TextureFormat.RGBA32, false, false) { filterMode = FilterMode.Point };
                if (jsonItem.Sprite.StartsWith("data:image/png;base64,"))
                {
                    byte[] imageBytes = Convert.FromBase64String(jsonItem.Sprite.Substring(22));
                    texture2D.LoadImage(imageBytes);
                }
                else
                {
                    if (Path.IsPathRooted(jsonItem.Sprite) && !jsonItem._autoLoadedSprite)
                    {
                        Plugin.Log.Warning("Using rooted path. This will likely cause issues when sharing the item!");
                        texture2D.LoadImage(File.ReadAllBytes(jsonItem.Sprite));
                    }
                    else
                    {
                        texture2D.LoadImage(File.ReadAllBytes(Path.Combine(jsonItem._path, jsonItem.Sprite)));
                    }
                }
                Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                    new Vector2(0.5f, 0.5f), 16f / jsonItem.spriteScale);
                item.GetComponent<SpriteRenderer>().sprite = sprite;
                item.transform.Find("MousePreview").GetComponent<SpriteRenderer>().sprite = sprite;
                foreach (BoxCollider2D boxCollider2D in item.GetComponents<BoxCollider2D>())
                {
                    Object.Destroy(boxCollider2D);
                }
                foreach (JsonItem.BoxColliderString boxCollider2D in jsonItem.Shape)
                {
                    BoxCollider2D boxCollider2D2 = item.AddComponent<BoxCollider2D>();
                    string[] size = boxCollider2D.size.Replace("x", ",").Split(',');
                    string[] offset = boxCollider2D.offset.Replace("x", ",").Split(',');
                    boxCollider2D2.size = new(float.Parse(size[0]), float.Parse(size[1]));
                    boxCollider2D2.offset = new(float.Parse(offset[0]), float.Parse(offset[1]));
                }
                if (jsonItem.createEffects.Count > 0)
                {
                    LinkItems += () =>
                    {
                        for (int index = 0; index < jsonItem.createEffects.Count; index++)
                        {
                            JsonItem.ReferencedCreateEffect effect = jsonItem.createEffects[index];
                            Item2.CreateEffect createEffect = item2.createEffects[index];
                            if (effect.itemsToCreateReference.Count > 0)
                            {
                                foreach (var typeToCreate in effect.itemsToCreateReference)
                                {
                                    var gameObject = items.Find(x =>
                                        x.name == typeToCreate || x.name == typeToCreate + " Variant" ||
                                        x.name == "/" + typeToCreate);
                                    if (gameObject != null)
                                    {
                                        if (createEffect.itemsToCreate == null)
                                        {
                                            createEffect.itemsToCreate = new List<GameObject>();
                                        }
                                        createEffect.itemsToCreate.Add(gameObject);
                                    }
                                    else
                                    {
                                        Debug.LogError($"Could not find item with name {typeToCreate}");
                                    }
                                }
                            }
                        }
                    };
                }
                if (jsonItem.combatEffects.Count > 0)
                {
                    LinkItems += () =>
                    {
                        for (int index = 0; index < jsonItem.combatEffects.Count; index++)
                        {
                            JsonItem.ReferencedCombatEffects effect = jsonItem.combatEffects[index];
                            Item2.CombattEffect combatEffect = item2.combatEffects[index];
                            if (effect.effect != null)
                            {
                                if (effect.effect.itemStatusEffects.Count > 0)
                                {
                                    for (int index2 = 0; index2 < effect.effect.itemStatusEffects.Count; index2++)
                                    {
                                        JsonItem.ReferencedItemStatusEffect statusEffect =
                                            effect.effect.itemStatusEffects[index2];
                                        foreach (string prefabReference in statusEffect.prefabReference)
                                        {
                                            var gameObject = GameObject.FindGameObjectWithTag(prefabReference);
                                            if (gameObject != null)
                                            {
                                                if (combatEffect.effect.itemStatusEffect[index2].prefabs == null)
                                                {
                                                    combatEffect.effect.itemStatusEffect[index2].prefabs =
                                                        new List<GameObject>();
                                                }
                                                combatEffect.effect.itemStatusEffect[index2].prefabs.Add(gameObject);
                                            }
                                            else
                                            {
                                                Debug.LogError($"Could not find prefab with tag {prefabReference}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                }
                if (jsonItem.modifiers.Count > 0)
                {
                    LinkItems += () =>
                    {
                        for (int index = 0; index < jsonItem.modifiers.Count; index++)
                        {
                            JsonItem.ReferencedModifier mod = jsonItem.modifiers[index];
                            Item2.Modifier modifier = item2.modifiers[index];
                            if (mod.effects.Count > 0)
                            {
                                for (int index2 = 0; index2 < mod.effects.Count; index2++)
                                {
                                    JsonItem.ReferencedEffect effect = mod.effects[index2];
                                    if (effect.itemStatusEffects.Count > 0)
                                    {
                                        for (int index3 = 0; index3 < effect.itemStatusEffects.Count; index3++)
                                        {
                                            JsonItem.ReferencedItemStatusEffect statusEffect =
                                                effect.itemStatusEffects[index3];
                                            foreach (string prefabReference in statusEffect.prefabReference)
                                            {
                                                var gameObject = GameObject.FindGameObjectWithTag(prefabReference);
                                                if (gameObject != null)
                                                {
                                                    if (modifier.effects[index2].itemStatusEffect[index3].prefabs ==
                                                        null)
                                                    {
                                                        modifier.effects[index2].itemStatusEffect[index3].prefabs =
                                                            new List<GameObject>();
                                                    }
                                                    modifier.effects[index2].itemStatusEffect[index3].prefabs.Add(gameObject);
                                                }
                                                else
                                                {
                                                    Debug.LogError($"Could not find prefab with tag {prefabReference}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                }
                if (jsonItem.activeItemStatusEffects.Count > 0)
                {
                    LinkItems += () =>
                    {
                        for (int index = 0; index < jsonItem.activeItemStatusEffects.Count; index++)
                        {
                            JsonItem.ReferencedItemStatusEffect statusEffect =
                                jsonItem.activeItemStatusEffects[index];
                            foreach (string prefabReference in statusEffect.prefabReference)
                            {
                                var gameObject = GameObject.FindGameObjectWithTag(prefabReference);
                                if (gameObject != null)
                                {
                                    if (item2.activeItemStatusEffects[index].prefabs == null)
                                    {
                                        item2.activeItemStatusEffects[index].prefabs = new List<GameObject>();
                                    }
                                    item2.activeItemStatusEffects[index].prefabs.Add(gameObject);
                                }
                                else
                                {
                                    Debug.LogError($"Could not find prefab with tag {prefabReference}");
                                }
                            }
                        }
                    };
                }

                items.Add(item);
            }

            LinkItems();
            ItemsLoaded = true;
            _itemsLoadedFor = __instance;
        }
    }
}