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
        __instance.seed = Client.NetSeed;
    }
}

[HarmonyPatch(typeof(M_Level))]
public static class M_LevelPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(M_Level), nameof(M_Level.SetFlipped))]
    public static bool SetFlipped(M_Level __instance)
    {
        return false;
    }
}
