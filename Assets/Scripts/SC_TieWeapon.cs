using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TieWeapon : MonoBehaviour {

    public delegate void NotifyToController(SoldierType newWeapon);
    public static event NotifyToController OnNewWeaponChoice;

    private Animator animator;
    public static readonly string TRIGGER_NAME = "Hovered";

    // Use this for initialization
    void Start () {
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool(TRIGGER_NAME, false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseOver() {
        animator.SetBool(TRIGGER_NAME, true);
    }

    void OnMouseExit() {
        animator.SetBool(TRIGGER_NAME, false);
    }

    void OnMouseDown() {
        if (OnNewWeaponChoice != null)
            OnNewWeaponChoice(GetClickedWeapon());
    }

    private SoldierType GetClickedWeapon() {
        if(gameObject.name.Contains(SoldierType.AXE.ToString().ToLower())) {
            return SoldierType.AXE;
        }
        if (gameObject.name.Contains(SoldierType.CLUB.ToString().ToLower())) {
            return SoldierType.CLUB;
        }
        if (gameObject.name.Contains(SoldierType.PITCHFORK.ToString().ToLower())) {
            return SoldierType.PITCHFORK;
        }

        return SoldierType.NO_TYPE;
    }
}
