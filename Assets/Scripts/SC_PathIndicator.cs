using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PathIndicator : MonoBehaviour {
    private Ray ray;
    private RaycastHit hit;
    private static float spinSpeed = 15.0f;
    private bool shouldSpin = false;
    private GameObject leafObj = null;
    private Quaternion originalRotation;

    void Start() {
        Debug.Log("called from " + gameObject);
        originalRotation = transform.rotation;
    }

    void FixedUpdate() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {

            shouldSpin = HoveredOverIndicator(hit.collider.name);
        
            if (shouldSpin) 
                SpinLeaf();
            else
                ResetLeafRotation();
        }
    }

    private bool HoveredOverIndicator(string objHitName) {
        return (objHitName == gameObject.name);
    }

    private void ResetLeafRotation() {
        transform.rotation = originalRotation;
    }

    private void SpinLeaf() {
        transform.Rotate(Vector3.forward * spinSpeed);
    }

}
