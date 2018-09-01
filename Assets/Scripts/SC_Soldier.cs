using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Soldier : MonoBehaviour {

    public delegate void ClickAction(GameObject obj);
    public static event ClickAction OnClickedSoldier;

    public delegate void OnSoldierAction(GameObject obj);
    public static event OnSoldierAction MarkSoldier;
    public static event OnSoldierAction UnmarkSoldier;


    public delegate void DragAction(GameObject obj, Vector3 screenClickPosition, Vector3 objTranslatePosition);
    public static event DragAction OnStartDragging;
    public static event DragAction OnFinishDragging;

    public delegate void OnAnimationEnd();
    public static event OnAnimationEnd OnSoldierMovementAnimationEnd;
    
    public SoldierTeam Team { get; set; }
    public GameObject Tile { get; set; }
     
    private Vector3 startDragPos = new Vector3();
    private Vector3 endDragPos = new Vector3();
    private Ray ray;
    private RaycastHit hit;
    private bool markEnemy = false;
    private GameObject currEnemy = null;

    void Start() {
        Team = gameObject.name.Contains(GameModel.PLAYER_NAME_VAR) ? SoldierTeam.PLAYER : SoldierTeam.ENEMY;
    }

    void OnMouseDown() {
        AssignCurrPos(ref startDragPos);
        if (OnStartDragging != null)
            OnStartDragging(gameObject.transform.parent.gameObject, startDragPos, transform.position);
    }

    void OnMouseDrag() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {

            markEnemy = HoveredOverEnemy(hit.collider.name);

            if (markEnemy) {
                currEnemy = hit.collider.gameObject;
                //todo: only mark enemy if he's in range
                MarkEnemy(currEnemy);
            }
            else {
                RemoveEnemyMark(currEnemy);
            }
        }
    }

    private void RemoveEnemyMark(GameObject currEnemy) {
        if (currEnemy) {
            if (UnmarkSoldier != null) {
                UnmarkSoldier(currEnemy);
            }
            Debug.Log("removing mark from " + currEnemy.name);
            currEnemy = null;
        }
    }

    private void MarkEnemy(GameObject enemy) {
        if (enemy) {
            Debug.Log("marking " + enemy.name);
            if (MarkSoldier != null) {
                MarkSoldier(enemy);
            }
        }
            
    }

    private bool HoveredOverEnemy(string objHitName) {
        return (objHitName.Contains(GameModel.ENEMY_NAME_VAR));
    }

    void OnMouseUp() {
        AssignCurrPos(ref endDragPos);
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
