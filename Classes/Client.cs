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

    public static void CreateClient()
    {
        instance = new GameObject().AddComponent<Client>();
        DontDestroyOnLoad(instance);

        var canvas = Builder.Canvas();
        DontDestroyOnLoad (canvas);
        DebugText = canvas.Text(Vector2.zero, new Vector2(500, 500), "hAI.", 20, TextAlignmentOptions.TopLeft, Color.white, Vector2.one, Vector2.one, Vector2.one);

        SteamFriends.OnGameLobbyJoinRequested += (lobby, _) => NetworkManager.JoinLobby(lobby);
    }

    public override void NetUpdate()
    {
        UpdateText("NetUpdate!");

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

    public override void OfflineUpdate()
    {
        UpdateText("OfflineUpdate!");
    }

    public void UpdateText(string prefix)
    {
        DebugText.text = $"{prefix}\n" +
            $"Time: {Time.unscaledTime:F2}\n" +
            $"State: {NetworkManager.CurrentState}\n" +
            $"IsHost: {NetworkManager.IsHost}\n" +
            $"Busy: {NetworkManager.busy}\n" +
            $"MemberCount: {(NetworkManager.Lobby.HasValue ? NetworkManager.Lobby.Value.MemberCount : "Not in lobby")}\n" +
            $"SteamID: {NetworkManager.SteamID}\n" +
            $"SteamName: {NetworkManager.SteamName}\n" +
            $"\n" +
            $"NetSeed: {NetSeed}\n" +
            $"Seed: {WorldLoader.instance?.seed}\n" +
            $"StartingSeed: {WorldLoader.instance?.startingSeed}\n";
    }
}
