using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client_SceneLink : MonoBehaviour
{
    public static Client_SceneLink Game = null;
    public ClientManager Player = null;

    private void Awake()
    {
        if (Player == null)
            Player = FindObjectOfType<ClientManager>();

        if (Player == null)
            Debug.Log("Could not establish connection to player");
        else
            Debug.Log("Scene successfully linked to player");

        if (Game == null)
            Game = this;
    }


    //Alpha Implementation ONLY
    public void Play()
    {
        Player.Play();
    }

    public bool Place(uint xx, uint yy)
    {
        return Player.RequestPlacement(xx, yy);

    }


}
