namespace DOL.Classes;

using Steamworks;

public struct Packet(SteamId user, string message)
{
    public SteamId User = user;
    public string Message = message;
    public string[] Data = message.Split(':');
    public string Type => Data[0];

    public override string ToString() =>
        new Friend(User) + " :: " + Message;
}