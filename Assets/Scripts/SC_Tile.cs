using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Tile : MonoBehaviour {
    
    public GameObject soldier;
    public bool IsOcuupied     { get; set; }
    public bool IsTraversal    { get; set; }

    private GameObject initialSoldier = null;

    void Start() {
        //saving the initial soldier for game restart optimizations:
        initialSoldier = soldier;

        Init();
    }

    public void Init() {
        if (initialSoldier.name == GameModel.NO_SOLDIER_NAME_VAR) {
            soldier = null;
            IsOcuupied = false;
            IsTraversal = true;
        }
        else {
            IsOcuupied = true;
            IsTraversal = false;

            //soldier is valid, set it to be the initial soldier when game starts:
            soldier = initialSoldier;

            //assign this tile to its sodlier
            soldier.GetComponent<SC_Soldier>().Tile = gameObject;
        }
    }

    public SC_Soldier GetCurrSoldier() {
        return soldier.GetComponent<SC_Soldier>();
    }

    public override string ToString() {
        return "IsOccupied = " + IsOcuupied + " | "
            + "IsTraversal = " + IsTraversal + " | "
            + "soldier = " + soldier.name;
    }
}
