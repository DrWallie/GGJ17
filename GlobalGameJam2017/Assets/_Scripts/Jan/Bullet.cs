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

    protected virtual void OnCollisionEnter(Collision c)
    {
        if (!isServer)
            return;

        if(c.transform.tag == "Player")
        {
            //deal damage
            DealsDamage(c.transform.GetComponent<PlayerManager>());
            AddForce(c);
            AddForce(c);
        }

        //check bounce
    }

    protected virtual void DealsDamage(PlayerManager pM)
    {
        pM.RpcTakesDamage(damage);
    }

    protected virtual void AddForce(Collision c)
    {
        //add a force to the colliding victims rigidbody
    }

    /* 
    REFERENCE:

    print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
    Debug.DrawRay(contact.point, contact.normal, Color.white);

    */

    protected virtual void Bounce(Collision c)
    {
        if (bounces == 0)
        {
            DestroyBullet();
            return;
        }

        Rigidbody rB = GetComponent<Rigidbody>();
        rB.velocity = -c.contacts[0].normal;
        bounces--;
    }

    protected virtual void DestroyBullet()
    {
        Destroy(transform);
    }
}
