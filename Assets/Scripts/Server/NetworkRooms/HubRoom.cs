using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubRoom : NetworkRoom
{

    public HubRoom(uint id, NetworkRoomData data, ServerMessenger messenger, ServerReceiver receiver)
        : base(id, data, messenger, receiver) 
    {
        AddEventListeners();
    }

    List<NetworkRoom> GameRooms = new List<NetworkRoom>();
    List<NetworkRoom> LobbyRooms = new List<NetworkRoom>();

    List<NetworkIdentity> ExpectedDisconnects = new List<NetworkIdentity>();

    NetworkRoomData LobbyRoom;
    NetworkRoomData GameRoom;

    ///////////////////////////////////////////////////
    //Event Listeners



    ///////////////////////////////////////////////////
    #region Overrides

    public override bool OnObserverDisconnected(NetworkConnection conn)
    {
        return OnObserverDisconnected(conn.connectionId);
    }

    public override bool OnObserverDisconnected(int id)
    {
        //Observerv only in hub
        if (base.OnObserverDisconnected(id))
            return true;

        //Expected disconnect - remove from waitlist
        NetworkIdentity identity = Observers[id].identity;
        if (ExpectedDisconnects.Remove(identity))
            return true;

        //Unexpected disconnects - notify active rooms
        bool observerFound = false;
        foreach (NetworkRoom room in GameRooms)
        {
            observerFound = room.OnObserverDisconnected(id);
            if (observerFound)
                return true;
        }

        observerFound = false;
        foreach (NetworkRoom room in LobbyRooms)
        {
            observerFound = room.OnObserverDisconnected(id);
            if (observerFound)
                return true;
        }

        return observerFound;
    }

    protected override void Close()
    {
        RemoveEventListeners();
        base.Close();
    }

    #endregion

    ///////////////////////////////////////////////////
    #region Listen For Requests

    private void AddEventListeners()
    {
        _receiver.E_PlayRequested += OnPlayRequested;
        _receiver.E_QuitAppRequested += OnQuitApplicationRequested;
    }

    private void RemoveEventListeners()
    {
        _receiver.E_PlayRequested -= OnPlayRequested;
        _receiver.E_QuitAppRequested -= OnQuitApplicationRequested;
    }

    #endregion

    ///////////////////////////////////////////////////
    #region Handle incoming client requests

    private void OnQuitApplicationRequested(NetworkIdentity id)
    {
        ExpectedDisconnects.Add(id);
        RemoveObserver(id.connectionToClient.connectionId);
        id.connectionToClient.Disconnect();
    }

    private void OnPlayRequested(NetworkIdentity id, eGameTypes type)
    {
        //TODO: Replace with allowed play types



        //Find or create Lobby for game
        
    }


    #endregion

}
