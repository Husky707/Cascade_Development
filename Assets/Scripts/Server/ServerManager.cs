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

    ///////////////////////////////////////////////////
    public override void OnStartServer()
    {
        base.OnStartServer();

        //Create ServerHub
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        //Spawn player
        GameObject newPlayer = SpawnPlayer(conn);
        NetworkServer.AddPlayerForConnection(conn, newPlayer);

        //Add player to Hub
        //Register Client
        Debug.Log("Attempting registry.");
        ClientManager client = newPlayer.GetComponent<ClientManager>();
        if(client == null )
        {
            Debug.Log("Registry attempt failed. No ClientManager found on player prefab.");
        }
        else
            client.Messenger.LinkClientMessengerToServerReceiver(_receiver);
    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Remove default implementation.
        //base.OnServerAddPlayer(conn);
    }

    [Server]
    private GameObject SpawnPlayer(NetworkConnection conn)
    {
        GameObject newPlayer = Instantiate(playerPrefab);

        Debug.Log("Spawned a new player!");
        return newPlayer;
    }

}
