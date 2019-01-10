using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    public int team = 0;
    public GameObject MagicHit;

    // Use this for initialization
    void Start() {
        changeOwner(team);
    }

    // Update is called once per frame
    void Update() {

    }

    public void changeOwner(int newowner) {

        if(newowner!=team)
            GetComponent<Rigidbody>().isKinematic = true;

        team = newowner;
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", config.GETTEAMCOLR(team));
       
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Bomb>() != null)
        {
            Instantiate(MagicHit, transform.position, transform.rotation);

            //彈跳力道根據砲彈質量做改變
            float bounceForce = collision.gameObject.GetComponent<Rigidbody>().mass * 500f;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(collision.transform.position-transform.position)* bounceForce);
            collision.transform.localScale = Vector3.one;
        }

        //該顏色玩家可以移動石頭
        if (collision.gameObject.GetComponent<Player>() != null) {
            if (team == -1) changeOwner(collision.gameObject.GetComponent<Player>().TeamIndex);
            if (collision.gameObject.GetComponent<Player>().TeamIndex != team) return;
             GetComponent<Rigidbody>().isKinematic = false;
        }        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (collision.gameObject.GetComponent<Player>().TeamIndex != team) return;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
