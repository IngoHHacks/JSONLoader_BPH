using HarmonyLib;
using JSONLoader_BPH.JSON;
using MelonLoader;
using System.IO;
using System.Linq;
using Plugin = JSONLoader_BPH.Plugin;

[assembly: MelonInfo(typeof(Plugin), Plugin.PluginName, Plugin.PluginVer, Plugin.PluginAuthor)]

namespace JSONLoader_BPH;

[HarmonyPatch]
public class Plugin : MelonMod
{
    public const string PluginName = "JSONLoader_BPH";
    public const string PluginVer = "1.0.0";
    public const string PluginAuthor = "IngoH";
    
    internal static MelonLogger.Instance Log;

    private static MelonPreferences_Category _jldr;
    internal static MelonPreferences_Entry<bool> DebugMode;

    public override void OnInitializeMelon()
    {
        _jldr = MelonPreferences.CreateCategory("JSONLoader");
        _jldr.SetFilePath(Path.Combine(MelonUtils.UserDataDirectory, "JSONLoader.cfg"));
        DebugMode = _jldr.CreateEntry("DebugMode", false, description: "Deletes all vanilla items.");
        _jldr.SaveToFile();
    }

    public override void OnApplicationStart()
    {
        Log = LoggerInstance;
        Log.Msg("JSONLoader_BPH loaded");
        
        LoadFiles();
    }
    
    internal static void LoadFiles()
    {
        // Change GameDirectory to Plugin directory
        string[] files = Directory.GetFiles(Path.Combine(MelonUtils.BaseDirectory, "Mods"), "*.json", SearchOption.AllDirectories);
        if (files.Length == 0) return;
        
        string[] items = files.Where(x => x.EndsWith("_item.json")).ToArray();
        JsonManager.LoadItems(items);

        /*
        string[] encounters = files.Where(x => x.EndsWith("_encounter.json")).ToArray();
        JsonManager.LoadEncounters(encounters);
        */
    }
}
