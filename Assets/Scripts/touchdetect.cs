using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class touchdetect : MonoBehaviour {


    public float sensitive = 50;

    public bool[] activeBtn = new bool[5];
    public GameObject controller;
    public GameObject dropBomb;
    public GameObject swapBtn;
    public GameObject ConfigBtn;
    
    private Vector2 controllerPosition ;
    private Vector2 DropBombPosition;
    private bool controllOnLeft;
    private bool configuring;
    private int configBtnCoolDown;
    private int swapBtnCooldown;

    private void Start()
    {
       configBtnCoolDown = 0;
        swapBtnCooldown = 0;
       controllerPosition = new Vector2(Screen.width / 5, Screen.height / 2f);
       DropBombPosition = new Vector2(Screen.width - controllerPosition.x, controllerPosition.y);
       controllOnLeft = true;
       configuring = false;

        controller.transform.position = controllerPosition;
        dropBomb.transform.position = DropBombPosition;
        
    }

    void swap() {

        if (swapBtnCooldown > 0) return;

        controllOnLeft = !controllOnLeft;

        Vector3 tmp = controller.transform.position;
        controller.transform.position = dropBomb.transform.position;
        dropBomb.transform.position = tmp;
        swapBtnCooldown = 20;
    }

    void setconfig() {

        if (configBtnCoolDown > 0) return;

        if (configuring == true)
        {
            configuring = false;
            swapBtn.SetActive(false);
            configBtnCoolDown = 20;
        }
        else
        {
            configuring = true;
            swapBtn.SetActive(true);
            configBtnCoolDown = 20;
        }

    }

    // Update is called once per frame
    void Update () {

        if (configBtnCoolDown > 0) configBtnCoolDown--;
        else if (configBtnCoolDown < 0) configBtnCoolDown = 0;

        if (swapBtnCooldown > 0) swapBtnCooldown--;
        else if (swapBtnCooldown < 0) swapBtnCooldown = 0;

        if (Input.touchCount > 0)
        {
            if (configuring == false)
            {
                bool dropbtnPressed = false;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    //觸碰configbtn
                    if (Vector2.Distance(Input.touches[i].position,ConfigBtn.transform.position)<Screen.width/10)
                    {
                        setconfig();
                        return;
                    }

                    //觸碰點到上下左右controller的向量
                    Vector2 dir = new Vector2((Input.touches[i].position.x - controller.transform.position.x), Input.touches[i].position.y - controller.transform.position.y);

                    //觸碰點離放炸彈按鈕的距離
                    if (Vector2.Distance(Input.touches[i].position, new Vector2(dropBomb.transform.position.x, dropBomb.transform.position.y)) < Screen.width / 10)
                    {
                        dropbtnPressed = true;
                    }

                    //觸碰離上下左右太晚就break
                    if ((dir.magnitude > Screen.height / 3) || (dir.magnitude > Screen.width / 3)) break;

                    dir.Normalize();


                    //根據公式算出點即是上下左右何者
                    float angle = 0;
                    if (dir.x < 0)
                    {
                        angle = 270 - (Mathf.Atan(dir.y / -dir.x) * Mathf.Rad2Deg);
                    }
                    else
                    {
                        angle = 90 + (Mathf.Atan(dir.y / dir.x) * Mathf.Rad2Deg);
                    }
                    if ((angle <= 45.0f && angle >= 0.0f) || (angle >= 315.0f && angle <= 360.0f))
                    {
                        activeBtn[1] = true;
                        activeBtn[0] = false;
                        activeBtn[2] = false;
                        activeBtn[3] = false;
                    }
                    else if (angle >= 45 && angle < 135)
                    {
                        activeBtn[3] = true;
                        activeBtn[0] = false;
                        activeBtn[2] = false;
                        activeBtn[1] = false;
                    }
                    else if (angle >= 135 && angle < 225)
                    {

                        activeBtn[0] = true;
                        activeBtn[1] = false;
                        activeBtn[2] = false;
                        activeBtn[3] = false;
                    }
                    else if (angle >= 225 && angle < 315)
                    {
                        activeBtn[2] = true;
                        activeBtn[0] = false;
                        activeBtn[1] = false;
                        activeBtn[3] = false;
                    }
                }

                activeBtn[4] = dropbtnPressed;

                for (int i = 4; i >= 0; i--)
                {
                    if (activeBtn[i] == true)
                    {
                        NetworkClientUI.SendControllerInfo(i);
                        return;
                    }
                }
            }
            else {

                for (int i = 0; i < Input.touchCount; i++)
                {
                    //觸碰configbtn
                    if (Vector2.Distance(Input.touches[i].position, ConfigBtn.transform.position) < Screen.width / 20)
                    {
                        setconfig();
                        return;
                    }

                    //左右交換按鈕
                    if (Vector2.Distance(Input.touches[i].position, swapBtn.transform.position) < Screen.width/20) {
                        swap();
                        return;
                    }

                    //touch在螢幕左邊
                    if (Input.touches[i].position.x < Screen.width / 2)
                    {
                        //控制上下左右的在左邊
                        if (controllOnLeft == true)
                        {
                            controller.transform.position = Input.touches[i].position;                            
                        }
                        //控制上下左右的在右邊
                        else
                        {
                            dropBomb.transform.position = Input.touches[i].position;
                        }

                    }
                    //touch在螢幕右邊
                    else
                    {                   

                        // 控制上下左右的在左邊
                        if (controllOnLeft == true)
                        {
                            dropBomb.transform.position = Input.touches[i].position;
                        }
                        //控制上下左右的在右邊
                        else
                        {
                            controller.transform.position = Input.touches[i].position;
                        }
                    }                  
               
                }


            }
            
        }
        else
        {
            for (int i = 4; i >= 0; i--)
            {
                activeBtn[i] = false;
            }

            NetworkClientUI.SendControllerInfo(-1);
        }     

    }
    
    public void AddAcitve(int i) {      
        if (i >= activeBtn.Length) return;
        activeBtn[i]=true;
    }

    public void RemoveAcitve(int i)
    {       
        if (i >= activeBtn.Length) return;
        activeBtn[i] = false;
    }



}
