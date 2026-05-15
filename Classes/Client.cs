global using static DOL.Classes.Client;

namespace DOL.Classes;

using DOL.UI;
using Steamworks;
using TMPro;
using UnityEngine;

public class Client : NetworkEntity
{
    public static Client instance;

    public static TMP_Text DebugText;

    public static int NetSeed;

    static string sufix = "\n";

    public const float interval = 1f/20f;
    float timer = 10000f;

    public static void CreateClient()
    {
        instance = new GameObject().AddComponent<Client>();
        DontDestroyOnLoad(instance);

        var canvas = Builder.Canvas();
        DontDestroyOnLoad (canvas);
        DebugText = canvas.Text(Vector2.zero, new Vector2(1000, 500), "hAI.", 20, TextAlignmentOptions.TopLeft, Color.white, Vector2.one, Vector2.one, Vector2.one);

        SteamFriends.OnGameLobbyJoinRequested += (lobby, _) => NetworkManager.JoinLobby(lobby);

        SteamMatchmaking.OnChatMessage += (lobby, user, message) =>
        {
            NetworkManager.HandlePacket(lobby, user, message);
        };

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

        if (NetworkManager.IsHost)
        {
            if (WorldLoader.instance != null)
            {
                if (NetSeed != WorldLoader.instance.seed)
                {
                    NetSeed = WorldLoader.instance.seed;
                    NetworkManager.Lobby.Value.SetData("seed", NetSeed.ToString());
                }
            }
        }

        if (NetworkManager.IsClient)
        {
            if (WorldLoader.instance != null)
            {
                var lobbySeed = NetworkManager.Lobby.Value.GetData("seed");
                if (NetworkManager.Lobby.HasValue)
                {
                    if (int.TryParse(lobbySeed, out var newSeed) && newSeed != NetSeed && WorldLoader.instance.seed != newSeed)
                    {
                        NetSeed = newSeed;
                        WorldLoader.instance.seed = newSeed;
                        CL_GameManager.ChangeState("restart");
                    }
                }
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
        if (packet.user == NetworkManager.SteamID) return;

        if (packet.type == "playerPos")
        {
            float x = float.Parse(packet.data[1]);
            float y = float.Parse(packet.data[2]);
            float z = float.Parse(packet.data[3]);
            Vector3 bodyPos = new Vector3(x, y, z);

            x = float.Parse(packet.data[4]);
            y = float.Parse(packet.data[5]);
            z = float.Parse(packet.data[6]);
            Vector3 lhPos = new Vector3(x, y, z);

            x = float.Parse(packet.data[7]);
            y = float.Parse(packet.data[8]);
            z = float.Parse(packet.data[9]);
            Vector3 rhPos = new Vector3(x, y, z);

            PlayerGhost.UpdateGhost(packet.user, bodyPos, lhPos, rhPos);
        }

        if (packet.type == "itemShoot")
        {
            float x = float.Parse(packet.data[1]);
            float y = float.Parse(packet.data[2]);
            float z = float.Parse(packet.data[3]);
            Vector3 position = new Vector3(x, y, z);

            x = float.Parse(packet.data[4]);
            y = float.Parse(packet.data[5]);
            z = float.Parse(packet.data[6]);
            Vector3 direction = new Vector3(x, y, z);

            x = float.Parse(packet.data[7]);
            y = float.Parse(packet.data[8]);
            z = float.Parse(packet.data[9]);
            Vector3 normalized = new Vector3(x, y, z);

            string id = packet.data[10];

            RebarController.SpawnItemShoot(position, direction, normalized, id);
        }
    }

    public void Sync()
    {
        Vector3 bodyPos = NetworkManager.LocalPlayer.transform.position;
        var lh = NetworkManager.LocalPlayer.hands[0];
        var rh = NetworkManager.LocalPlayer.hands[1];
        Vector3 leftHandPos = lh.GetHoldWorldPosition();
        Vector3 rightHandPos = rh.GetHoldWorldPosition();
        if (lh.GetHandItem() != null)
            leftHandPos = lh.GetHandItem().transform.position;
        if (rh.GetHandItem() != null)
            rightHandPos = rh.GetHandItem().transform.position;
        float m = 100;
        bodyPos = SnapVector(bodyPos, m);
        leftHandPos = SnapVector(leftHandPos, m);
        rightHandPos = SnapVector(rightHandPos, m);
        NetworkManager.SendPacket("playerPos", bodyPos.x, bodyPos.y, bodyPos.z, leftHandPos.x, leftHandPos.y, leftHandPos.z, rightHandPos.x, rightHandPos.y, rightHandPos.z);
    }

    public void UpdateText(string prefix)
    {
        DebugText.text = $"{prefix}\n" +
            $"Time: {Time.unscaledTime:F2}\n" +
            $"State: {NetworkManager.CurrentState}\n" +
            $"IsHost: {NetworkManager.IsHost}\n" +
            $"LocalPlayer: {(NetworkManager.LocalPlayer ? "Not null" : "Null")}\n" +
            $"Busy: {NetworkManager.busy}\n" +
            $"MemberCount: {(NetworkManager.Lobby.HasValue ? NetworkManager.Lobby.Value.MemberCount : "Not in lobby")}\n" +
            $"SteamID: {NetworkManager.SteamID}\n" +
            $"SteamName: {NetworkManager.SteamName}\n" +
            $"\n" +
            $"NetSeed: {NetSeed}\n" +
            $"Seed: {WorldLoader.instance?.seed}\n" +
            $"StartingSeed: {WorldLoader.instance?.startingSeed}\n\n<color=#ffffff22>" + sufix;
    }

    public static void AddSufix(string text)
    {
        //Plugin.LogInfo(text);
        sufix = text + "\n" + sufix;
        var split = sufix.Split('\n');
        if (split.Length > 5)
        {
            sufix = string.Join("\n", split, 0, 5);
        }
    }

    public static Vector3 SnapVector(Vector3 pos, float m)
    {
        return new Vector3((int)(pos.x * m) / m, (int)(pos.y * m) / m, (int)(pos.z * m) / m);
    }
}
