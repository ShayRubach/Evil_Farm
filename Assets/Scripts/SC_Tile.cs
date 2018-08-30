using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Tile : MonoBehaviour {
    
    public GameObject soldier;
    public bool ShowSoldier { get; set; }

    private bool IsOcuupied { get; set; }
    private bool IsTraversal { get; set; }

    void Start() {
        if(soldier.name == GameModel.NO_SOLDIER_NAME_VAR) {
            soldier = null;
            IsOcuupied = false;
            IsTraversal = true;
        }
        else {
            IsOcuupied = true;
            IsTraversal = false;
        }
    }

    public SC_Soldier GetCurrSoldier() {
        return IsOcuupied ? soldier.GetComponent<SC_Soldier>() : null ;   
    }





}
