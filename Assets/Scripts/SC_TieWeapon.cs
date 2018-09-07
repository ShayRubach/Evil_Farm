using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TieWeapon : MonoBehaviour {

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
        Debug.Log("PICKED TIE WEAPON = " + gameObject);
    }
}
