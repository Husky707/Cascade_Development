using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientMessenger : NetworkBehaviour
{
    public bool showDebug = false;
    public IReceiveClientCommands ServerLink { get { if (!isServer) return null; return _serverLink; } }
    private IReceiveClientCommands _serverLink = null;


    ///////////////////////////////////////////////////
    ///Init

    [Server]
    public void LinkClientMessengerToServerReceiver(IReceiveClientCommands netComm)
    {
        if (netComm == null)
        {
            Debug.Log("Could not link client messenger: Null NetworkCommunicator.");
            return;
        }

        _serverLink = netComm;
    }

    [Server]
    public void DisconnectClinetMessengerFromServerReceiver()
    {
        _serverLink = null;
    }

    ///////////////////////////////////////////////////
    #region Clientside Methods

    [Client]
    public void RequestPlay()
    {
        //TODO add param of play type
        if(showDebug) Debug.Log("Client is sending a play request");
        CMD_RequestPlay();
    }

    [Client]
    public void RequestQuit()
    {
        if(showDebug) Debug.Log("Client is sending a quit request");
        CMD_RequestQuit();
    }

    [Client]
    public void RequestPlacement(uint xx, uint yy)
    {
        if (showDebug) Debug.Log("Client is sending a placement request");
        CMD_RequestPlacement(xx, yy);
    }

    #endregion

    ///////////////////////////////////////////////////
    #region Send Commands To Server

    [Command]
    private void CMD_RequestPlay()
    {
        ServerLink.RequestPlay(this.netIdentity, eGameTypes.Alpha);
    }


    [Command]
    private void CMD_RequestQuit()
    {
        ServerLink.RequestQuitApplication(this.netIdentity);
    }

    [Command]
    private void CMD_RequestPlacement(uint xx, uint yy)
    {
        ServerLink.RequestPlacement(this.netIdentity, xx, yy);
    }

    #endregion


}
