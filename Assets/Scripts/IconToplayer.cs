using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconToplayer : MonoBehaviour {

    public GameObject ObjectInWorld;
    public Camera cam;

    public int index;
    public Vector3 moveing;
    public Vector3 scaling;
    public Vector3 dest;
    public float Rate=0.1f;

    private void Start()
    {
        moveing = Vector3.zero;
        scaling = Vector3.zero;
        transform.position = cam.WorldToScreenPoint(ObjectInWorld.transform.position);
        dest = transform.position;       
    }
    // Update is called once per frame
    void Update () {
        
        transform.position += moveing;
        transform.localScale += scaling;

        if (Vector3.Distance(dest, transform.position) < moveing.magnitude) {
            transform.position = dest;            
            moveing = Vector3.zero;                  
        }

        if (transform.localScale.x < 0 || transform.localScale.x > 1) {
            if (scaling.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
            scaling = Vector3.zero;
        }


	}

    public void showIcon() {

       Vector3 objectPosition = new Vector3(Screen.width/7*index+Screen.width/8,0,0)+new Vector3(0,Screen.height/3.5F,0);

        dest = objectPosition;
        moveing = (objectPosition - transform.position) * Rate;
        scaling = Vector3.one * Rate;
    }

   public  void hideIcon() {

        Vector3 objectPosition = cam.WorldToScreenPoint(ObjectInWorld.transform.position);

        dest = objectPosition;

        moveing = (objectPosition - transform.position) * Rate;
        scaling = -Vector3.one * Rate;
    }
}
