using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBall : MonoBehaviour {
    
    public float explosionR;
    public GameObject PaintEffect;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Painter>() != null) {

            int team;
            if (other.gameObject.GetComponent<Player>() != null)
                team = other.gameObject.GetComponent<Player>().getTeam();
            else
                team = other.gameObject.GetComponent<Painter>().team;

            if (team != -1) {

                GameObject paint = Instantiate(PaintEffect, transform.position, transform.rotation);
                ParticleSystem.MainModule newMain = paint.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                newMain.startColor = config.GETTEAMCOLR(team);

                CanBePainted.PaintPoint(transform.position, config.GETTEAMCOLR(team), explosionR);
            }
        }
           
    }   
    

}
