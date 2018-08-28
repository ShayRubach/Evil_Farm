using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Soldier : MonoBehaviour {

    public delegate void ClickAction(GameObject obj);
    public static event ClickAction OnClickedSoldier;

    public delegate void DragAction(Vector3 screenClickPosition, Vector3 objTranslatePosition);
    public static event DragAction OnStartDragging;
    public static event DragAction OnFinishDragging;

    private Vector3 startDragPos = new Vector3();
    private Vector3 endDragPos = new Vector3();

    void OnMouseDown() {
        //Debug.Log("clicked on " + gameObject.name);
        AssignCurrPos(ref startDragPos);
        Debug.Log("OnMouseDown startDragPos = " + startDragPos);
        if (OnStartDragging != null)
            OnStartDragging(startDragPos, transform.position);
    }


    void OnMouseDrag() {
        //do some animation here
    }

    void OnMouseUp() {
        AssignCurrPos(ref endDragPos);
        Debug.Log("OnMouseUp endDragPos = " + endDragPos);
        if (OnFinishDragging != null)
            OnFinishDragging(endDragPos, transform.position);
    }

    private void AssignCurrPos(ref Vector3 pos) {
        pos.x = Input.mousePosition.x;
        pos.y = Input.mousePosition.y;
        pos.z = Input.mousePosition.z;
        
    }
}
