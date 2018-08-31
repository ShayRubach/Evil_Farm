using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PathIndicator : MonoBehaviour {
    private Ray ray;
    private RaycastHit hit;
    private static float spinSpeed = 10.0f;
    private bool spin = false;
    private GameObject leafIndicatorsParent = null;
    private GameObject leafObj = null;
    private Animator leaf_path_indicator_animator;
    private Quaternion originalRotation;

    void Start() {
        leafIndicatorsParent = GameObject.Find("path_indicators");
        originalRotation = transform.rotation;
    }

    void FixedUpdate() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            string objHitName = hit.collider.name;

            if (objHitName == gameObject.name) {
                Debug.Log("hit " + gameObject.name);
                leafObj = hit.collider.gameObject;
                spin = true;
            }
            else {
                spin = false;
            }

            if (spin) {
                SpinLeaf(leafObj);
            }
            else {
                ResetLeafRotation(leafObj);
            }
        }
    }

    private void ResetLeafRotation(GameObject leaf) {
        if (leaf) {
            transform.rotation = originalRotation;
        }
    }

    private void SpinLeaf(GameObject leaf) {
        if(leaf)
            transform.Rotate(Vector3.forward * spinSpeed);
    }

}
