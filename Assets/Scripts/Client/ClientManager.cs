using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(ClientMessenger))]
[RequireComponent(typeof(ClientReceiver))]
[RequireComponent(typeof(ClientRoomManager))]
public class ClientManager : NetworkBehaviour
{



    ///////////////////////////////////////////////////

    public ClientMessenger Messenger => _messenger;
    private ClientMessenger _messenger = null;

    public ClientReceiver Receiver => _receiver;
    private ClientReceiver _receiver = null;

    public ClientRoomManager RoomManager => _roomManager;
    private ClientRoomManager _roomManager = null;


    ///////////////////////////////////////////////////
    #region Init

    private void Awake()
    {
        _messenger = GetComponent<ClientMessenger>();
        _receiver = GetComponent<ClientReceiver>();
        _roomManager = GetComponent<ClientRoomManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

       //TEST _messenger.RequestPlay();
    }

    private void OnEnable()
    {
        SetupEventListeners();
    }

    private void OnDisable()
    {
        SetupEventListeners(false);
    }

    void SetupEventListeners(bool link = true)
    {
        if(link)
        {
            _receiver.E_EnteredRoom += OnEnteredRoom;
            _receiver.E_ExitedRoom += OnExitedRoom;
            _receiver.E_RetrunToHub += OnReturnToHub;
        }
        else
        {
            _receiver.E_EnteredRoom -= OnEnteredRoom;
            _receiver.E_ExitedRoom -= OnExitedRoom;
            _receiver.E_RetrunToHub -= OnReturnToHub;
        }
    }

    #endregion

    ///////////////////////////////////////////////////
    #region Event Landings

    void OnEnteredRoom(uint roomId, string name, eNetRooms roomType, eRoomRoles withRole)
    {
        RoomManager.JoinNewRoom(roomId, name, roomType, withRole);
    }

    void OnExitedRoom(uint uitn)
    {
        
    }

    void OnReturnToHub()
    {
        RoomManager.ReturnToHub();
    }


    #endregion

    ///////////////////////////////////////////////////
    ///Public requests

    public void Play()
    {
        _messenger.RequestPlay();
    }

    public void Quit()
    {
        _messenger.RequestQuit();
    }

    public bool RequestPlacement(uint xx, uint yy)
    {
        _messenger.RequestPlacement(xx, yy);
        return false;
    }
}
