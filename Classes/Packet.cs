namespace DOL.Classes;

using Steamworks;

public struct Packet
{
    public SteamId user;
    public string message;

    public string type => GetPacketType(this);
    public string firstData => GetPacketData(this);
    public string[] data => SplitPacket(this);

    public static string GetPacketType(Packet packet)
    {
        var split = SplitPacket(packet);
        return split[0];
    }

    public static string GetPacketData(Packet packet)
    {
        var split = SplitPacket(packet);
        return split[1];
    }

    public static string[] SplitPacket(Packet packet)
    {
        return packet.message.Split(':');
    }
}