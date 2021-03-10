using UnityEngine;
using Mirror;

public interface INetworkCommunicator
{

    void RequestPlay(NetworkIdentity id);
}
