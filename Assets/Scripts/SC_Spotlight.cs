﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Spotlight : MonoBehaviour {

    static Vector3 basePosition;

    void Start() {
        basePosition = gameObject.transform.position;
    }

    public void HighlightSoldier(GameObject soldier) {
        transform.position = new Vector3(soldier.transform.position.x, 3, soldier.transform.position.z);
    }

    public void RemoveHighlight() {
        transform.position = basePosition;
    }


}
