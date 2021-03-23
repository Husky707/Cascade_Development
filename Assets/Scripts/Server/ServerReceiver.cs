using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ServerReceiver : NetworkBehaviour, IReceiveClientCommands
{

    public bool LogRequests = false;

    #region Request Events

    public event Action<NetworkIdentity, eGameTypes> E_PlayRequested = delegate { };
    public event Action<NetworkIdentity> E_QuitAppRequested = delegate { };

    public event Action<NetworkIdentity, uint, uint> E_PlacementRequested = delegate { };


    #endregion

    ///////////////////////////////////////////////////

    public void RequestPlay(NetworkIdentity id, eGameTypes type)
    {
        if(LogRequests) Debug.Log("Server is resolving a play request.");

        E_PlayRequested.Invoke(id, type);
    }

    public void RequestQuitApplication(NetworkIdentity identity)
    {
        if (LogRequests) Debug.Log("Server is resolving a quit application request.");

        E_QuitAppRequested.Invoke(identity);
    }

    public void RequestPlacement(NetworkIdentity identity, uint xx, uint yy)
    {
        if (LogRequests) Debug.Log("Server is resolving a placement request.");

        E_PlacementRequested.Invoke(identity, xx, yy);

    }
}
