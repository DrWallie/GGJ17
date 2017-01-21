using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour {

    public float timeToRespawn;

    protected abstract void OnTriggerEnter(Collider c);

    protected virtual IEnumerator Respawn()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToRespawn);
        gameObject.SetActive(true);
    }
}
