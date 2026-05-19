namespace DOL.Classes.Endpoints;

using Steamworks;
using Steamworks.Data;
using System.Text;

public class Client : IConnectionManager
{
    public ConnectionManager manager;

    public void Connect(SteamId hostId)
    {
        Player.AddSuffix("Client connecting to server... (" + new Friend(hostId).Name + ")");
        manager = SteamNetworkingSockets.ConnectRelay(hostId, Server.virtualport, this);
    }

    public void Disconnect()
    {
        Player.AddSuffix("Client disconnecting from server...");
        manager?.Close();
        manager = null;

        NetworkManager.connections.Clear();
    }

    public void OnConnecting(ConnectionInfo info)
    {
        Player.AddSuffix("Client connecting to server... (" + new Friend(info.Identity.SteamId).Name + ")");
    }

    public void OnConnected(ConnectionInfo info)
    {
        Player.AddSuffix("Client connected");
        manager.Connection.UserData = unchecked((long)info.Identity.SteamId.Value);
        NetworkManager.connections = [manager.Connection];
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        Player.AddSuffix("Client disconnected");
        NetworkManager.connections.Clear();
    }

    public unsafe void OnMessage(nint data, int size, long messageNum, long recvTime, int channel)
    {
        string packet = Encoding.UTF8.GetString((byte*)data, size);
        Friend player = new(unchecked((ulong)manager.Connection.UserData));
        Player.AddSuffixDebug("Client received packet from(" + player.Name + "), packet: " + packet);
        NetworkManager.HandlePacket(player, packet);
    }
}
