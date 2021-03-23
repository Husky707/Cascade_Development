using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using UnityEngine.SceneManagement;

public class ClientRoomManager : MonoBehaviour
{
    public event Action<eScenes> E_ClientLoadedScene = delegate { };

    //CurrentRoom
    public NetworkRoomData CurrentRoom => _currentRoom;
    [SerializeField] NetworkRoomData _currentRoom;

    public eRoomRoles CurrentRole => _currentRole;
    eRoomRoles _currentRole = eRoomRoles.Null;

    public uint CurrentRoomId => _currentRoomId;
    uint _currentRoomId = 0;

    public string CurrentRoomName => _currentRoomName;
    string _currentRoomName = "";

    //DefaultRoom
    [SerializeField] NetworkRoomData defaultRoom;
    [SerializeField] NetworkRoomData alphaGameRoom;
    [SerializeField] NetworkRoomData alphaLobbyRoom;

    Coroutine coroutine = null;
    bool coRunning = false;


    //////////////////////////////////////////////////
    //Public input

    public void JoinNewRoom(uint roomId, string name, eNetRooms roomType, eRoomRoles withRole)
    {
        _currentRole = withRole;
        _currentRoomId = roomId;
        _currentRoomName = name;

        OnJoinedNewRoom(GetRoomDataByType(roomType));
    }

    public void ReturnToHub()
    {
        _currentRole = eRoomRoles.Player;
        _currentRoomId = 0;
        _currentRoomName = defaultRoom.Name;
    }

    //////////////////////////////////////////////////
    /// Private implementation
    private void OnJoinedNewRoom(NetworkRoomData roomData)
    {
        eNetRooms room = roomData.Type;
        if (room == eNetRooms.Null)
        {
            Debug.Log("Client cannot enter a null room.");
            return;
        }

        if (room == CurrentRoom.Type)
        {
            Debug.Log("Attempted to reload current room scene.");
            return;
        }

        NetworkRoomSetupInstructions setup = roomData.RoomSetup;
        if (coRunning)
        {
            StopCoroutine(coroutine);
            coRunning = false;
        }

        coroutine = StartCoroutine(LoadNewScene(setup.SceneToLoad, CurrentRoom.RoomSetup.TimeToCloseRoom, CurrentRoom.RoomSetup.ExitAnimation, setup.EntryAnimation));
    }

    private void OnExitedRoom()
    {

    }


    private void OnReturnToHub()
    {
        if (CurrentRoom.Type == defaultRoom.Type)
        {
            Debug.Log("Change scene stopped: Already in hub.");
            return;
        }

        if (coRunning)
        {
            StopCoroutine(coroutine);
            coRunning = false;
        }

        NetworkRoomSetupInstructions newSetup = defaultRoom.RoomSetup;
        NetworkRoomSetupInstructions oldSetup = CurrentRoom.RoomSetup;

        coroutine = StartCoroutine(LoadNewScene(newSetup.SceneToLoad, oldSetup.TimeToCloseRoom, oldSetup.ExitAnimation, newSetup.EntryAnimation));
    }



    ///////////////////////////////////////////////////
    //Helpers
    IEnumerator LoadNewScene(eScenes scene, float minTransitionTime, Animator exitAnimation, Animator entryAnimation)
    {
        coRunning = true;
        float tic = 0f;

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        //Load next scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);

        //Begin exit animation
        if (exitAnimation != null)
            exitAnimation.SetTrigger("Start");

        while(tic < minTransitionTime || !loadOp.isDone)
        {
            tic += Time.deltaTime;
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        if (entryAnimation != null)
            entryAnimation.SetTrigger("Start");

        E_ClientLoadedScene.Invoke(scene);
        coRunning = false;
    }

    NetworkRoomData GetRoomDataByType(eNetRooms roomType)
    {
        //TEMP ugly implementation
        switch(roomType)
        {
            case (eNetRooms.Hub):
            {
                    return defaultRoom;
            }
            case eNetRooms.Lobby:
            {
                return alphaLobbyRoom;
            }
            case eNetRooms.Game:
            {
                return alphaGameRoom;
            }
            default:
                return defaultRoom;
        }
    }
}
