using DigitalRubyShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * the MVC's controller for our app.
 * it will pass commands to logic and view.
 */

public class SC_GameController : MonoBehaviour {

    private SC_Spotlight soldierSpotlight;
    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;
    private SwipeGestureRecognizer swipeGesture;
    private SC_EventManager eventManager = SC_EventManager.GetInstance;
    private GameModel model;
    private Ray ray;
    private RaycastHit hit;
    private GameObject duelSoldierPlayer;
    private Vector3 onScreenDuelSoldierVector;
    private Vector3 offScreenDuelSoldierVector;
    private Animator soldierAnimator;
    private GameObject pathIndicators;


    private static readonly string PLAYER_NAME_VAR = "soldier";
    private static readonly string TILE_NAME_VAR = "tile";
    private static readonly string SPOTLIGHT_NAME_VAR = "spotlight";
    private static readonly string DUEL_SOLDIER_NAME_VAR = "duel_soldier_player";

    void Awake() {
        soldierAnimator = GetComponent<Animator>();
    }

    void OnEnable() {
        SC_Soldier.OnStartDragging += OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging += OnFinishDraggingSoldier;
    }

    void OnDisable() {
        SC_Soldier.OnStartDragging -= OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging -= OnFinishDraggingSoldier;
    }

    private void OnStartDraggingSoldier(Vector3 pos, Vector3 objTranslatePosition) {
        Debug.Log("drag started at " + pos);
        ShowDuelSoldier();
        ShowPathIndicators(objTranslatePosition);
    }

    private void ShowPathIndicators(Vector3 pos) {
        pathIndicators.transform.position = pos;
    }

    private void HidePathIndicators() {
        pathIndicators.transform.position = offScreenDuelSoldierVector;
    }

    private void OnFinishDraggingSoldier(Vector3 pos, Vector3 objTranslatePosition) {
        Debug.Log("drag finished at " + pos);
        HideDuelSoldier();
        DisplayMovementAnimation();
        HidePathIndicators();
    }

    private void DisplayMovementAnimation() {
        Debug.Log("DisplayMovementAnimation called.");
        soldierAnimator.SetBool("IsMoving", true);
    }

    private void FixPositionAfterAnimation() {
        Debug.Log("FixPositionAfterAnimation called.");
        soldierAnimator.SetBool("IsMoving", false);


    }





    // Use this for initialization
    void Start () {
        model = GameObject.Find("SC_GameModel").GetComponent<GameModel>();
        pathIndicators = model.GetObjects()["path_indicators"];
        soldierSpotlight = model.GetObjects()[SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        duelSoldierPlayer = model.GetObjects()[DUEL_SOLDIER_NAME_VAR];
        
        //CreateTapGesture();

        onScreenDuelSoldierVector = new Vector3(duelSoldierPlayer.transform.position.x, duelSoldierPlayer.transform.position.y, duelSoldierPlayer.transform.position.z);
        offScreenDuelSoldierVector = new Vector3(duelSoldierPlayer.transform.position.x, duelSoldierPlayer.transform.position.y - 10.0f, duelSoldierPlayer.transform.position.z);

        HideDuelSoldier();

    }

    private void HideDuelSoldier() {
         duelSoldierPlayer.transform.position = offScreenDuelSoldierVector;
    }

    // Update is called once per frame
    void FixedUpdate () {

            


    }

    private void HandleClickEvent() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject clickedObject = null;

        //Raycast on the clicked object and get its name
        if (Physics.Raycast(ray, out hit)) {
            string clickedObjectName = hit.transform.name;

            try {
                //get a reference to the clicked object by name
                clickedObject = model.GetObjects()[clickedObjectName];

                if (clickedObjectName.Contains(PLAYER_NAME_VAR)) {
                    Debug.Log("clicked on a player");
                    soldierSpotlight.HighlightSoldier(clickedObject);
                    ShowDuelSoldier();
                }

                if (hit.transform.name.Contains(TILE_NAME_VAR)) {
                    Debug.Log("clicked on a tile");
                    HideDuelSoldier();
                }

            }
            catch(KeyNotFoundException ex) {
                //do nothing
            }


        }
        
        return;
}

    private void ShowDuelSoldier() {
        duelSoldierPlayer.transform.position = onScreenDuelSoldierVector;
    }

    private void TapGestureCallback(GestureRecognizer gesture) {
        if (gesture.State == GestureRecognizerState.Ended) {
            DebugText("tap ended at {0}, {1}", gesture.FocusX, gesture.FocusY);
            HandleClickEvent();
        }

    }

    private void CreateTapGesture() {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void DebugText(string text, params object[] format) {
        Debug.Log(string.Format(text, format));
    }

}
