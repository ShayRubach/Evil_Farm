﻿using System;
using System.Collections;
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
    
    private SC_Spotlight soldierSpotlight;
    private SC_EventManager eventManager = SC_EventManager.GetInstance;

    private Vector3 onScreenPreviewSoldierVector;
    private Vector3 offScreenPreviewSoldierVector;
    private Vector3 nextPosition, startDragPos, endDragPos;

    private Animator soldierAnimator;
    private MovementDirections soldierMovementDirection;

    private static readonly string GAME_MODEL_NAME_VAR = "SC_GameModel";

    void Start() {
        
       
        //get reference to our model class
        model = GameObject.Find(GAME_MODEL_NAME_VAR).GetComponent<GameModel>();

        
        
        soldierSpotlight = model.GetObjects()[GameModel.SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        duelSoldierPlayer = model.GetObjects()[GameModel.DUEL_SOLDIER_NAME_VAR];

        //todo: fix this to always hold the animator of the current soldier..
        //soldierAnimator = model.GetObjects()["soldier_player"].GetComponent<Animator>();



        onScreenPreviewSoldierVector = new Vector3(duelSoldierPlayer.transform.position.x, duelSoldierPlayer.transform.position.y, duelSoldierPlayer.transform.position.z);
        offScreenPreviewSoldierVector = new Vector3(duelSoldierPlayer.transform.position.x, duelSoldierPlayer.transform.position.y - 10.0f, duelSoldierPlayer.transform.position.z);

        HideDuelSoldier();

    }


    IEnumerator Fade() {
        GameObject leaf = GameObject.Find("leaf_test");
        Renderer sr = leaf.GetComponent<Renderer>();

        for (float f = 1f; f >= 0; f -= 0.1f) {
            Color c = sr.material.color;
            c.a = f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
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
        
        //StartCoroutine("Fade");

    }

    private void OnFinishDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        endDragPos = pos;
        HideDuelSoldier();
        HidePathIndicators();

        soldierMovementDirection = model.GetSoldierMovementDirection(startDragPos, endDragPos);
        if(soldierMovementDirection != MovementDirections.NONE) {

            //fetch the exact soldier position of the parent obj:
            Vector3 exactSoldierPosition = focusedSoldierParent.transform.GetChild(0).transform.position;

            //only move if its within board borders:
            if (IsValidMove(exactSoldierPosition, soldierMovementDirection)){

                //check if next movement will initiate a fight (landing on a rival tile): 
                IsPossibleMatch(GetRequestedMoveCoord());

                //currrently a static movement, will turn into animation later.
                MoveSoldier(focusedSoldierParent, soldierMovementDirection);
            }
                
            Debug.Log("focusedSoldier.transform.GetChild(0).position= " + focusedSoldierParent.transform.GetChild(0).position);

            Debug.Log("tile landed on is = " + 
                model.PointToTile(focusedSoldierParent.transform.GetChild(0).position).transform.position.x + "," +
                Mathf.Abs(model.PointToTile(focusedSoldierParent.transform.GetChild(0).position).transform.position.z));

            //Debug.Log("curr position is = " + objTranslatePosition);
            //nextPosition = new Vector3(objTranslatePosition.x, objTranslatePosition.y, objTranslatePosition.z + 1);
            //Debug.Log("new position will be = " + nextPosition);
        }

    }

    private void IsPossibleMatch(Point point) {
        TileStatus status = GetNextTileStatus();
        Debug.Log("IsPossibleMatch: status = " + status);
    }

    private TileStatus GetNextTileStatus() {
        return model.GetNextTileStatus();
    }

    private Point GetRequestedMoveCoord() {
        return model.GetNextMoveCoord();
    }

    private bool IsValidMove(Vector3 exactSoldierPosition, MovementDirections soldierMovementDirection) {
        return model.IsValidMove(exactSoldierPosition, soldierMovementDirection);
    }

    private void MoveSoldier(GameObject focusedSodlier, MovementDirections soldierMovementDirection) {
        model.MoveSoldier(focusedSoldierParent, soldierMovementDirection);
    }

    private void DisplayMovementAnimation() {

        //Debug.Log("DisplayMovementAnimation called.");
        //soldierAnimator.SetBool("IsMoving", true);
        //soldierAnimator.Play("soldier_solo_movement_forward");


    }

    public void FixPositionAfterAnimation() {
        //Debug.Log("FixPositionAfterAnimation called.");
        //Debug.Log("nextPosition= " + nextPosition);


        //soldierAnimator.Play("soldier_solo_movement_idle");
        //gameObject.transform.position = nextPosition;
        //soldierAnimator.SetBool("IsMoving", false);
    }

    
    private void HideDuelSoldier() {
         duelSoldierPlayer.transform.position = offScreenPreviewSoldierVector;
    }

    private void ShowDuelSoldier() {
        duelSoldierPlayer.transform.position = onScreenPreviewSoldierVector;
    }


    private void ShowPathIndicators(Vector3 pos) {
        model.ShowPathIndicators(pos);
    }

    private void HidePathIndicators() {
        model.HidePathIndicators();
    }

}
