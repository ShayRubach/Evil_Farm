using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Spotlight : MonoBehaviour {

    public void HighlightSoldier(GameObject soldier) {
        Debug.Log("HighlightSoldier called.");
        transform.position = new Vector3(soldier.transform.position.x, transform.position.y, soldier.transform.position.z);
    }
}
