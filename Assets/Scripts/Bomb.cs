using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public int OwnerTeam;
    public float threshold;

    private float smallerFactor = 0.98f;
    private float dispearThreshold = 0.25f;

	// Use this for initialization
	void Start () {

        setColor(OwnerTeam);

        GetComponent<Painter>().threshold = threshold;

    }

    public void setColor(int team) {
        //Fetch the Renderer from the GameObject
        Renderer rend = GetComponent<Renderer>();

  
        rend.material.SetColor("Color_DCA9D026", config.GETTEAMCOLR(team)-(config.GETTEAMCOLR(team)==Color.cyan? Color.white*0.4f: Color.black) );

        GetComponent<Painter>().team = team;
    }

    // Update is called once per frame
    void Update () {

        transform.position = new Vector3(transform.position.x,transform.GetComponent<SphereCollider>().radius * transform.localScale.y, transform.position.z);
        transform.localScale = new Vector3(transform.localScale.x * smallerFactor, transform.localScale.y * smallerFactor, transform.localScale.z * smallerFactor);

        if ((transform.localScale.x< dispearThreshold) || (transform.localScale.y < dispearThreshold) || (transform.localScale.z < dispearThreshold))
            Destroy(gameObject);

        CanBePainted.PaintPoint(
            new Vector3(transform.position.x, 0, transform.position.z),
            config.GETTEAMCOLR(GetComponent<Painter>().team),
            GetComponent<SphereCollider>().radius* transform.localScale.y);
      
    }

    private void OnDestroy()
    {      
        
    }

}
