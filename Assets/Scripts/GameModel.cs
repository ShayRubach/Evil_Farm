using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * the Board. it will hold all needed data and calculations
 * for our gameplay and be responsible for the pre-instantiations
 * of the GameObjects.
*/

public class GameModel : MonoBehaviour {
    
    private static readonly int COLS = 7;
    private static readonly int ROWS = 6;

    private static readonly string UNITY_OBJECTS_TAG = "UnityObject";

    private static readonly int LEFT_BOARD_EDGE_IDX = 0;
    private static readonly int RIGHT_BOARD_EDGE_IDX = COLS - 1;
    private static readonly int TOP_BOARD_EDGE_IDX = 0;
    private static readonly int BTM_BOARD_EDGE_IDX = ROWS - 1;

    public static readonly string PLAYER_NAME_VAR = "soldier";
    public static readonly string TILE_NAME_VAR = "tile_";
    public static readonly string SPOTLIGHT_NAME_VAR = "spotlight";
    public static readonly string DUEL_SOLDIER_NAME_VAR = "preview_soldier_player";
    public static readonly string PATH_INDICATORS_NAME_VAR = "path_indicators";

    private GameObject pathIndicators;

    private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    private void Awake() {
        GameObject[] objectsArray = GameObject.FindGameObjectsWithTag(UNITY_OBJECTS_TAG);

        foreach (GameObject obj in objectsArray) {
            objects.Add(obj.name, obj);
        }

        pathIndicators = objects[GameModel.PATH_INDICATORS_NAME_VAR];
    }
    
    public GameObject PointToTile(Vector3 pos) {
        Debug.Log("PointToTile: " + pos);
        float x=0, y=0;
        x = pos.x;
        y = Mathf.Abs(pos.z);

        Debug.Log("x,y = " + x + "," + y);

        return objects[TILE_NAME_VAR + x + y];
    }

    public Dictionary<string,GameObject> GetObjects() {
        return objects;
    }

    public GameObject GetObject(string name) {
        return objects[name];
    }

    public MovementDirections GetSoldierMovementDirection(Vector3 pos) {
        float angle = Mathf.Atan2(-pos.y, -pos.x) * Mathf.Rad2Deg;
        return CalculateMovementDirectionByAngle(angle);
    }

    private MovementDirections CalculateMovementDirectionByAngle(float angle) {
        MovementDirections movement = MovementDirections.NONE;

        if (angle >= -45.0f && angle <= 45.0f) {
            movement = MovementDirections.LEFT;
        }
        else if (angle >= -135.0f && angle <= -45.0f) {
            movement = MovementDirections.UP;
        }
        else if (angle >= 45.0f && angle <= 135.0f) {
            movement = MovementDirections.DOWN;
        }
        else {
            movement = MovementDirections.RIGHT;
        }


        Debug.Log("movement direction = " + movement);
        return movement;
    }

    public void MoveSoldier(GameObject focusedSoldier, MovementDirections soldierMovementDirection) {

        switch (soldierMovementDirection) {
            case MovementDirections.UP:
                focusedSoldier.transform.position = new Vector3(focusedSoldier.transform.position.x, focusedSoldier.transform.position.y, focusedSoldier.transform.position.z + 1);
                break;
            case MovementDirections.DOWN:
                focusedSoldier.transform.position = new Vector3(focusedSoldier.transform.position.x, focusedSoldier.transform.position.y, focusedSoldier.transform.position.z - 1);
                break;
            case MovementDirections.LEFT:
                focusedSoldier.transform.position = new Vector3(focusedSoldier.transform.position.x - 1, focusedSoldier.transform.position.y, focusedSoldier.transform.position.z);
                break;
            case MovementDirections.RIGHT:
                focusedSoldier.transform.position = new Vector3(focusedSoldier.transform.position.x + 1, focusedSoldier.transform.position.y, focusedSoldier.transform.position.z);
                break;
        }

    }

    public void ShowPathIndicators(Vector3 objectPos) {
        Debug.Log("ShowPathIndicators: pos = {" + objectPos.x + " , " + Mathf.Abs(objectPos.z) + "}");

        ResetIndicators();                                      //enable and show all indicators.
        pathIndicators.transform.position = objectPos;          //move all indicators so they surround the object.
        FilterIndicators(objectPos);                            //hide non travesal indicators.
        
    }

    private void ResetIndicators() {

        pathIndicators.SetActive(true);

        for (int i = 0; i < pathIndicators.transform.childCount; ++i) {
            pathIndicators.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    /*
     * used to decide which indicators (right,left,up,down) indicators are eligible to be displayed
     * according to the position of the soldier
    */
    private void FilterIndicators(Vector3 pos) {

        //soldier is located in most left side of the border
        if (pos.x == LEFT_BOARD_EDGE_IDX) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.LEFT).gameObject);
        }
        //soldier is located in most right side of the border
        if (pos.x == RIGHT_BOARD_EDGE_IDX) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.RIGHT).gameObject);
        }
        //soldier is located in the top side of the border
        if (Mathf.Abs(pos.z) == TOP_BOARD_EDGE_IDX) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.UP).gameObject);
        }
        //soldier is located in the bottom side of the border
        if (Mathf.Abs(pos.z) == BTM_BOARD_EDGE_IDX) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.DOWN).gameObject);
        }

        
    }

    private void HideObjectUnderBoard(GameObject obj) {
        //obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 1, obj.transform.position.z);
        obj.SetActive(false);
    }

    public void HidePathIndicators() {
        Debug.Log("HidePathIndicators called");
        HideObjectUnderBoard(pathIndicators);
    }
}
