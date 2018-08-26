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

    private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    private void Awake() {
        GameObject[] objectsArray = GameObject.FindGameObjectsWithTag(UNITY_OBJECTS_TAG);

        foreach (GameObject obj in objectsArray) {
            objects.Add(obj.name, obj);
        }
    }
    
    public SC_Tile PointToTile(float x, float y) {
        
        return null;
    }

    public Dictionary<string,GameObject> GetObjects() {
        return objects;
    }

    public GameObject GetObject(string name) {
        return objects[name];
    }

}
