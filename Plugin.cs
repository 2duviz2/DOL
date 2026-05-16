namespace DOL;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using DOL.Helpers;
using UnityEngine;
using DOL.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DOL.Classes;
using Steamworks;
using DOL.Classes.Endpoints;

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
        /*try { SteamClient.Init(3195790); }
        catch { }*/

        Player.CreatePlayer();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public void Update()
    {
        if (NetworkManager.CurrentState != GameState.Offline)
        {
            if (NetworkManager.CurrentState == GameState.Host)
                NetworkManager.server.manager.Receive();
            else
                NetworkManager.client.manager.Receive();
        }
    }

    public void SceneLoaded(Scene scene, LoadSceneMode __)
    {
        if (scene.name == "Main-Menu")
        {
            NetworkManager.LeaveLobby();
        }
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
    public const string Version = "0.0.1";
}