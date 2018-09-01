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
    public SoldierType Type { get; set; }
    public GameObject Tile { get; set; }
     
    private Vector3 startDragPos = new Vector3();
    private Vector3 endDragPos = new Vector3();
    private Ray ray;
    private RaycastHit hit;
    private bool markEnemy = false;
    private GameObject currEnemy = null;

    void Start() {
        Team = gameObject.name.Contains(GameModel.PLAYER_NAME_VAR) ? SoldierTeam.PLAYER : SoldierTeam.ENEMY;
        FigureInitialWeaponType();
        ConcealWeapon();
    }


    private void ConcealWeapon() {
        //todo: get weapon and conceal it
    }

    /*
     * we can figure out what type of soldier this is by examining his children status (active/not)
     * due to the fact each player is initially holding a weapon
     */
    private void FigureInitialWeaponType() {
        if (HasChildren(gameObject)) {
            //get first child (weapon container):

            GameObject weapons = gameObject.transform.GetChild(0).gameObject;
            if (HasChildren(weapons)) {
                
                //iterate over grandchildren (weapons):
                for (int i = 0; weapons != null && i < weapons.transform.childCount; i++) {
                    GameObject child = weapons.transform.GetChild(i).gameObject;
                    if (IsObjectActive(child))
                        Type = ObjectToSoldierType(child);
                }
            }
        }

    }

    private SoldierType ObjectToSoldierType(GameObject child) {
        if (child.name.Contains(SoldierType.PITCHFORK.ToString().ToLower())) {
            return SoldierType.PITCHFORK;
        }
        if (child.name.Contains(SoldierType.SHIELD.ToString().ToLower())) {
            return SoldierType.SHIELD;
        }
        if (child.name.Contains(SoldierType.AXE.ToString().ToLower())) {
            return SoldierType.AXE;
        }
        if (child.name.Contains(SoldierType.SCYTHE.ToString().ToLower())) {
            return SoldierType.SCYTHE;
        }
        if (child.name.Contains(SoldierType.SCARECROW.ToString().ToLower())) {
            return SoldierType.SCARECROW;
        }
        if (child.name.Contains(SoldierType.FARMER.ToString().ToLower())) {
            return SoldierType.FARMER;
        }
        if (child.name.Contains(SoldierType.CLUB.ToString().ToLower())) {
            return SoldierType.CLUB;
        }
        return SoldierType.NO_TYPE;
    }

    private bool IsObjectActive(GameObject obj) {
        return obj.activeSelf;
    }

    private bool HasChildren(GameObject obj ) {
        return (obj.transform.childCount > 0 );
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
            currEnemy = null;
        }
    }

    private void MarkEnemy(GameObject enemy) {
        if (enemy) {
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
