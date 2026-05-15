namespace DOL.Classes;

using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour, ISocketManager
{
    public static Server Instance { get; private set; }

    public List<Connection> Connections = [];

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        InvokeRepeating(nameof(Loop), 0f, interval);
    }

    public void Loop()
    {

    }

    public void Close()
    {
        Connections.Clear();
        CancelInvoke(nameof(Loop));
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        Connections.Add(connection);
    }

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {

    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Connections.Remove(connection);
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {

    }
}