using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Soldier : MonoBehaviour {

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
    public bool Revealed { get; set; }
    public bool Alive { get; set; }

    private GameObject initialWeapon;
    private Vector3 initialPos;
    private Vector3 startDragPos = new Vector3();
    private Vector3 endDragPos = new Vector3();
    private Ray ray;
    private RaycastHit hit;
    private bool markEnemy = false;
    private GameObject currEnemy = null;

    void Awake() {
        
        //saving some initial soldier attributes for game restart optimizations:
        initialPos = transform.position;
        initialWeapon = GetActiveWeapon();

        Init();
    }

    public void Init() {
        gameObject.SetActive(true);
        Team = gameObject.name.Contains(SC_GameModel.PLAYER_NAME_VAR) ? SoldierTeam.PLAYER : SoldierTeam.ENEMY;
        transform.position = initialPos;
        Revealed = false;
        Alive = true;
        Type = ObjectToSoldierType(initialWeapon);
        //FigureInitialWeaponType();
        DisplayWeapon();
        ConcealWeapon(initialWeapon);
        RemoveFocus();
    }
    
    /* 
     * remove red spotlight focus that procs after a kill.
     */
    private void RemoveFocus() {
        foreach(Transform child in transform) {
            if (child.gameObject.name.Contains(SC_GameModel.SPOTLIGHT_NAME_VAR)) {
                child.gameObject.SetActive(false);
            }
        }
    }

    /*
     * activate the initial weapon chosen by game scene. this is a util method to enhance the restart game usability.
     */
    private void DisplayWeapon() {
        GameObject child = null;
        GameObject weapons = GetAllWeapons();

        //iterate over grandchildren (weapons) and force disable
        for (int i = 0; weapons != null && i < weapons.transform.childCount; i++) {
            child = weapons.transform.GetChild(i).gameObject;
            child.SetActive(false);
        }

        //display correct initial weapon
        initialWeapon.SetActive(true);
    }

    public void ConcealWeapon(GameObject weaponParentObj) {
        if (Team == SoldierTeam.ENEMY) {
            //hide weapon
            weaponParentObj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            //show flower
            weaponParentObj.transform.parent.transform.parent.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void RevealWeapon() {
        //show weapon
        GetActiveWeapon().transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        //hide flower
        GetActiveWeapon().transform.parent.transform.parent.GetChild(1).gameObject.SetActive(false);
    }

    /*
     * we can figure out what type of soldier this is by examining his children status (active/not)
     * due to the fact each player is initially holding a weapon
     */
    private void FigureInitialWeaponType() {
        SoldierType fetchedType = ObjectToSoldierType(GetActiveWeapon());
        if(fetchedType != SoldierType.NO_TYPE)
            Type = fetchedType;
    }

    internal GameObject GetActiveWeapon() {
        GameObject child = null;
        GameObject weapons = GetAllWeapons();

        //iterate over grandchildren (weapons) and find the active weapon:
        for (int i = 0; weapons != null && i < weapons.transform.childCount; i++) {
            child = weapons.transform.GetChild(i).gameObject;
            if (IsObjectActive(child)) {
                return child;
            }
        }

        Debug.Log("GetActiveWeapon: couldn't find active weapon. returns null");
        return null;
    }

    private SoldierType ObjectToSoldierType(GameObject child) {

        if (child == null)
            return SoldierType.NO_TYPE;

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
        if (child.name.Contains(SoldierType.CRYSTAL.ToString().ToLower())) {
            return SoldierType.CRYSTAL;
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

        if (AttemptingToMoveEnemySoldier())
            return;

        AssignCurrPos(ref startDragPos);
        if (OnStartDragging != null)
            OnStartDragging(gameObject.transform.parent.gameObject, startDragPos, transform.position);

    }

    void OnMouseDrag() {

        if (AttemptingToMoveEnemySoldier())
            return;
        /*
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
        */
    }

    void OnMouseUp() {
        if (AttemptingToMoveEnemySoldier())
            return;

        AssignCurrPos(ref endDragPos);
        if (OnFinishDragging != null)
            OnFinishDragging(gameObject.transform.parent.gameObject, endDragPos, transform.position);
    }

    private bool AttemptingToMoveEnemySoldier() {
        //dont let player move the enemy soldiers on his turn
        return gameObject.GetComponent<SC_Soldier>().Team == SoldierTeam.ENEMY;
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
        //todo: only mark enemy if he's in range
        return (objHitName.Contains(SC_GameModel.ENEMY_NAME_VAR));
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

    public override string ToString() {
        return "Team = " + Team + " | "
            + "Type = " + Type + " | "
            + "Tile  = {" + Tile.GetComponent<SC_Tile>().transform.position.x
            + "," + Mathf.Abs(Tile.GetComponent<SC_Tile>().transform.position.z) + "}";
    }

    /*
     * setting new weapon & refreshing the ui with the new weapon.
     * disables curr weapon and enables new weapon.
     */
    internal void RefreshWeapon(SoldierType newWeapon, bool conceal = true) {
        GameObject child = null;
        GameObject weapons;
        weapons = GetAllWeapons();

        //set new weapon indicator
        Type = newWeapon;

        //iterate over grandchildren (weapons) and find the active weapon:
        for (int i = 0; weapons != null && i < weapons.transform.childCount; i++) {
            child = weapons.transform.GetChild(i).gameObject;
            //turn off curr active weapon
            if (IsObjectActive(child)) {
                child.SetActive(false);
            }
            //turn on new weapon
            if (child.name.Contains(newWeapon.ToString().ToLower())) {
                child.SetActive(true);
                //RevealWeapon();

                if(conceal)
                    ConcealWeapon(child);
            }
        }
    }

    private GameObject GetAllWeapons() {
        if (HasChildren(gameObject)) {
            //get the weapon container game object:
            GameObject weapons = gameObject.transform.GetChild(0).gameObject;
            if (HasChildren(weapons)) {
                return weapons;
            }
        }

        Debug.Log("GetAllWeapons: from " + gameObject.name + " :couldn't find all weapons. returns null");
        return null;
    }
}
