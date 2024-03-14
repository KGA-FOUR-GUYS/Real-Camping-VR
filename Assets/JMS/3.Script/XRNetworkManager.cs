using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class XRNetworkManager : NetworkManager
{
    public string IPAddress = "127.0.0.1";

    public override void Start()
    {
        networkAddress = IPAddress;
        base.Start();

#if UNITY_SERVER
        StartServer();
#elif UNITY_ANDROID
        StartClient();
#endif
    }

#if UNITY_SERVER
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[{conn.address}] New client connected.");
    }
#endif
}
