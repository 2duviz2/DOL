namespace DOL.Classes;

using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    public Vector3Lerp bodyPos;

    public void Update()
    {
        transform.position = bodyPos.Grab();
    }

    public static Dictionary<SteamId, GameObject> ghosts = [];

    public static void CreateGhost(SteamId id)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        g.AddComponent<PlayerGhost>();
        ghosts.Add(id, g);
    }

    public static void UpdateGhost(SteamId id, Vector3 pos)
    {
        if (!ghosts.ContainsKey(id))
        {
            CreateGhost(id);
        }

        if (ghosts[id] == null)
        {
            ghosts.Remove(id);
            CreateGhost(id);
        }

        ghosts[id].GetComponent<PlayerGhost>().bodyPos.Set(pos);
    }
}
