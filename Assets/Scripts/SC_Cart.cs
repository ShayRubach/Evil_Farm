using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Cart : MonoBehaviour {

    public delegate void Hax(bool state);
    public static event Hax GodMode;

    private bool state = false;
    private static int hax = 0;
	private static readonly int CLICK_TRESHOLD = 3;


    void OnMouseDown() {
        if(++hax == CLICK_TRESHOLD) {
            if(GodMode != null) {
                GodMode(state);
            }
            hax = 0;
            state = !state;
        }
    }
}
