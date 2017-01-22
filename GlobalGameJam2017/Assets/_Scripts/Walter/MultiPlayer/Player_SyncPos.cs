using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.016f)]
public class Player_SyncPos : NetworkBehaviour
{
    [SyncVar]
    private Vector3 syncPos;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float lerpRate = 15;

    //Update is called once per frame
    void Start()
    {
        playerTransform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, syncPos, Time.deltaTime * lerpRate);// * lerpRate);
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvidePositionToServer(playerTransform.position);
        }
    }
}