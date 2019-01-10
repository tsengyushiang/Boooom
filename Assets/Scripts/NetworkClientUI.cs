using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityStandardAssets.CrossPlatformInput;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class NetworkClientUI : MonoBehaviour {

    static NetworkClient client;
    static int latest = -2;

    public string connectIP = "140.118.127.121";

    public InputField ipfield;
    public Button connectbtn;
    public Button disconnectbtn;
    public GameObject[] buttons;
 
    public void changeIP(string newip) {

        connectIP = ipfield.text;
    }

    private void updateUI() {
        if (!client.isConnected)
        {
            ipfield.gameObject.SetActive(true);
            connectbtn.gameObject.SetActive(true);
            disconnectbtn.gameObject.SetActive(false);

            setButtonColor(Color.white);
        }
        else
        {
            ipfield.gameObject.SetActive(false);
            connectbtn.gameObject.SetActive(false);
            disconnectbtn.gameObject.SetActive(true);
        }
    }

    // Use this for initialization
    void Start () {
        client = new NetworkClient();
        
        string test = Application.absoluteURL;
        char[] c = { '/', '.' };

        string[] k = test.Split(c);

        string currentIP="";
        if (k.Length > 6) {
            currentIP = k[2] + '.' + k[3] + '.' + k[4] + '.' + k[5];
        }

        ipfield.transform.GetChild(0).gameObject.GetComponent<Text>().text = currentIP;
        connectIP = currentIP;

        updateUI();
    }    

    public void Connect()
    {
        client.RegisterHandler(1000, myTeam);
        client.Connect(connectIP, 3000);     
    }

    public void myTeam(NetworkMessage message) {

        StringMessage msg = new StringMessage();
        msg.value = message.ReadMessage<StringMessage>().value;

        if (int.Parse(msg.value) == 0)
        {
            setButtonColor(Color.red*0.7f+Color.green*0.3f);
        }
        else if (int.Parse(msg.value) == 1) {
            setButtonColor(Color.cyan);
        }
    }

    void setButtonColor(Color team) {

        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].GetComponent<Image>().color = team;
        }

    }


    public void send(int a) {
        SendControllerInfo(a);
    }

    static public void SendControllerInfo(int key) {

        //Debug.Log(key);

        if (client.isConnected) {

            if (latest == key) return;
            latest = key;
            StringMessage msg = new StringMessage();
            msg.value = key.ToString();
            
            client.Send(888, msg);
        }
    }

    // Update is called once per frame
    void Update () {
        updateUI();

    }

    public void disconnect()
    {
        client.Disconnect();
    }

    public void endgame() {
        Application.Quit();
    }

}
