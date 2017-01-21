using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPull : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Pull(transform.position, 30f);
	}

    void Pull (Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (Collider c in hitColliders)
        {
            if(c.transform.tag == "Player")
            {
                c.transform.position = Vector3.Lerp(c.transform.position, transform.position, 5F * Time.deltaTime);
            }
        }
    }
}
