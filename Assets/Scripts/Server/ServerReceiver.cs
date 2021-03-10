using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerReceiver : NetworkBehaviour, IReceiveOnServer
{

    public void RequestPlay(NetworkIdentity id)
    {
        Debug.Log("Server is resolving a play request.");
    }
}
