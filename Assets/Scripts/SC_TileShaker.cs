using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TileShaker : MonoBehaviour {

    public GameObject tile;
    private bool tiltLeft = true;
    private int duration = 0;

    void Update() {
        /*
        if (tiltLeft) {
            transform.Rotate(Vector3.up * Time.deltaTime * 20);
            
            if (duration++ == 100) {
                duration = 0;
                tiltLeft = false;
            }

        }
        else {
            transform.Rotate(Vector3.down * Time.deltaTime * 20);
            if (duration++ == 100) {
                duration = 0;
                tiltLeft = true;
            }
        }
        */

    }
}
