namespace DOL.Classes;

using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    public Vector3Lerp bodyPos, lhPos, rhPos;

    public GameObject LeftHand, RightHand;

    public void Awake()
    {
        LeftHand = CreateHand();
        RightHand = CreateHand();
    }

    public void Update()
    {
        transform.position = bodyPos.Grab();
        LeftHand.transform.position = lhPos.Grab();
        RightHand.transform.position = rhPos.Grab();
    }

    public static Dictionary<SteamId, GameObject> ghosts = [];

    public static void CreateGhost(SteamId id)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        g.GetComponent<Renderer>().material = UnlitMat;
        g.GetComponent<Collider>().enabled = false;
        g.AddComponent<PlayerGhost>();
        ghosts.Add(id, g);
    }

    public static void UpdateGhost(SteamId id, Vector3 bodyPos, Vector3 leftHandPos, Vector3 rightHandPos)
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

        var ph = ghosts[id].GetComponent<PlayerGhost>();

        ph.bodyPos.Set(bodyPos);
        ph.lhPos.Set(leftHandPos);
        ph.rhPos.Set(rightHandPos);
    }

    public static Material UnlitMat
    {
        get
        {
            if (!field)
                field = new Material(Shader.Find("Unlit/Color"))
                {
                    color = new Color(1, 0, 0, 1)
                };

            return field;
        }
    }

    public static GameObject CreateHand()
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.localScale = Vector3.one * 0.2f;
        g.GetComponent<Renderer>().material = UnlitMat;
        g.GetComponent<Collider>().enabled = false;
        return g;
    }
}
