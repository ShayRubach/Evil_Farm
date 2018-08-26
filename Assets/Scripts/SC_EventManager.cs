using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EventManager {


    private static SC_EventManager instance = null;
    private static readonly System.Object lockingObj = new System.Object();

    public delegate void ClickAction(GameObject obj);
    public static event ClickAction OnClickedSoldier;
    public static event ClickAction OnClickedTile;

    private SC_EventManager() { }

    public static SC_EventManager GetInstance {
        get {
            if (instance == null) {
                lock (lockingObj) {
                    instance = (instance == null) ? new SC_EventManager() : instance;
                }
            }
            return instance;
        }
    }

    public void FireOnClickedSoldier(GameObject obj) {
        if (OnClickedSoldier != null)
            OnClickedSoldier(obj);
    }

    public void FireOnClickedTile(GameObject obj) {
        if (OnClickedTile != null)
            OnClickedTile(obj);
    }

}
