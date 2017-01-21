using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour {

    public float timeToRespawn;

    protected virtual void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Player")
            PickupThis(c);
    }

    protected virtual void PickupThis(Collider c)
    {
        StartCoroutine(Respawn());
        PlayerManager pM = c.GetComponent<PlayerManager>();
        if (pM == LocalGameManager.thisPlayer)
            AddPickupToPlayer(pM);
    }

    protected abstract void AddPickupToPlayer(PlayerManager pM);

    protected virtual IEnumerator Respawn()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToRespawn);
        gameObject.SetActive(true);
    }
}
