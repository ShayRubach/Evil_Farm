using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Tile : MonoBehaviour {


    public GameObject soldier = null;
    public bool showSoldier = true;

    private bool isOcuupied = false;
    private bool isTraversal = true;


    // Use this for initialization
    void Start () {
        if(soldier != null) {
            Debug.Log("I am " + this + " and I have " + soldier);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {

    }







}
