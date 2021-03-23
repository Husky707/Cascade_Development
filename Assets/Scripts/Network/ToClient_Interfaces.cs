//Commands sent by the server, received on a client
public interface IReceiveServerCommands
{
    //Severside connection Methods
    void ConnectClientMessenger(IReceiveClientCommands serverReceiver);
    void DisconnectClientMessenger();

    //Room Methods
    void ClientEnteredRoom(uint roomid, string roomName, eNetRooms roomType, eRoomRoles withRole);
    void ClientExitedRoom(uint roomid);

    void ReturnToHub();

}

//Commands sent by a server
public interface ISendClientCommands
{
    //Severside connection Methods
    void ConnectClientMessenger(ClientReceiver target, IReceiveClientCommands serverReceiver);
    void DisconnectClientMessenger(ClientReceiver target);

    //Room Methods
    void ClientEnteredRoom(ClientReceiver target, uint roomid, string roomName, eNetRooms roomType, eRoomRoles withRole);
    void ClientExitedRoom(ClientReceiver target, uint roomid);

    void ReturnToHub(ClientReceiver target);
}