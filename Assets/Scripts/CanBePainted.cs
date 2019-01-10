using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CanBePainted : MonoBehaviour
{

    private static Mesh mesh;
    private static Vector3[] vertices;
    private static Color[] colors;
    public static int winner;

    public Texture2D texture;
  

    // Use this for initialization
    void Awake()
    {
        Time.timeScale = 1.0f;
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetTexture("Texture2D_82858D65", texture);

        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        colors = new Color[vertices.Length];
        mesh.colors = colors;
    }
    public static int GETCOLOR(Color c)
    {
        int total = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (c == colors[i])
                total++;
        }

        return total;       
    }
    public static void PaintPoint(Vector3 point, Color color, float threshold)
    {
        if (color == Color.white) return;

        for (int i = 0; i < vertices.Length;i++) {          
            if (Vector3.Distance(vertices[i], point) < threshold)
            {
                colors[i] = color;
            }
         }
        mesh.colors = colors;
    }  

}
