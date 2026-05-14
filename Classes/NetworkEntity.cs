namespace DOL.Classes;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkEntity : MonoBehaviour
{
    public static List<NetworkEntity> entities = [];

    public bool isOwned = false;

    public void Update()
    {
        if (NetworkManager.CurrentState == GameState.Offline)
        {
            OfflineUpdate();
        }
        else
        {
            NetUpdate();
        }
    }

    public void TakeOwnership() => isOwned = true;

    public void Awake()
    {
        entities.Add(this);
    }

    public void OnDestroy()
    {
        entities.Remove(this);
    }

    public virtual void NetUpdate()
    {

    }

    public virtual void OfflineUpdate()
    {

    }

    public virtual void GetPacket(Packet packet)
    {

    }

    public static void SendGlobalPacket(Packet packet)
    {
        foreach (var entity in entities.ToList())
        {
            if (entity == null)
            {
                entities.Remove(entity);
                continue;
            }

            if (entity.isOwned)
            {
                entity.SendPacket(packet);
            }
        }
    }

    public void SendPacket(Packet packet)
    {
        GetPacket(packet);
    }
}
