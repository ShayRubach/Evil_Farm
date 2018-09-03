using System;
using System.Collections;
using DigitalRubyShared;
using UnityEngine;

/* 
 * the MVC's controller for our app.
 * it will pass commands to logic and view.
 */

public class SC_GameController : MonoBehaviour {

    private GameObject focusedPlayerParent;                //parent GameObject that holds the soldier and its weapon
    private GameObject previewSoldierPlayer;                //the enlarged previewed soldier user sees on a soldier focus (click)
    private GameModel model;                                //our game logic model
    
    private SC_Spotlight soldierSpotlight;                  //highlights the rival on drag

    private Vector3 nextPosition, startDragPos, endDragPos;

    private Animator soldierAnimator;
    private MovementDirections soldierMovementDirection;

    private static readonly string GAME_MODEL_NAME_VAR = "SC_GameModel";

    void Start() {
        
        //get reference to our model class
        model = GameObject.Find(GAME_MODEL_NAME_VAR).GetComponent<GameModel>();

        soldierSpotlight = model.GetObjects()[GameModel.SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        previewSoldierPlayer = model.GetObjects()[GameModel.PREVIEW_SOLDIER_NAME_VAR];

        //todo: fix this to always hold the animator of the current soldier..
        //soldierAnimator = model.GetObjects()["soldier_player"].GetComponent<Animator>();

        HidePreviewSoldier();
    }

    void FixedUpdate() {

        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (System.Collections.Generic.KeyValuePair<string, GameObject> obj in model.GetObjects()) {
                if (obj.Value.name.Contains("tile")) {
                    if (obj.Value.GetComponent<SC_Tile>().soldier != null)
                        Debug.Log(obj.Value.name + " has " + obj.Value.GetComponent<SC_Tile>().soldier);
                }

            }
        }
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
        SC_Soldier.MarkSoldier += MarkSoldier;
        SC_Soldier.UnmarkSoldier += UnmarkSoldier;
        GameModel.FinishGame += FinishGame;
        SC_Cart.GodMode += GodMode;
    }

    void OnDisable() {
        SC_Soldier.OnStartDragging -= OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging -= OnFinishDraggingSoldier;
        SC_Soldier.OnSoldierMovementAnimationEnd -= OnSoldierMovementAnimationEnd;
        SC_Soldier.MarkSoldier -= MarkSoldier;
        SC_Soldier.UnmarkSoldier -= UnmarkSoldier;
        GameModel.FinishGame -= FinishGame;
        SC_Cart.GodMode -= GodMode;
    }

    private void FinishGame(SoldierTeam winner) {
        //todo: announce winner
        Debug.Log("team " + winner + " won!");
    }

    private void MarkSoldier(GameObject soldier) {
        soldierSpotlight.HighlightSoldier(soldier);
    }

    private void UnmarkSoldier(GameObject soldier) {
        soldierSpotlight.RemoveHighlight(soldier);
    }

    private void OnSoldierMovementAnimationEnd() {
        FixPositionAfterAnimation();
    }

    private void OnStartDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        focusedPlayerParent = obj;
        model.FocusedPlayer = obj.transform.GetChild(0).gameObject;

        startDragPos = pos;
        ShowDuelSoldier();
        ShowPathIndicators(objTranslatePosition);
        
        //StartCoroutine("Fade");

    }

    private void OnFinishDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        endDragPos = pos;
        HidePreviewSoldier();
        HidePathIndicators();

        soldierMovementDirection = model.GetSoldierMovementDirection(startDragPos, endDragPos);
        if(soldierMovementDirection != MovementDirections.NONE) {

            //use the exact soldier position (not parent):
            Vector3 exactSoldierPosition = model.FocusedPlayer.transform.position;

            //only move if its within board borders:
            if (IsValidMove(exactSoldierPosition, soldierMovementDirection)){

                //check if next movement will initiate a fight (as landing on a rival tile): 
                if (IsPossibleMatch(GetRequestedMoveCoord())) {
                    Match();
                }
                else {
                    //currrently a static movement, will turn into animation later.
                    MoveSoldier(focusedPlayerParent, soldierMovementDirection);
                }

            }
                
            //Debug.Log("focusedSoldier.transform.GetChild(0).position= " + focusedPlayerParent.transform.GetChild(0).position);

            //Debug.Log("tile landed on is = " + 
            //    model.PointToTile(focusedPlayerParent.transform.GetChild(0).position).transform.position.x + "," +
            //    Mathf.Abs(model.PointToTile(focusedPlayerParent.transform.GetChild(0).position).transform.position.z));

            //Debug.Log("curr position is = " + objTranslatePosition);
            //nextPosition = new Vector3(objTranslatePosition.x, objTranslatePosition.y, objTranslatePosition.z + 1);
            //Debug.Log("new position will be = " + nextPosition);
        }

    }

    private void Match() {
        model.Match();
    }

    private void StartMatch() {
        throw new NotImplementedException();
    }

    private bool IsPossibleMatch(Point point) {
        return GetNextTileStatus() == TileStatus.VALID_OPPONENT;
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

    private void MoveSoldier(GameObject soldierParent, MovementDirections soldierMovementDirection) {
        model.MoveSoldier(soldierParent, soldierMovementDirection);
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

    
    private void HidePreviewSoldier() {
        previewSoldierPlayer.SetActive(false); 
    }

    private void ShowDuelSoldier() {
        previewSoldierPlayer.SetActive(true);
    }


    private void ShowPathIndicators(Vector3 pos) {
        model.ShowPathIndicators(pos);
    }

    private void HidePathIndicators() {
        model.HidePathIndicators();
    }

    private void GodMode(bool state) {
        model.GodMode(state);
    }
}
