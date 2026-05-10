namespace DOL;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using DOL.Helpers;
using UnityEngine;
using DOL.UI;
using TMPro;

[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin instance;
    public static ConfigFile config;

    public void Awake()
    {
        instance = this;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        config = Config;

        new Harmony(PluginInfo.GUID).PatchAll();

        Debug.unityLogger.filterLogType = LogType.Error | LogType.Warning | LogType.Exception;
    }

    public void Start()
    {
        Canvas c = Builder.Canvas();
        DontDestroyOnLoad(c);
        TMP_Text t = Builder.Text(c.gameObject, new Vector2(0, 0), new Vector2(200, 200), "Ella jura");
    }

    public static T Ass<T>(string path) => AssHelper.Ass<T>(path);
    public static void LogInfo(object msg) => instance.Logger.LogInfo(msg);
    public static void LogWarning(object msg) => instance.Logger.LogWarning(msg);
    public static void LogError(object msg) => instance.Logger.LogError(msg);
}

public class PluginInfo
{
    public const string GUID = "duviz.DOL";
    public const string Name = "DOL";
    public const string Version = "0.1.0";
}