namespace DOL.Patches;

using DOL.Classes;
using HarmonyLib;

[HarmonyPatch(typeof(WorldLoader))]
public static class WorldLoaderPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldLoader), nameof(WorldLoader.Awake))]
    public static void Awake(WorldLoader __instance)
    {
        if (NetworkManager.IsClient)
        {
            __instance.seed = __instance.startingSeed = Player.NetSeed;
        }
    }
}
