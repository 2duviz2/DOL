namespace DOL.Classes;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class PitonController
{
    public static void SpawnItemPiton(Vector3 position, Vector3 direction, float buff, string id = "Item_Piton", int entityId = 0, bool owned = false)
    {
        var piton = CL_AssetManager.GetItemObjectPrefab(id).itemData.handItemAsset as HandItem_Piton;

        var pitonObject = piton.pitonWorldObject;

        var obj = GameObject.Instantiate(pitonObject, position, Quaternion.Euler(direction));

        var handhold = obj.GetComponent<CL_Handhold>();
        if (handhold != null && !handhold.secure)
        {
            handhold.HammerIn(buff);
        }

        AddPitonListener(obj, entityId, owned);
    }

    public static void SendItemPiton(Vector3 position, Vector3 direction, float buff, string id = "Item_Piton")
    {
        var entityId = Random.RandomRangeInt(0, 1000000);
        NetworkManager.SendPacket("itemPiton", position.x, position.y, position.z, direction.x, direction.y, direction.z, buff, id, entityId);
        SpawnItemPiton(position, direction, buff, id, entityId, true);
    }

    public static void AddPitonListener(GameObject piton, int id, bool owned)
    {
        if (piton == null) return;

        PitonListener.AddPiton(piton.AddComponent<PitonListener>(), id, owned);
    }
}


public class PitonListener : MonoBehaviour
{
    public static List<PitonListener> instances = [];

    public int id;
    public bool owned;

    public static void CleanList()
    {
        foreach (var instance in instances.ToList())
            if (instance == null)
                instances.Remove(instance);
    }

    public static PitonListener GetPiton(int id)
    {
        foreach (var piton in instances)
        {
            if (piton.id == id)
            {
                return piton;
            }
        }

        return null;
    }

    public static void AddPiton(PitonListener piton, int id, bool owned = false)
    {
        piton.id = id;
        piton.owned = owned;
        instances.Add(piton);
    }

    public static void HammerItIn(int id, float amount)
    {
        CleanList();

        var p = GetPiton(id);
        if (p != null)
        {
            p.GetComponent<CL_Handhold>().HammerIn(amount);
        }
    }

    public void HammerIn(float amount)
    {
        NetworkManager.SendPacket("itemPitonHammerIn", id, amount);
    }
}

[HarmonyPatch(typeof(CL_Handhold))]
public static class HandholdPatch
{
    [HarmonyPatch(typeof(CL_Handhold), nameof(CL_Handhold.HammerIn))]
    public static void Prefix(CL_Handhold __instance, float amount)
    {
        var pl = __instance.GetComponent<PitonListener>();

        if (pl && pl.owned)
        {
            pl.HammerIn(amount);
        }
    }
}


[HarmonyPatch(typeof(HandItem_Piton))]
public static class PitonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HandItem_Piton), nameof(HandItem_Piton.PitonHit))]
    public static bool PitonHit(HandItem_Piton __instance)
    {
        if (NetworkManager.CurrentState == GameState.Offline) return true;

        ref var i = ref __instance;

        if (i.canDamage && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hitInfo, i.placeDistance, i.damageMask))
        {
            Damageable component = hitInfo.collider.gameObject.GetComponent<Damageable>();
            ObjectTagger component2 = hitInfo.collider.gameObject.GetComponent<ObjectTagger>();
            if (component != null)
            {
                CL_CameraControl.Shake(0.08f);
                component.Damage(Damageable.DamageInfo.CreateDamageInfo(i.damage, "piton", new List<string>(), ENT_Player.playerObject));
                Object.Instantiate(i.damageEffect, hitInfo.point, Quaternion.identity);
                if (component2 != null && component2.HasTag("Denizen"))
                {
                    CL_GameManager.gMan.FreezeForTime(0.2f, 0.1f);
                }

                AudioManager.PlaySound(i.damageAudio, hitInfo.point);
            }
        }

        if (i.PitonRaycast())
        {
            Debug.DrawLine(Camera.main.transform.position, i.hit.point, Color.red);
            i.used = true;
            i.hand.lockHand = true;
            PitonController.SendItemPiton(i.hit.collider.transform.TransformPoint(i.hitLocal), (Quaternion.LookRotation(i.hit.normal) * Quaternion.Euler(0f, 0f, Random.Range(-30, 30))).eulerAngles, ENT_Player.playerObject.curBuffs.GetBuff("addPitonSecure"), i.item.prefabName);

            i.hand.lockHand = false;
            CL_CameraControl.Shake(0.04f);
            if (i.autograbOnPlaceIfUsing && InputManager.GetButton(i.hand.fireButton).Pressed)
            {
                i.RemoveItem();
            }
        }
        else
        {
            i.anim.SetTrigger("Fail");
        }

        return false;
    }
}