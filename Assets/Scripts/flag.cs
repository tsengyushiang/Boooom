using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flag : MonoBehaviour {

    public int ownerteam;
    public float radius = 9f;
    public GameObject splashEffect;
    public int freqency;
    private float accumTime = 0;
    public float detectObjR=0;


    // Use this for initialization
    void Start () {
        setTeam(ownerteam);
        GetComponent<SphereCollider>().radius = radius;
      //  emitChangeColor(GetComponent<SphereCollider>().radius);        
    }
    
	// Update is called once per frame
	void Update () {

        if (ownerteam != -1)
            if (accumTime > freqency && accumTime - freqency < radius)
            {                
                emitChangeColor(accumTime % freqency);
                accumTime += 0.2f ;

                if (accumTime - freqency > radius)
                {                   
                    accumTime = 0;
                }
            }
            else
            {                
                accumTime++;
                detectObjR = 0;
            }

    }


    private void OnTriggerStay(Collider other)
    {

        if (new Vector2(other.transform.position.x - transform.position.x, other.transform.position.z - transform.position.z).magnitude < detectObjR) {

        
            if (other.gameObject.GetComponent<colorBallPool>() != null)
            {
                other.gameObject.GetComponent<colorBallPool>().setColor(ownerteam);
            }
            if (other.gameObject.GetComponent<Rock>() != null)
            {
                other.gameObject.GetComponent<Rock>().changeOwner(ownerteam);
            }
            if (other.gameObject.GetComponent<Player>() != null) {
                if (other.gameObject.GetComponent<Player>().getTeam() != ownerteam) {
                    other.gameObject.GetComponent<Player>().Dead();
                }
            }
        }

        if (other.gameObject.GetComponent<Painter>() != null) {           

            if (Vector3.Distance(other.transform.position, transform.position) < 0.7f) {

                if (accumTime > freqency) return;

                if (other.gameObject.GetComponent<Player>() != null)
                {
                    if (ownerteam != other.gameObject.GetComponent<Player>().getTeam()) 
                      setTeam(other.gameObject.GetComponent<Player>().getTeam());
                }
                else
                {
                    if(ownerteam!= other.gameObject.GetComponent<Painter>().team && other.gameObject.GetComponent<Painter>().team!=-1)
                     setTeam(other.gameObject.GetComponent<Painter>().team);
                }
            }
            
        }
    }


    public void setTeam(int team) {
        accumTime = freqency;
        for (int i = 0; i < transform.childCount; i++) {
            if (i == team)
                transform.GetChild(i).gameObject.SetActive(true);
            else
                transform.GetChild(i).gameObject.SetActive(false);
        }
        ownerteam = team;
    }

    private void emitChangeColor(float r) {

        if (Time.timeScale < 1) return;
        detectObjR = r;

        CanBePainted.PaintPoint(transform.position, config.GETTEAMCOLR(ownerteam), r);

        if (r == 1)
        {
            
            GameObject ripple = Instantiate(splashEffect);
            ripple.transform.position = transform.position + Vector3.up * 0.01f;
            var ps = ripple.GetComponent<ParticleSystem>().main;
            ps.startSize = radius * 1.5f;
        }


    }

}
