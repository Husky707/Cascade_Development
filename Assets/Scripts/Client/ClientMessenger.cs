using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientMessenger : NetworkBehaviour
{

    public IReceiveOnServer ServerLink { get { if (!isServer) return null; return _serverLink; } }
    private IReceiveOnServer _serverLink = null;


    ///////////////////////////////////////////////////
    ///Init
    
    [Server]
    public void LinkClientMessengerToServerReceiver(IReceiveOnServer netComm)
    {
        if (netComm == null)
        {
            Debug.Log("Could not link client messenger: Null NetworkCommunicator.");
            return;
        }

        _serverLink = netComm;
    }

    ///////////////////////////////////////////////////
    #region Client Methods Commands

    [Client]
    public void RequestPlay()
    {
        //TODO add param of play type
        Debug.Log("Client is sending a play request");
        CMD_RequestPlay();
    }


    #endregion

    ///////////////////////////////////////////////////
    #region Send Commands To Server

    [Command]
    private void CMD_RequestPlay()
    {
        ServerLink.RequestPlay(this.netIdentity);
    }


    #endregion


}
