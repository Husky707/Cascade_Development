using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerMessenger : NetworkBehaviour, ISendClientCommands
{

    protected ClientReceiver _currentTarget = null;
    protected bool _messengerReady = false;
    int _messageDepth = 0;

    ///////////////////////////////////////////////////////////////////////////////////
    #region Messenger Functionality
    public bool BeginMessage(ClientReceiver target)
    {
        if (_currentTarget == target)
        {
            _messageDepth++;
            return true;
        }

        if(_currentTarget != null)
        {
            EndMessage(_currentTarget);
        }

        _messageDepth = 1;
        _currentTarget = target;
        _messengerReady = true;
        return true;
    }

    public virtual void EndMessage(ClientReceiver target)
    {
        if(_currentTarget != target)
        {
            Debug.Log("Cannot end messege: ServerMessenger is messeging a different client.");
            return;
        }

        if(_messageDepth > 1)
        {
            //Multiple nested requests have been made. Close last request
            _messageDepth--;
            return;
        }
        else if(_messageDepth <= 1)
        {
            _messengerReady = false;
            _currentTarget = null;
            _messageDepth = 0;
        }

    }

    public virtual void EndAllCurrentMesseges()
    {
        _messengerReady = false;
        _currentTarget = null;
        _messageDepth = 0;
    }

    private bool MessengerReady()
    {
        if (!_messengerReady || _currentTarget == null)
            return false;

        return true;
    }

    public bool MessengerReady(ClientReceiver desiredTarget)
    {
        if(desiredTarget != _currentTarget)
        {
            Debug.Log("Messenger cannot send messege: Desired target does not match current target");
            return false;
        }

        if(!MessengerReady())
        {
            Debug.Log("Messenger cannot send messege: Messenger not ready.");
            return false;
        }

        return true;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////
    #region Sendable Messeges

    [Server]
    public void ClientEnteredRoom(ClientReceiver target, uint roomId, string name, eNetRooms roomType, eRoomRoles withRole)
    {
        if (MessengerReady(target))
            ClientEnteredRoom(roomId, name, roomType, withRole);
    }

    [Server]
    public void ClientExitedRoom(ClientReceiver target, uint roomId)
    {
        if (MessengerReady(target))
            ClientExitedRoom(roomId);
    }

    [Server]
    public void ConnectClientMessenger(ClientReceiver target, IReceiveClientCommands serverReceiver)
    {
        if (MessengerReady(target))
            ConnectClientMessenger(serverReceiver);
    }

    [Server]
    public void DisconnectClientMessenger(ClientReceiver target)
    {
        if (MessengerReady(target))
            DisconnectClientMessenger();
    }

    [Server]
    public void ReturnToHub(ClientReceiver target)
    {
        if (MessengerReady(target))
            ReturnToHub();
    }

    #endregion

    //////////////////////////////////////////////////////////////////
    #region Sendable Messege Implementations

    [Server]
    protected virtual void ReturnToHub()
    {
        Debug.Log("Current ServerMessenger does not implement 'ReturnToHub'.");

    }

    [Server]
    protected virtual void ClientEnteredRoom(uint roomId, string name, eNetRooms roomType, eRoomRoles withRole)
    {
        Debug.Log("Current ServerMessenger does not implement 'ClientEnteredRoom'.");
    }

    [Server]
    protected virtual void ClientExitedRoom(uint roomId)
    {
        Debug.Log("Current ServerMessenger does not implement 'ClientExitedRoom'.");
    }

    [Server]
    protected virtual void ConnectClientMessenger(IReceiveClientCommands serverReceiver)
    {
        Debug.Log("Current ServerMessenger does not implement 'ConnectMessengerToServer'.");
    }

    [Server]
    protected virtual void DisconnectClientMessenger()
    {
        Debug.Log("Current ServerMessenger does not implement 'DisconnectClientMessenger'.");

    }

    #endregion

    /////////////////////////////////
}
