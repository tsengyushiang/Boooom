using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAll : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
            collision.gameObject.GetComponent<Player>().Dead();
        else
            Destroy(collision.gameObject);
    }
}
