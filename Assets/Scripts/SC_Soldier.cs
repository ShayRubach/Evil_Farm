using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Soldier : MonoBehaviour {

    public delegate void ClickAction(GameObject obj);
    public static event ClickAction OnClickedSoldier;

    public delegate void DragAction(GameObject obj, Vector3 screenClickPosition, Vector3 objTranslatePosition);
    public static event DragAction OnStartDragging;
    public static event DragAction OnFinishDragging;

    public delegate void OnAnimationEnd();
    public static event OnAnimationEnd OnSoldierMovementAnimationEnd;
    
    private Vector3 startDragPos = new Vector3();
    private Vector3 endDragPos = new Vector3();

    public SoldierTeam team;
    public GameObject tile;
    

    void FixedUpdate() {
    }

    void OnMouseDown() {
        Debug.Log("clicked on " + gameObject);

        AssignCurrPos(ref startDragPos);
        //Debug.Log("OnMouseDown startDragPos = " + startDragPos);
        if (OnStartDragging != null)
            OnStartDragging(gameObject.transform.parent.gameObject, startDragPos, transform.position);
    }

    void OnMouseDrag() {
        //do some animation here
    }

    void OnMouseUp() {
        AssignCurrPos(ref endDragPos);
        //Debug.Log("OnMouseUp endDragPos = " + endDragPos);
        if (OnFinishDragging != null)
            OnFinishDragging(gameObject.transform.parent.gameObject, endDragPos, transform.position);
    }

    public void OnAnimationEnded() {

        if (OnSoldierMovementAnimationEnd != null)
            OnSoldierMovementAnimationEnd();

    }

    private void AssignCurrPos(ref Vector3 pos) {
        pos.x = Input.mousePosition.x;
        pos.y = Input.mousePosition.y;
        pos.z = Input.mousePosition.z;
        
    }
}
