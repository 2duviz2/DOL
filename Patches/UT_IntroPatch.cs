namespace DOL.Patches;

using HarmonyLib;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(UT_Intro))]
public static class UT_IntroPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UT_Intro), nameof(UT_Intro.Start))]
    public static bool Start(UT_Intro __instance)
    {
        SceneManager.LoadScene(__instance.loadScene);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UT_Intro), nameof(UT_Intro.Update))]
    public static bool Update()
    {
        return false;
    }
}
