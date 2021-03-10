using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkManager))]
public class AutoStartClient : MonoBehaviour
{

    [SerializeField] private bool autoStart = false;

    NetworkManager manager = null;
    bool clientStarted = false;

    private void Start()
    {
        if (autoStart)
        {
            StartClient();
        }
    }

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void StartClient()
    {
        if (clientStarted)
            return;

        clientStarted = true;
        manager.StartClient();
    }

}
