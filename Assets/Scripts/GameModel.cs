using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point {
    public int x { get; set; }
    public int y { get; set; }
}

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
    private static readonly float MINIMUM_DRAG_DISTANCE = 40.0f;

    public static readonly string NO_SOLDIER_NAME_VAR = "no_soldier";
    public static readonly string PLAYER_NAME_VAR = "soldier";
    public static readonly string TILE_NAME_VAR = "tile_";
    public static readonly string SPOTLIGHT_NAME_VAR = "spotlight";
    public static readonly string DUEL_SOLDIER_NAME_VAR = "preview_soldier_player";
    public static readonly string PATH_INDICATORS_NAME_VAR = "path_indicators";
    public static readonly string LEAF_INDICATOR_NAME_VAR = "leaf";

    private Vector3 relativePos;
    private GameObject pathIndicators;
    private Point nextMoveCoord;

    private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    private void Awake() {

        GameObject[] objectsArray = GameObject.FindGameObjectsWithTag(UNITY_OBJECTS_TAG);

        foreach (GameObject obj in objectsArray) {
            objects.Add(obj.name, obj);
        }

        pathIndicators = objects[GameModel.PATH_INDICATORS_NAME_VAR];

        nextMoveCoord.x = 0;
        nextMoveCoord.y = 0;

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

    public MovementDirections GetSoldierMovementDirection(Vector3 startPos, Vector3 endPos) {
        relativePos = endPos - startPos;

        Debug.Log("distance: " + Vector3.Distance(startPos, endPos));
        if (Vector3.Distance(startPos, endPos) < MINIMUM_DRAG_DISTANCE) {
            return MovementDirections.NONE;
        }

        float angle = Mathf.Atan2(-relativePos.y, -relativePos.x) * Mathf.Rad2Deg;
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

    public TileStatus GetNextTileStatus() {
        GameObject tile = objects[TILE_NAME_VAR + nextMoveCoord.x + nextMoveCoord.y];
        SC_Soldier soldier = tile.GetComponent<SC_Tile>().GetCurrSoldier();

        if (soldier != null) {
            //next tile is occupied with a soldier
            if(soldier.Team == SoldierTeam.ENEMY) {
                //soldier from the rivals's team
                return TileStatus.VALID_OPPONENT;
            }
            else {
                //soldier from the player's team
                return TileStatus.PLAYER_SOLDIER;
            }
        }
        
        return TileStatus.TRV_TILE;
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

    public bool IsValidMove(Vector3 soldierPos, MovementDirections move) {
        bool isValid = false;
        nextMoveCoord.x = (int)Mathf.Abs(soldierPos.x);
        nextMoveCoord.y = (int)Mathf.Abs(soldierPos.z);


        //the 'z' axis is treated as 'y' on our board, due to camera placement.
        switch (move) {
            case MovementDirections.UP:
                if (Mathf.Abs(soldierPos.z) - 1 >= TOP_BOARD_EDGE_IDX) {
                    nextMoveCoord.y -= 1;
                    isValid = true;
                }
                break;
            case MovementDirections.DOWN:
                if (Mathf.Abs(soldierPos.z) + 1 <= BTM_BOARD_EDGE_IDX) {
                    nextMoveCoord.y += 1;
                    isValid = true;
                }
                break;
            case MovementDirections.LEFT:
                if (soldierPos.x - 1 >= LEFT_BOARD_EDGE_IDX) {
                    nextMoveCoord.x -= 1;
                    isValid = true;
                }
                break;
            case MovementDirections.RIGHT:
                if (soldierPos.x + 1 <= RIGHT_BOARD_EDGE_IDX) {
                    nextMoveCoord.x += 1;
                    isValid = true;
                }
                break;
        }

        Debug.Log("IsValidMove: " + isValid);
        Debug.Log("nextMoveCoord: " + nextMoveCoord.x + "," + nextMoveCoord.y);
        return isValid;
    }

    public Point GetNextMoveCoord() {
        return nextMoveCoord;
    }
}
