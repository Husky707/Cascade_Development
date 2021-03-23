using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(ServerMessenger))]
[RequireComponent(typeof(ServerReceiver))]
public class ServerManager : NetworkManager
{

    ///////////////////////////////////////////////////
    private ServerReceiver _receiver = null;
    private ServerMessenger _messenger = null;

    private HubRoom _hub = null;
    [Header("Hub Settings")]
    [SerializeField] NetworkRoomData HubData;

    ///////////////////////////////////////////////////
    #region Init
    public override void Awake()
    {
        base.Awake();

        _receiver = GetComponent<ServerReceiver>();
        _messenger = GetComponent<ServerMessenger>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Debug.Log("This connection is a client, destroying server scripts.");
        _receiver.enabled = false;
        _messenger.enabled = false;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        SetupHub();
    }

    #endregion

    ///////////////////////////////////////////////////
    #region Handle Connections
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        //Spawn player
        GameObject newPlayer = SpawnPlayer(conn);
        NetworkServer.AddPlayerForConnection(conn, newPlayer);

        //Add player to Hub
        _hub.AddObserver(conn);

    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Removes default implementation.
        //base.OnServerAddPlayer(conn);
    }

    [Server]
    private GameObject SpawnPlayer(NetworkConnection conn)
    {
        GameObject newPlayer = Instantiate(playerPrefab);

        Debug.Log("Spawned a new player!");
        return newPlayer;
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        _hub.OnObserverDisconnected(conn);

    }

    #endregion


    ///////////////////////////////////////////////////
    #region Setup Server Hub

    void SetupHub()
    {
        maxConnections = (int)HubData.OccupancySetting.MaxObservers;
        _hub = new HubRoom(0, HubData, _messenger, _receiver);

        Debug.Log("Server hub ready!");
    }


    #endregion
}
