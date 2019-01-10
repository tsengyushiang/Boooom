using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class config : MonoBehaviour {
    public static float DROPBOMBCOOLDOWN=1;
    private static Color[] teamcolors = { Color.red, Color.cyan};
    public static Color GETTEAMCOLR(int index) {
        switch (index) {
            case 0:
                return teamcolors[0];
            case 1:
                return teamcolors[1];
            default:
                return Color.white;
        }
    }
}
