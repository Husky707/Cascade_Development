using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ClientReceiver : NetworkBehaviour, IReceiveServerCommands
{
    ClientManager _clientManager = null;
    ClientMessenger _clientMessenger => _clientManager.Messenger;

    ///////////////////////////////////////////////////
    #region Reception Events

    public event Action<uint, string, eNetRooms, eRoomRoles> E_EnteredRoom = delegate { };
    public event Action<uint> E_ExitedRoom = delegate { };
    public event Action E_RetrunToHub = delegate { };


    #endregion

    ///////////////////////////////////////////////////
    #region Init
    private void Awake()
    {
        _clientManager = GetComponent<ClientManager>();
        if(_clientManager == null)
        {
            Debug.Log("Server's 'ClientReceiver' object does no contain the required 'ClientManager' component.");
            this.enabled = false;
            return;
        }
    }
    #endregion

    ///////////////////////////////////////////////////
    #region Server side connection methods
    [Server]
    public void DisconnectClientMessenger()
    {

        _clientMessenger.DisconnectClinetMessengerFromServerReceiver();
    }

    [Server]
    public void ConnectClientMessenger(IReceiveClientCommands server)
    {

        _clientMessenger.LinkClientMessengerToServerReceiver(server);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Messeges for client

    ///////////////////////////////////////////////////
    #region NetRoom Methods
    [TargetRpc]
    public void ClientEnteredRoom(uint roomId, string name, eNetRooms roomType, eRoomRoles withRole)
    {
        E_EnteredRoom.Invoke(roomId, name, roomType, withRole);
    }

    [TargetRpc]
    public void ClientExitedRoom(uint roomid)
    {
        E_ExitedRoom.Invoke(roomid);
    }

    [TargetRpc]
    public void ReturnToHub()
    {
        E_RetrunToHub.Invoke();
    }

    #endregion

    ///////////////////////////////////////////////////
    #region Lobby Methods



    #endregion


    #endregion
}
