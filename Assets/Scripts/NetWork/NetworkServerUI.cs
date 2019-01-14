using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Net;
using UnityEngine.UI;
using UnityEngine;

public class NetworkServerUI : MonoBehaviour {

    public GameObject[] players = new GameObject[4];
    public GameObject[] playersIcons = new GameObject[4];

    public GameObject wating;
    private int[] playerConnected = new int[4];

    public string GetIP()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[addr.Length - 1].ToString();

    }  
    
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < playerConnected.Length; i++)
            playerConnected[i] = -1;           

        NetworkServer.Reset();

        NetworkServer.useWebSockets=true;
        NetworkServer.Listen(3000);

        NetworkServer.RegisterHandler(888, ServerRecieveMessage);
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);

        
        UpdateUI();
    }

    private void UpdateUI() {

        int Connectcount = 0;
        foreach (var connect in NetworkServer.connections)
        {
            if (connect != null && connect.isConnected == transform)
                Connectcount++;
        }

        if (Connectcount < 4)
        {
            wating.transform.GetChild(2).GetComponent<Text>().text = "IP:" + GetIP();
            StartCoroutine(waitingFadeIn());
        }
        else
        {
            StartCoroutine(waitingFadeout());

        }
    }

    IEnumerator waitingFadeIn() {

        wating.GetComponent<Animator>().Play("fade_out");
        for (int i = 0; i < 4; i++)
        {
            if (playerConnected[i] != -1)
                playersIcons[i].GetComponent<IconToplayer>().showIcon();
        }

        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 0;
    }

    IEnumerator waitingFadeout() {  

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
        wating.GetComponent<Animator>().Play("fade_in");
        for (int i = 0; i < 4; i++) {
            playersIcons[i].GetComponent<IconToplayer>().hideIcon();
        }

    }
  
    public void OnDisconnected(NetworkMessage msg)
    {
        Debug.Log(msg.conn.connectionId.ToString() + " Disconnected");

        for (int i = 0; i < 4; i++)
        {
            if (playerConnected[i] == msg.conn.connectionId)
            {
                playerConnected[i] = -1;
                playersIcons[i].GetComponent<IconToplayer>().hideIcon();

                UpdateUI();

                return;
            }     
        }     
    }

    public void OnConnected(NetworkMessage msg)
    {
        Debug.Log(msg.conn.connectionId.ToString() + " Connected");

        for (int i = 0; i < 4; i++) {

            if (playerConnected[i] == -1) {

                playerConnected[i] = msg.conn.connectionId;
                playersIcons[i].GetComponent<IconToplayer>().showIcon();

                StringMessage server2Client = new StringMessage();
                server2Client.value = players[i].GetComponent<Player>().TeamIndex.ToString();

                NetworkServer.SendToClient(msg.conn.connectionId, 1000, server2Client);

                UpdateUI();

                return ;
            }
        }
    }

    void ServerRecieveMessage(NetworkMessage message)
    {
        for (int i = 0; i < 4; i++) {

            if (playerConnected[i] == message.conn.connectionId) {

                StringMessage msg = new StringMessage();
                msg.value = message.ReadMessage<StringMessage>().value;
                players[i].GetComponent<Player>().latestPressbtn = int.Parse(msg.value);

                return;
            }

        }       
    }
}