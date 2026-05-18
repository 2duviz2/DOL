namespace DOL.Classes;

using DOL.Classes.Endpoints;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class NetworkManager
{
    public static List<Connection> connections = [];
    public static Server server = new();
    public static Client client = new();

    public static GameState CurrentState = GameState.Offline;

    public static bool busy;

    public static ENT_Player LocalPlayer;
    public static Lobby? Lobby;

    public static SteamId? _steamID;
    public static SteamId SteamID
    {
        get
        {
            _steamID ??= SteamClient.SteamId;
            return _steamID.Value;
        }
    }

    public static string _steamName;
    public static string SteamName
    {
        get
        {
            _steamName ??= SteamClient.Name;
            return _steamName;
        }
    }

    public static bool IsHost => CurrentState == GameState.Host;
    public static bool IsClient => CurrentState == GameState.Client;

    public static void CreateLobby()
    {
        if (Lobby.HasValue) LeaveLobby();
        if (busy) return;

        busy = true;

        Plugin.instance.StartCoroutine(CreateLobbyCoroutine());
    }

    public static IEnumerator CreateLobbyCoroutine()
    {
        Task<Lobby?> createAsync = SteamMatchmaking.CreateLobbyAsync(16);
        yield return createAsync.AsCoroutine();
        Lobby = createAsync.Result;
        busy = false;

        Lobby.Value.SetJoinable(true);
        Lobby.Value.SetFriendsOnly();
        Lobby.Value.SetData("version", PluginInfo.Version);
        Lobby.Value.SetData("client", PluginInfo.Name);
        Lobby.Value.SetData("seed", "0");

        CurrentState = GameState.Host;

        Player.AddSufix("connected to steam lobby");

        server.Open();
    }

    public static void JoinCopiedLobby()
    {
        Player.DebugText.text = "Joining canceled";
        if (busy || CurrentState != GameState.Offline) return;
        if (!ulong.TryParse(GUIUtility.systemCopyBuffer, out var lobbyID))
        {
            Player.DebugText.text = "Invalid lobby code";
            return;
        }
        var lobby = new Lobby(lobbyID);
        JoinLobby(lobby);
    }

    public static void JoinLobby(Lobby TargetLobby)
    {
        Player.DebugText.text = "Joining cancelled";

        if (busy || CurrentState != GameState.Offline) return;

        Player.DebugText.text = "Joining...";

        busy = true;
        
        Plugin.instance.StartCoroutine(JoinLobbyCoroutine(TargetLobby));
    }

    public static IEnumerator JoinLobbyCoroutine(Lobby TargetLobby)
    {
        Task<RoomEnter> joinAsync = TargetLobby.Join();
        yield return joinAsync.AsCoroutine();

        if (joinAsync.Result == RoomEnter.Success && TargetLobby.GetData("client") == PluginInfo.Name)
        {
            Player.DebugText.text = $"Join successful ({joinAsync.Result})";
            busy = false;
            Lobby = TargetLobby;
            CurrentState = GameState.Client;

            client.Connect(Lobby.Value.Owner.Id);
        }
        else
        {
            Player.DebugText.text = $"Join error ({joinAsync.Result})";
            CurrentState = GameState.Offline;
            busy = false;
        }
    }

    public static void LeaveLobby()
    {
        Lobby?.Leave();
        Lobby = null;
        server.Close();
        client.Disconnect();
        CurrentState = GameState.Offline;
    }

    public static void HandlePacket(Friend user, string message)
    {
        if (CurrentState == GameState.Offline) return;
        if (message == null) return;

        //Client.AddSufix($"P {user.Name}:{message}");

        var p = new Packet
        {
            user = user.Id,
            message = message,
        };

        if (p.type != "playerPos")
        {
            Player.AddSufix($"{p.message}");
        }

        NetworkEntity.SendGlobalPacket(p);
    }

    public static void SendPacket(params object[] data)
    {
        if (CurrentState == GameState.Offline) return;
        if (Lobby == null) return;

        var args = string.Join(":", data);

        if (data[0].ToString() != "playerPos")
        {
            Player.AddSufix($"S {args}");
        }
        Redirect(args);
    }

    public static void Redirect(string msg, Connection? avoid = null)
    {
        foreach (Connection con in connections)
        {
            if (con != avoid)
                con.SendMessage(msg);
        }
    }
}

public enum GameState
{
    Offline,
    Host,
    Client
}