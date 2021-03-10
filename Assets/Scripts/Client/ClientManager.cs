using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(ClientMessenger))]
[RequireComponent(typeof(ClientReceiver))]
public class ClientManager : NetworkBehaviour
{



    ///////////////////////////////////////////////////

    public ClientMessenger Messenger => _messenger;
    private ClientMessenger _messenger = null;

    public ClientReceiver Receiver => _receiver;
    private ClientReceiver _receiver = null;

    ///////////////////////////////////////////////////
    #region Init

    private void Awake()
    {
        _messenger = GetComponent<ClientMessenger>();
        _receiver = GetComponent<ClientReceiver>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    #endregion

    public override void OnStartClient()
    {
        base.OnStartClient();

        _messenger.RequestPlay();
    }
}
