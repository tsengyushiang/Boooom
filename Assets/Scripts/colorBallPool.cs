using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorBallPool : MonoBehaviour {

    public GameObject ball;
    public float emitPower;
    public int freqency;
    public float offset;
    public float threshold;
    private int team=-1;

    private int accumTime = 0;

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        accumTime++;

        
        if (accumTime % freqency == 0 ) {
                emit();
        }        
	}

    void emit() {

        if (team == -1) return;

        GameObject b = Instantiate(ball);
        b.transform.position = transform.position + Vector3.Normalize(transform.forward) * offset;
        b.GetComponent<Rigidbody>().AddForce(transform.forward * emitPower);
        b.GetComponent<Rigidbody>().mass = 500;
        b.GetComponent<Bomb>().OwnerTeam = team;
        b.GetComponent<Bomb>().threshold = threshold;
        accumTime = 0;

    }


    public void setColor(int setteam)
    {
        team = setteam;
        Renderer rend = transform.GetChild(0).GetComponent<Renderer>();
        rend.material.SetColor("_Color", config.GETTEAMCOLR(team));
    }

    private void OnCollisionStay(Collision collision)
    {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                setColor(collision.gameObject.GetComponent<Player>().getTeam());
            }
         
    }

    private void OnCollisionEnter(Collision collision)
    {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                setColor(collision.gameObject.GetComponent<Player>().getTeam());
            if (accumTime > freqency)
                emit();
        }
            

          
    }

    private void OnCollisionExit(Collision collision)
    {
        
            if (collision.gameObject.GetComponent<Player>() != null && collision.gameObject.GetComponent<Player>().getTeam() == team)
            {
                setColor(-1);
            }        

       
    }

}
