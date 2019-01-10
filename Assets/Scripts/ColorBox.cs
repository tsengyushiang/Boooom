using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBox : MonoBehaviour {


	// Use this for initialization
	void Start () {
        GetComponent<Painter>().team = -1;
	}
	
	// Update is called once per frame
	void Update () {
        CanBePainted.PaintPoint(
            new Vector3(transform.position.x, 0, transform.position.z),
            config.GETTEAMCOLR(GetComponent<Painter>().team),
            GetComponent<Painter>().threshold);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Painter>() != null)
        {
            if (collision.gameObject.GetComponent<Player>()!=null)
                GetComponent<Painter>().team = collision.gameObject.GetComponent<Player>().getTeam();
            else
            {
                if (collision.gameObject.GetComponent<Painter>().team == -1) return;
                GetComponent<Painter>().team = collision.gameObject.GetComponent<Painter>().team;
            }

            Renderer rend = transform.GetChild(0).GetComponent<Renderer>();
            rend.material.SetColor("_Color", config.GETTEAMCOLR(GetComponent<Painter>().team));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Painter>() != null)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
                GetComponent<Painter>().team = collision.gameObject.GetComponent<Player>().getTeam();
            else
            {
                if (collision.gameObject.GetComponent<Painter>().team == -1) return;
                GetComponent<Painter>().team = collision.gameObject.GetComponent<Painter>().team;
            }
            Renderer rend = transform.GetChild(0).GetComponent<Renderer>();
            rend.material.SetColor("_Color", config.GETTEAMCOLR(GetComponent<Painter>().team));
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.GetComponent<Painter>() != null)
        {         
            int leaveTeam = 0;
            if (collision.gameObject.GetComponent<Player>() != null)
                leaveTeam = collision.gameObject.GetComponent<Player>().getTeam();
            else
                leaveTeam = collision.gameObject.GetComponent<Painter>().team;

            if(leaveTeam== GetComponent<Painter>().team)
                GetComponent<Painter>().team = -1;

            Renderer rend = transform.GetChild(0).GetComponent<Renderer>();
            rend.material.SetColor("_Color", config.GETTEAMCOLR(GetComponent<Painter>().team));
        }
    }

}
