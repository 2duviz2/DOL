namespace DOL.Classes.Endpoints;

using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Text;

public class Server : ISocketManager
{
    public const int virtualport = 0x00F9C6;
    public SocketManager manager;

    public void Open()
    {
        Player.AddSufix("Opening server...");
        manager = SteamNetworkingSockets.CreateRelaySocket(virtualport, this);
        Player.AddSufix("Opened server...");
    }

    public void Close()
    {
        Player.AddSufix("Closing server...");
        manager?.Close();
        manager = null;

        NetworkManager.connections.Clear();
        Player.AddSufix("Closed server");
    }

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Player.AddSufix("User is connecting...");
        connection.Accept();
        connection.UserData = unchecked((long)info.Identity.SteamId.Value);
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        // this is where duviz will need to add a level join packet thingamajig
        Player.AddSufix("User connected " + new Friend(info.Identity.SteamId).Name);
        NetworkManager.connections.Add(connection);
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Player.AddSufix("User disconnected " + new Friend(info.Identity.SteamId).Name);
        NetworkManager.connections.Remove(connection);
    }

    public unsafe void OnMessage(Connection connection, NetIdentity identity, nint data, int size, long messageNum, long recvTime, int channel)
    {
        string packet = Encoding.UTF8.GetString((byte*)data, size);
        Friend player = new(unchecked((ulong)connection));
        Player.AddSufix("Server received packet from(" + player.Name + "), paclet: " + packet);
        NetworkManager.HandlePacket(player, packet);

        NetworkManager.Redirect(packet, connection); // redirect to all other clients teehee >w<
    }
}