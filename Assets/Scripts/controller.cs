using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
    
            if (Input.GetKey(KeyCode.Space))
            {
                NetworkClientUI.SendControllerInfo(4);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                NetworkClientUI.SendControllerInfo(0);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                NetworkClientUI.SendControllerInfo(1);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                NetworkClientUI.SendControllerInfo(2);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                NetworkClientUI.SendControllerInfo(3);
            }
            else
            {
                NetworkClientUI.SendControllerInfo(-1);
            }


        }

    }

