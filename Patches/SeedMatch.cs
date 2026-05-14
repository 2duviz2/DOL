namespace DOL.Patches;

using DOL.Classes;
using HarmonyLib;

[HarmonyPatch(typeof(UT_RadiusSpawner))]
public static class UT_RadiusSpawnerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UT_RadiusSpawner), nameof(UT_RadiusSpawner.Start))]
    public static void Start(UT_RadiusSpawner __instance)
    {
        if (NetworkManager.CurrentState == GameState.Offline) return;
        __instance.seed = Client.NetSeed;
    }
}

[HarmonyPatch(typeof(UT_TriggerSpawner))]
public static class UT_TriggerSpawnerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UT_TriggerSpawner), nameof(UT_TriggerSpawner.Start))]
    public static void Start(UT_TriggerSpawner __instance)
    {
        if (NetworkManager.CurrentState == GameState.Offline) return;
        __instance.seed = Client.NetSeed;
    }
}

[HarmonyPatch(typeof(UT_MeshFaceSpawner))]
public static class UT_MeshFaceSpawnerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UT_MeshFaceSpawner), nameof(UT_MeshFaceSpawner.Start))]
    public static void Start(UT_MeshFaceSpawner __instance)
    {
        if (NetworkManager.CurrentState == GameState.Offline) return;
        __instance.seed = Client.NetSeed;
    }
}

[HarmonyPatch(typeof(M_Level), nameof(M_Level.Awake))]
public class M_LevelAwakePatch
{
    public static void Prefix(M_Level __instance)
    {
        if (NetworkManager.CurrentState == GameState.Offline) return;
        __instance.canFlip = false;
    }
}

[HarmonyPatch(typeof(WorldLoader), nameof(WorldLoader.IncrementSeed))]
public class IncrementSeedPatch
{
    public static bool Prefix()
    {
        if (NetworkManager.CurrentState == GameState.Offline) return true;
        return false;
    }
}

[HarmonyPatch(typeof(CL_ProgressionManager), nameof(CL_ProgressionManager.HasProgressionUnlock))]
public class HasProgressionUnlockPatch
{
    public static bool Prefix(ref bool __result)
    {
        if (NetworkManager.CurrentState != GameState.Offline)
        {
            __result = true;
            return false;
        }
        return true;
    }
}