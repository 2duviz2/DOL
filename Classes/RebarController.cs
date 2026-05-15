namespace DOL.Classes;

using HarmonyLib;
using UnityEngine;

public static class RebarController
{
    public static void SpawnItemShoot(Vector3 position, Vector3 direction, Vector3 normalized, string id = "Item_Rebar")
    {
        var shoot = CL_AssetManager.GetItemObjectPrefab(id).itemData.handItemAsset as HandItem_Shoot;

        var obj = GameObject.Instantiate(shoot.projectile, position, Quaternion.Euler(direction));
        var projectile = obj.GetComponent<Projectile>();

        projectile.Initialize(normalized);
    }

    public static void SendItemShoot(Vector3 position, Vector3 direction, Vector3 normalized, string id = "Item_Rebar")
    {
        NetworkManager.SendPacket("itemShoot", position.x, position.y, position.z, direction.x, direction.y, direction.z, normalized.x, normalized.y, normalized.z, id);
    }
}

[HarmonyPatch(typeof(HandItem_Shoot))]
public static class RebarPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HandItem_Shoot), nameof(HandItem_Shoot.Shoot))]
    public static void ShootPrefix(HandItem_Shoot __instance)
    {
        if (NetworkManager.CurrentState != GameState.Offline)
        {
            int frameCount = Time.frameCount;
            if (frameCount - __instance.lastShotFrame <= 2) return;

            Vector3 vector = Vector3.Lerp(__instance.transform.position, Camera.main.transform.position, 0.8f) + -Camera.main.transform.forward * 0.4f;

            RaycastHit raycastHit;
            Vector3 normalized;

            Vector3 vector2 = Vector3.zero; // remove inaccuracy

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, float.PositiveInfinity, __instance.aimMask))
                normalized = ((raycastHit.point - __instance.transform.position).normalized + vector2).normalized;
            else
                normalized = (Camera.main.transform.forward + vector2).normalized;

            RebarController.SendItemShoot(vector, Quaternion.LookRotation(__instance.transform.forward).eulerAngles, normalized * __instance.shootSpeed, __instance.item.prefabName);
        }
    }

    public static Vector3 GetInaccuracy(HandItem_Shoot __instance)
    {
        Vector3 vector2 = Random.insideUnitSphere * Mathf.Max(__instance.curShotInaccuracy, __instance.initialShotInaccuracy);
        vector2 = Vector3.ClampMagnitude(vector2, __instance.maxShotInaccuracyMagnitude);
        return vector2;
    }
}