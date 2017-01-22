using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_SyncPos : NetworkBehaviour
{
    [SyncVar]
    private Vector3 syncPos;

    [SerializeField]
    private Transform myTransform;

    [SerializeField]
    private float lerpRate = 15;

    //Update is called once per frame
    void Start()
    {
        myTransform = GetComponent<Transform>();
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
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
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
            CmdProvidePositionToServer(myTransform.position);
        }
    }
}