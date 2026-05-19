namespace DOL.Classes;

using DOL.UI;
using Steamworks;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Player : NetworkEntity
{
    public static Player instance;

    public static TMP_Text DebugText;

    public static int NetSeed;

    public static List<string> suffixes = [string.Empty];

    public const float interval = 1f/15f;
    float timer = 10000f;

    public static void CreatePlayer()
    {
        instance = new GameObject("player").AddComponent<Player>();
        DontDestroyOnLoad(instance);

        var canvas = Builder.Canvas();
        DontDestroyOnLoad (canvas);
        DebugText = canvas.Text(Vector2.zero, new Vector2(1000, 500), "hAI.", 20, TextAlignmentOptions.TopLeft, Color.white, Vector2.one, Vector2.one, Vector2.one);

        SteamFriends.OnGameLobbyJoinRequested += (lobby, _) => NetworkManager.JoinLobby(lobby);

        instance.TakeOwnership();
    }

    public override void NetUpdate()
    {
        if (!isOwned) return;

        UpdateText("NetUpdate!");

        timer += Time.unscaledDeltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            Sync();
        }

        if (WorldLoader.instance != null && NetSeed != WorldLoader.instance.seed)
        {
            if (NetworkManager.IsHost)
            {
                NetSeed = WorldLoader.instance.seed;
                NetworkManager.SendPacket("seed", NetSeed);
            }

            if (NetworkManager.IsClient)
            {
                WorldLoader.instance.seed = NetSeed;
                CL_GameManager.ChangeState("restart");
                WorldLoader.instance.seed = NetSeed;
            }
        }
    }

    public override void OfflineUpdate()
    {
        if (!isOwned) return;

        UpdateText("OfflineUpdate!");
    }

    public override void GetPacket(Packet packet)
    {
        switch (packet.Type)
        {
            case "seed" when packet.User == NetworkManager.host?.Id:
                NetSeed = int.Parse(packet.Data[1]);

                break;

            case "playerPos":
                float x = float.Parse(packet.Data[1]);
                float y = float.Parse(packet.Data[2]);
                float z = float.Parse(packet.Data[3]);
                Vector3 bodyPos = new Vector3(x, y, z);

                x = float.Parse(packet.Data[4]);
                y = float.Parse(packet.Data[5]);
                z = float.Parse(packet.Data[6]);
                Vector3 lhPos = new Vector3(x, y, z);

                x = float.Parse(packet.Data[7]);
                y = float.Parse(packet.Data[8]);
                z = float.Parse(packet.Data[9]);
                Vector3 rhPos = new Vector3(x, y, z);

                PlayerGhost.UpdateGhost(packet.User, bodyPos, lhPos, rhPos);
                break;

            case "itemShoot":
                float x1 = float.Parse(packet.Data[1]);
                float y1 = float.Parse(packet.Data[2]);
                float z1 = float.Parse(packet.Data[3]);
                Vector3 position1 = new Vector3(x1, y1, z1);

                x1 = float.Parse(packet.Data[4]);
                y1 = float.Parse(packet.Data[5]);
                z1 = float.Parse(packet.Data[6]);
                Vector3 direction1 = new Vector3(x1, y1, z1);

                x1 = float.Parse(packet.Data[7]);
                y1 = float.Parse(packet.Data[8]);
                z1 = float.Parse(packet.Data[9]);
                Vector3 normalized1 = new Vector3(x1, y1, z1);

                string id1 = packet.Data[10];
                RebarController.SpawnItemShoot(position1, direction1, normalized1, id1);
                break;

            case "itemPiton":
                AddSuffixDebug("Piton packet!");

                float x2 = float.Parse(packet.Data[1]);
                float y2 = float.Parse(packet.Data[2]);
                float z2 = float.Parse(packet.Data[3]);
                Vector3 position2 = new Vector3(x2, y2, z2);

                x2 = float.Parse(packet.Data[4]);
                y2 = float.Parse(packet.Data[5]);
                z2 = float.Parse(packet.Data[6]);
                Vector3 direction2 = new Vector3(x2, y2, z2);

                var buff2 = float.Parse(packet.Data[7]);
                string id2 = packet.Data[8];

                AddSuffixDebug($"id {id2} pos {position2} dir {direction2} buff {buff2}");

                PitonController.SpawnItemPiton(position2, direction2, buff2, id2);
                break;
        }
    }

    public void Sync()
    {
        Vector3 bodyPos = NetworkManager.LocalPlayer.transform.position;
        var lh = NetworkManager.LocalPlayer.hands[0];
        var rh = NetworkManager.LocalPlayer.hands[1];
        Vector3 leftHandPos = lh.GetHoldWorldPosition();
        Vector3 rightHandPos = rh.GetHoldWorldPosition();
        if (lh.GetHandItem() != null) leftHandPos = lh.GetHandItem().transform.position;
        if (rh.GetHandItem() != null) rightHandPos = rh.GetHandItem().transform.position;
        float m = 100;
        bodyPos = SnapVector(bodyPos, m);
        leftHandPos = SnapVector(leftHandPos, m);
        rightHandPos = SnapVector(rightHandPos, m);
        NetworkManager.SendPacket("playerPos", bodyPos.x, bodyPos.y, bodyPos.z, leftHandPos.x, leftHandPos.y, leftHandPos.z, rightHandPos.x, rightHandPos.y, rightHandPos.z);
    }

    public void UpdateText(string prefix)
    {
        DebugText.text = $@"{prefix}
Time: {Time.unscaledTime:F2}
State: {NetworkManager.CurrentState}
IsHost: {NetworkManager.IsHost}
LocalPlayer: {(NetworkManager.LocalPlayer ? "Not null" : "Null")}
Busy: {NetworkManager.busy}
MemberCount: {(NetworkManager.Lobby.HasValue ? NetworkManager.Lobby.Value.MemberCount : "Not in lobby")}
SteamID: {NetworkManager.SteamID}
SteamName: {NetworkManager.SteamName}
HostID: {NetworkManager.host}

NetSeed: {NetSeed}
Seed: {WorldLoader.instance?.seed}
StartingSeed: {WorldLoader.instance?.startingSeed}


<alpha=#22><noparse>{string.Join("\n", suffixes)}";
    }

    /// <summary> Conditionals are compiled out by roslyn if the compiler constant isnt set </summary>
    /// <remarks> i put smt in the csproj that u just gotta uncomment btw duvi </remarks>
    [Conditional("Debug")] public static void AddSuffixDebug(string text) => AddSuffix(text);

    public static void AddSuffix(string text)
    {
        Plugin.LogInfo(text);

        if (suffixes[0] == text)
        {
            suffixes[0] = text;
        }
        else
        {
            suffixes.Insert(0, text);
            if (suffixes.Count > 5)
                suffixes.RemoveAt(4);
        }
    }

    public static Vector3 SnapVector(Vector3 pos, float m)
    {
        return new Vector3((int)(pos.x * m) / m, (int)(pos.y * m) / m, (int)(pos.z * m) / m);
    }
}
