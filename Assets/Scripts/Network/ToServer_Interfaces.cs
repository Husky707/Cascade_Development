using UnityEngine;
using Mirror;

//Commands sent by client, receiverd on server
public interface IReceiveClientCommands 
{

    void RequestPlay(NetworkIdentity id, eGameTypes type);

    void RequestQuitApplication(NetworkIdentity id);

    void RequestPlacement(NetworkIdentity identity, uint xx, uint yy);

}

//Commands on client sent to server
public interface ISendServerCommands
{
    void RequestPlay(eGameTypes types);
    void RequestQuitApplication();

    void RequestPlacement(uint xx, uint yy);


}