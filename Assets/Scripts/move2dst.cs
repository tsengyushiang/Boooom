using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move2dst : MonoBehaviour {

    private Vector3 dst=Vector3.zero;
    private float moveSpeed=0.05f;

    private void Start()
    {
        dst = transform.position+Vector3.one;
    }

    public void setdst(Vector3 newdst) {
        
        dst = newdst;
    }

	// Update is called once per frame
	void Update () {

        transform.GetChild(0).GetComponent<Animator>().speed = Vector3.Distance(dst, transform.position)+1;

        if (Vector3.Distance(dst, transform.position)> moveSpeed)
          transform.position +=(dst-transform.position) / Vector3.Distance(dst, transform.position)*moveSpeed;
        if (Vector3.Distance(dst, transform.position) < moveSpeed)
            transform.position = dst;

	}
}
