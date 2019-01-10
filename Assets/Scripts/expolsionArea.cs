using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class expolsionArea : MonoBehaviour {

    
    public float explosionRadius;
    public float explosionForce;
    private float explosionAddFactor;

	// Use this for initialization
	void Start () {
        float intialR = GetComponent<SphereCollider>().radius;
        explosionAddFactor = (explosionRadius - intialR) / 14;
    }	
	// Update is called once per frame
	void Update () {

        if (GetComponent<SphereCollider>().enabled == true && GetComponent<SphereCollider>().radius < explosionRadius)
        {
            GetComponent<SphereCollider>().radius += explosionAddFactor;
        }
        else {
            Destroy(gameObject);
        }      
	}

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rig = collision.gameObject.GetComponent<Rigidbody>();
        if (rig != null) {
            rig.AddForce(Vector3.Normalize(collision.transform.position-this.transform.position)* explosionForce);
        }
    }

}
