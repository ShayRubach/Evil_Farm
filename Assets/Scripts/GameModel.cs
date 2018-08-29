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

    public static readonly string PLAYER_NAME_VAR = "soldier";
    public static readonly string TILE_NAME_VAR = "tile_";
    public static readonly string SPOTLIGHT_NAME_VAR = "spotlight";
    public static readonly string DUEL_SOLDIER_NAME_VAR = "duel_soldier_player";
    public static readonly string PATH_INDICATORS_NAME_VAR = "path_indicators";

    private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    private void Awake() {
        GameObject[] objectsArray = GameObject.FindGameObjectsWithTag(UNITY_OBJECTS_TAG);

        foreach (GameObject obj in objectsArray) {
            objects.Add(obj.name, obj);
        }
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

        //Debug.Log("direction angle is = " + angle);
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
        float x = 0, y = 0;
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
}
