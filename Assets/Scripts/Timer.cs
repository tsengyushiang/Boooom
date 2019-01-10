using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public float TotalTime = 90f;
    private float Totalmove = 19.33f*1.5f;
    
    public float RemainTime;
    public GameObject resultScene;

    private Color currentWin=Color.white;
    private static Mesh mesh;
    private static Vector3[] vertices;
    private static Color[] colors;

    // Use this for initialization
    void Start()
    {
        RemainTime = TotalTime;
       

        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        colors = new Color[vertices.Length];
        updateTimebar(currentWin, Color.black);

        Renderer rend = gameObject.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Renderer>();
        rend.materials[1].SetColor("_Color", config.GETTEAMCOLR(0));

        rend = gameObject.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Renderer>();
        rend.materials[1].SetColor("_Color", config.GETTEAMCOLR(1));
        Time.timeScale = 1.0f;

    }

    void  updateTimebar(Color Remain ,Color used) {

        for (int i = 0; i < colors.Length; i++)
        {

            if (i > colors.Length * (RemainTime/ TotalTime))
            {
                colors[i] = used;
            }
            else
            {
                colors[i] = Remain;
            }
        }
        if (Remain == config.GETTEAMCOLR(0))
        {
            transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
        }
        else if (Remain == config.GETTEAMCOLR(1))
        {
            transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
        }
        else 
        {
            transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
        }
        mesh.colors = colors;
    }

	// Update is called once per frame
	void Update () {

        if (RemainTime > 0) {
            RemainTime -= Time.deltaTime;
           
            float team1=CanBePainted.GETCOLOR(config.GETTEAMCOLR(0));
            float team2=CanBePainted.GETCOLOR(config.GETTEAMCOLR(1));
            float dist = (Mathf.Abs(team1 - team2)) / (team1 + team2);
            int winner = team1 > team2 ? 0 : 1;
            
            currentWin = config.GETTEAMCOLR(winner);


            transform.GetChild(winner).GetComponent<move2dst>().setdst(
                new Vector3(transform.GetChild(winner).position.x,
                0.5f,
                Mathf.Lerp(transform.GetChild(winner).position.z, -9.81f+(RemainTime/TotalTime)*Totalmove,Time.deltaTime)));

            transform.GetChild(winner == 0 ? 1 : 0).GetComponent<move2dst>().setdst( 
                new Vector3(transform.GetChild(winner).position.x, 
                0.2f,
                transform.GetChild(winner).position.z + dist*10));


            updateTimebar(currentWin, Color.black);


            if (RemainTime <= 0)
            {
                resultScene.SetActive(true);
                resultScene.transform.GetChild(1).GetComponent<Image>().color = config.GETTEAMCOLR(winner);
            }
        }       

    }
}
