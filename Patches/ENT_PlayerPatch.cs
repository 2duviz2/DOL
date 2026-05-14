namespace DOL.Patches;

using DOL.Classes;
using HarmonyLib;

[HarmonyPatch(typeof(ENT_Player))]
public static class ENT_PlayerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ENT_Player), nameof(ENT_Player.Start))]
    public static void Start(ENT_Player __instance)
    {
        NetworkManager.LocalPlayer = __instance;
        UIManager.CreateUI();
    }
}
