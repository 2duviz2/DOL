namespace DOL.Classes;

using Steamworks;
using Steamworks.Data;
using UnityEngine;

public static class NetworkManager
{
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

    public static string? _steamName;
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

        CreateLobbyAsync();
    }

    public static async void CreateLobbyAsync()
    {
        await SteamMatchmaking.CreateLobbyAsync(16).ContinueWith(t =>
        {
            busy = false;
            Lobby = t.Result;

            Lobby?.SetJoinable(true);
            Lobby?.SetFriendsOnly();
            Lobby?.SetData("version", PluginInfo.Version);
            Lobby?.SetData("client", PluginInfo.Name);
            Lobby?.SetData("seed", "0");

            CurrentState = GameState.Host;
        });
    }

    public static void JoinCopiedLobby()
    {
        Client.DebugText.text = "Joining canceled";
        if (busy || CurrentState != GameState.Offline) return;
        if (!ulong.TryParse(GUIUtility.systemCopyBuffer, out var lobbyID))
        {
            Client.DebugText.text = "Invalid lobby code";
            return;
        }
        var lobby = new Lobby(lobbyID);
        JoinLobby(lobby);
    }

    public static void JoinLobby(Lobby TargetLobby)
    {
        Client.DebugText.text = "Joining canceled";

        if (busy || CurrentState != GameState.Offline) return;

        Client.DebugText.text = "Joining...";

        busy = true;
        
        JoinLobbyAsync(TargetLobby);
    }

    public static async void JoinLobbyAsync(Lobby TargetLobby)
    {
        await TargetLobby.Join().ContinueWith((t) =>
        {
            if (t.IsCompletedSuccessfully && t.Result == RoomEnter.Success && TargetLobby.GetData("client") == PluginInfo.Name)
            {
                Client.DebugText.text = $"Join successful ({t.Result}/{t.IsCompletedSuccessfully})";
                busy = false;
                Lobby = TargetLobby;
                CurrentState = GameState.Client;
            }
            else
            {
                Client.DebugText.text = $"Join error ({t.Result}/{t.IsCompletedSuccessfully})";
                CurrentState = GameState.Offline;
                busy = false;
            }
        });
    }

    public static void LeaveLobby()
    {
        Lobby?.Leave();
        Lobby = null;
        CurrentState = GameState.Offline;
    }
}

public enum GameState
{
    Offline,
    Host,
    Client
}