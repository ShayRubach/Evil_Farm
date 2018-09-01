using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Tile : MonoBehaviour {
    
    public GameObject soldier;
	public bool ShowSoldier    { get; set; }
    public bool IsOcuupied     { get; set; }
    public bool IsTraversal    { get; set; }

    void Start() {
        if(soldier.name == GameModel.NO_SOLDIER_NAME_VAR) {
            soldier = null;
            IsOcuupied = false;
            IsTraversal = true;
        }
        else {
            IsOcuupied = true;
            IsTraversal = false;

            //assign this tile to its sodlier
            soldier.GetComponent<SC_Soldier>().Tile = gameObject;
        }
    }

    public SC_Soldier GetCurrSoldier() {
        return soldier.GetComponent<SC_Soldier>();
    }
    
}
