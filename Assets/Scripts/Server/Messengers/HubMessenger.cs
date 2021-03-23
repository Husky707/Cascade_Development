using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HubMessenger : ServerMessenger
{


    //////////////////////////////////////////////////////////////////

    [Server]
    protected override void ClientEnteredRoom(uint roomId, string name, eNetRooms roomType, eRoomRoles withRole)
    {
        _currentTarget.ClientEnteredRoom(roomId, name, roomType, withRole);
    }

    [Server]
    protected override void ClientExitedRoom(uint roomId)
    {
        _currentTarget.ClientExitedRoom(roomId);
    }

    [Server]
    protected override void ConnectClientMessenger(IReceiveClientCommands serverReceiver)
    {
        _currentTarget.ConnectClientMessenger(serverReceiver);
    }

    [Server]
    protected override void DisconnectClientMessenger()
    {
        _currentTarget.DisconnectClientMessenger();

    }
}
