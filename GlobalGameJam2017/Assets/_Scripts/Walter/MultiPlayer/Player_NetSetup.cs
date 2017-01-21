using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetSetup : NetworkBehaviour
{
    [SerializeField]
    AudioListener audioListener;
    [SerializeField]
    Camera PlayerCam;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    void Start()
    {
        if (!isLocalPlayer)
        {
            Disable();
            AssignRemoteLayer();
        }

        RegisterPlayer();
    }

    void RegisterPlayer()
    {
        string _ID = "Player  " + GetComponent<NetworkIdentity>().netId;
        transform.name = _ID;
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void Disable()
    {
        PlayerCam.enabled = false;
        audioListener.enabled = false;
        GetComponent<PlayerController>().enabled = false;
        //GetComponent<Rigidbody>().enabled = false;
    }
}