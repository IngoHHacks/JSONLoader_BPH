using JSONLoader_BPH.JSON.Data;
using JSONLoader_BPH.Managers;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.IO;

namespace JSONLoader_BPH.JSON;

public class JsonManager {
    
    public static void LoadItems(params string[] paths)
    {
        foreach (string path in paths)
        {
            string json = File.ReadAllText(path);
            try
            {
                JsonItem item = JsonConvert.DeserializeObject<JsonItem>(json, DefaultItemCreationSettings);
                if (item != null)
                {
                    if (File.Exists(path.Substring(0, path.Length - 5) + ".png"))
                    {
                        item.Sprite = path.Substring(0, path.Length - 5) + ".png";
                        item._autoLoadedSprite = true;
                    }
                    if (File.Exists(path.Substring(0, path.Length - 10) + ".png"))
                    {
                        item.Sprite = path.Substring(0, path.Length - 10) + ".png";
                        item._autoLoadedSprite = true;
                    }
                    ItemManager.AddItem(item);
                    item._path = path;
                    Plugin.Log.Msg($"Loaded {item.Name} from {path.Replace(MelonUtils.BaseDirectory, ".")}");
                    if (!item.Name.StartsWith("/"))
                    {
                        item.Name = "/" + item.Name;
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Error("Error loading file: " + path);
                Plugin.Log.Error(e.Message);
                Plugin.Log.Error(e.StackTrace);
            }
        }
    }
    
    public static void LoadEncounters(params string[] paths)
    {
        // TBA
    }

    public static readonly JsonSerializerSettings DefaultItemConversionSettings =
        new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
    
    public static readonly JsonSerializerSettings DefaultItemCreationSettings =
        new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
}