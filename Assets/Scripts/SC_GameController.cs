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


    private static readonly string PLAYER_NAME_VAR = "soldier";
    private static readonly string TILE_NAME_VAR = "tile";
    private static readonly string SPOTLIGHT_NAME_VAR = "spotlight";
    private static readonly string DUEL_SOLDIER_NAME_VAR = "duel_soldier_player";



    // Use this for initialization
    void Start () {
        model = GameObject.Find("SC_GameModel").GetComponent<GameModel>();
        soldierSpotlight = model.GetObjects()[SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        duelSoldierPlayer = model.GetObjects()[DUEL_SOLDIER_NAME_VAR];
        CreateTapGesture();

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
                    eventManager.FireOnClickedSoldier(clickedObject);
                    soldierSpotlight.HighlightSoldier(clickedObject);
                    ShowDuelSoldier();
                }

                if (hit.transform.name.Contains(TILE_NAME_VAR)) {
                    Debug.Log("clicked on a tile");
                    eventManager.FireOnClickedTile(clickedObject);
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
