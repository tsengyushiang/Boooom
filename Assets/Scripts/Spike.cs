using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

    public GameObject explosionEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Player>() != null)
        {
            other.transform.GetComponent<Player>().Dead();
        }
        else
        {
            Destroy(other.gameObject);
            GameObject explosion= Instantiate(explosionEffect);
            explosion.transform.position = other.transform.position;
        }
    }
  
}
