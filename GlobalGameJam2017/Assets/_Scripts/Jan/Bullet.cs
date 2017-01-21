using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [HideInInspector]
    public int damage;
    [HideInInspector]
    public int bounces;
    [HideInInspector]
    public float force;

    private void OnCollisionEnter(Collision c)
    {
        if (!isServer)
            return;

        if(c.transform.tag == "Player")
        {
            //deal damage
            c.transform.GetComponent<PlayerManager>().RpcTakesDamage(damage);
            //addforce
        }

        //check bounce
    }
}
