using System;
using DigitalRubyShared;
using UnityEngine;

/* 
 * the MVC's controller for our app.
 * it will pass commands to logic and view.
 */

public class SC_GameController : MonoBehaviour {

    private GameObject focusedSoldierParent;                //parent GameObject that holds the soldier and its weapon
    private GameModel model;
    private GameObject duelSoldierPlayer;
    private GameObject pathIndicators;

    private SC_Spotlight soldierSpotlight;
    private SC_EventManager eventManager = SC_EventManager.GetInstance;

    private Vector3 onScreenDuelSoldierVector;
    private Vector3 offScreenDuelSoldierVector;
    private Vector3 nextPosition, startDragPos, endDragPos, relativePos;

    private Animator soldierAnimator;
    private MovementDirections soldierMovementDirection;

    private static readonly string GAME_MODEL_NAME_VAR = "SC_GameModel";

    void Start() {
        
        //get reference to our model class
        model = GameObject.Find(GAME_MODEL_NAME_VAR).GetComponent<GameModel>();

        
        pathIndicators = model.GetObjects()[GameModel.PATH_INDICATORS_NAME_VAR];
        soldierSpotlight = model.GetObjects()[GameModel.SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        duelSoldierPlayer = model.GetObjects()[GameModel.DUEL_SOLDIER_NAME_VAR];

        //todo: fix this to always hold the animator of the current soldier..
        //soldierAnimator = model.GetObjects()["soldier_player"].GetComponent<Animator>();



        onScreenDuelSoldierVector = new Vector3(duelSoldierPlayer.transform.position.x, duelSoldierPlayer.transform.position.y, duelSoldierPlayer.transform.position.z);
        offScreenDuelSoldierVector = new Vector3(duelSoldierPlayer.transform.position.x, duelSoldierPlayer.transform.position.y - 10.0f, duelSoldierPlayer.transform.position.z);

        HideDuelSoldier();

    }

    void OnEnable() {
        SC_Soldier.OnStartDragging += OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging += OnFinishDraggingSoldier;
        SC_Soldier.OnSoldierMovementAnimationEnd += OnSoldierMovementAnimationEnd;
    }

    void OnDisable() {
        SC_Soldier.OnStartDragging -= OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging -= OnFinishDraggingSoldier;
        SC_Soldier.OnSoldierMovementAnimationEnd -= OnSoldierMovementAnimationEnd;
    }

    private void OnSoldierMovementAnimationEnd() {
        FixPositionAfterAnimation();
    }

    private void OnStartDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        focusedSoldierParent = obj;
        startDragPos = pos;
        ShowDuelSoldier();
        ShowPathIndicators(objTranslatePosition);
    }

    private void OnFinishDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        endDragPos = pos;
        relativePos = endDragPos - startDragPos;
        HideDuelSoldier();
        HidePathIndicators();

        soldierMovementDirection = model.GetSoldierMovementDirection(relativePos);
        if(soldierMovementDirection != MovementDirections.NONE) { 
            //currrently a static movement, will turn into animation later.
            DisplayMovementAnimation();

            Debug.Log("focusedSoldier.transform.GetChild(0).position= " + focusedSoldierParent.transform.GetChild(0).position);

            Debug.Log("tile landed on is = " + 
                model.PointToTile(focusedSoldierParent.transform.GetChild(0).position).transform.position.x + "," +
                Mathf.Abs(model.PointToTile(focusedSoldierParent.transform.GetChild(0).position).transform.position.z));

            //Debug.Log("curr position is = " + objTranslatePosition);
            //nextPosition = new Vector3(objTranslatePosition.x, objTranslatePosition.y, objTranslatePosition.z + 1);
            //Debug.Log("new position will be = " + nextPosition);
        }

    }

    private void MoveSoldier(GameObject focusedSodlier, MovementDirections soldierMovementDirection) {
        model.MoveSoldier(focusedSoldierParent, soldierMovementDirection);
    }

    private void DisplayMovementAnimation() {
        //Debug.Log("DisplayMovementAnimation called.");
        //soldierAnimator.SetBool("IsMoving", true);
        //soldierAnimator.Play("soldier_solo_movement_forward");
        MoveSoldier(focusedSoldierParent, soldierMovementDirection);

    }

    public void FixPositionAfterAnimation() {
        //Debug.Log("FixPositionAfterAnimation called.");
        //Debug.Log("nextPosition= " + nextPosition);


        //soldierAnimator.Play("soldier_solo_movement_idle");
        //gameObject.transform.position = nextPosition;
        //soldierAnimator.SetBool("IsMoving", false);
    }

    
    private void HideDuelSoldier() {
         duelSoldierPlayer.transform.position = offScreenDuelSoldierVector;
    }

    private void ShowDuelSoldier() {
        duelSoldierPlayer.transform.position = onScreenDuelSoldierVector;
    }


    private void ShowPathIndicators(Vector3 pos) {
        pathIndicators.transform.position = pos;
    }

    private void HidePathIndicators() {
        pathIndicators.transform.position = offScreenDuelSoldierVector;
    }

}
