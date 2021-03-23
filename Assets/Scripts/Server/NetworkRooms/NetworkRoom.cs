using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class NetworkRoom
{
    public static event Action<NetworkRoom> RoomClosed = delegate { };
    public static event Action<NetworkRoom> RoomOpened = delegate { };

    //int: ID created by Mirror for each NetConn
    protected Dictionary<int, NetworkConnection> Observers = new Dictionary<int, NetworkConnection>();
    public int NumObservers => Observers.Count;

    public uint RoomId => _roomId;
    protected uint _roomId = 0;


    public NetworkRoomData RoomData;
    public string Name => RoomData.Name;
    public eNetRooms Type => RoomData.Type;

    protected NetRoomSettings ObserverSettings => RoomData.OccupancySetting;

    public bool RoomIsActive => IsRoomActive();
    protected bool _roomIsActive = false;
    public bool IsEmpty => Observers.Count <= 0;

    protected ServerMessenger _messenger = null;
    protected ServerReceiver _receiver = null;

    ///////////////////////////////////////////////////////////////////////////////////
    #region Init

    public NetworkRoom(uint id, NetworkRoomData data, ServerMessenger messenger, ServerReceiver receiver)
    {
        _roomId = id;

        RoomData = data;

        _messenger = messenger;
        _receiver = receiver;

        _roomIsActive = true;
        RoomOpened.Invoke(this);
    }

    protected virtual void Close()
    {
        RemoveObservers();
        RoomClosed.Invoke(this);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////
    #region Observer Data
    [Server]
    public bool IsRoomActive()
    {
        if (_roomIsActive == false)
            return false;

        return true;
    }

    [Server]
    public bool HasObserver(int target)
    {
        if (Observers.ContainsKey(target))
            return true;

        return false;
    }

    [Server]
    public bool HasObserver(NetworkConnection conn)
    {
        foreach (int id in Observers.Keys)
            if (Observers[id] == conn)
                return true;

        return false;
    }

    [Server]
    public bool HasObserver(NetworkIdentity identity)
    {
        return HasObserver(identity.connectionToClient.connectionId);
    }

    [Server]
    protected bool IsReceivingObservers()
    {
        if (Observers.Count >= ObserverSettings.MaxObservers)
            return false;
        if (!RoomIsActive)
            return false;

        return true;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////
    #region Communication

    [Server]
    public ClientReceiver GetClient(int targetid)
    {
        if (Observers == null || !Observers.ContainsKey(targetid))
            return null;

        return Observers[targetid].identity.GetComponent<ClientReceiver>();
    }

    [Server]
    protected virtual bool HasReceiver(NetworkIdentity identity)
    {
        if (identity == null)
            return false;

        int id = identity.connectionToClient.connectionId;
        if (!HasObserver(id))
        {
            Debug.Log("Room does not have a receiver for ID: " + id.ToString());
            HandleUnknownMessager(identity);
            return false;
        }

        return true;
    }

    [Server]
    protected virtual void HandleUnknownMessager(NetworkIdentity identity)
    {
        Debug.Log("TODO: Notify Hub of missing player message. Find player. DO stuff");
    }


    #endregion

    ///////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////
    #region Add/Remove Observers
    [Server]
    private void ConnectObserverMessenger(int id, ClientReceiver player = null)
    {
        if (Observers == null)
            return;

        if (!Observers.ContainsKey(id))
        {
            Debug.Log("Trying to connect a client to a room it is not observering");
            return;
        }

        if (player == null)
            player = GetClient(id);

        if (player == null)
        {
            Debug.Log("Trouble finding client receiver over the network. Cannot link messenger to room " + Name);
            return;
        }

        _messenger.BeginMessage(player);
        _messenger.ConnectClientMessenger(player, _receiver);
        _messenger.EndMessage(player);

        Debug.Log("Client " + id + " is now linked to room " + Name);
    }

    [Server]
    protected void DisconnectObserverMessenger(int id)
    {
        if (Observers == null)
            return;

        if (!Observers.ContainsKey(id))
        {
            Debug.Log("Trying to disconnect a client from a room it is not observering");
            return;
        }

        ClientReceiver player = GetClient(id);
        if (player == null)
        {
            Debug.Log("Trouble finding player messenger over the network. Cannot disconnect player messenger from room " + Name);
            return;
        }

        _messenger.BeginMessage(player);
        _messenger.DisconnectClientMessenger(player);
        _messenger.EndMessage(player);
    }

    [Server]
    public bool AddObserver(NetworkConnection conn)
    {

        return IsReceivingObservers() && OnAddObserver(conn);
    }

    [Server]
    private bool OnAddObserver(NetworkConnection conn)
    {
        if (conn == null)
            return false;

        int id = conn.connectionId;
        if (Observers.ContainsKey(id))
        {
            UnityEngine.Debug.Log("Room already contains player with id " + id.ToString());
            return false;
        }

        Observers.Add(id, conn);

        ClientReceiver clientReceiver = GetClient(id);
        _messenger.BeginMessage(clientReceiver);

        ConnectObserverMessenger(id, clientReceiver);
        _messenger.ClientEnteredRoom(clientReceiver, RoomId, Name, Type, eRoomRoles.Observer);

        _messenger.EndMessage(clientReceiver);

        return true;
    }

    [Server]
    public virtual bool RemoveObserver(int connId)
    {
        if (!Observers.ContainsKey(connId))
            return false;

        ClientReceiver client = GetClient(connId);
        if(client == null)
        {
            Debug.Log("Client no longer exists, cannot send room removal messages.");
        }
        else
        {
            _messenger.BeginMessage(client);

            _messenger.ClientExitedRoom(client, RoomId);
            _messenger.DisconnectClientMessenger(client);

            _messenger.EndMessage(client);
        }

        return Observers.Remove(connId);
    }

    [Server]
    public virtual int[] RemoveObservers()
    {
        int[] removed = new int[Observers.Count];
        int i = 0;
        foreach (int id in Observers.Keys)
        {
            removed[i] = id;
            i++;
        }

        Observers.Clear();

        return removed;
    }

    [Server]
    public virtual bool OnObserverDisconnected(NetworkConnection conn)
    {

        return Observers.Remove(conn.connectionId);
    }

    [Server]
    public virtual bool OnObserverDisconnected(int id)
    {
        return Observers.Remove(id);
    }

    #endregion
}

[Serializable]
public struct NetRoomSettings
{
    public NetRoomSettings(uint maxObservers, uint maxActors, uint maxSpectators)
    {
        _maxObservers = maxObservers;
        _maxActors = maxActors;
        _maxSpectators = maxSpectators;
    }

    public uint MaxObservers => _maxObservers;
    [SerializeField] uint _maxObservers;

    public uint MaxActors => _maxActors;
    [SerializeField] uint _maxActors;
    public uint MaxSpectators => _maxSpectators;
    [SerializeField] uint _maxSpectators;

}


[Serializable]
public struct NetworkRoomData
{
    public string Name => _name;
    [SerializeField] string _name;
    public eNetRooms Type => _type;
    [SerializeField] eNetRooms _type;

    public NetworkRoomSetupInstructions RoomSetup => _roomSetup;
    [SerializeField] NetworkRoomSetupInstructions _roomSetup;

    public NetRoomSettings OccupancySetting => _occupancySettings;
    [SerializeField] NetRoomSettings _occupancySettings;

}


[Serializable]
public struct NetworkRoomSetupInstructions
{
    public eScenes SceneToLoad => _sceneToLoad;
    [SerializeField] eScenes _sceneToLoad;

    public float TimeToCloseRoom => _timeToCloseRoom;
    [SerializeField] float _timeToCloseRoom;

    public Animator ExitAnimation => _exitAnimation;
    [SerializeField] Animator _exitAnimation;

    public Animator EntryAnimation => _entryAnimation;
    [SerializeField] Animator _entryAnimation;
}