using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CloudsMovement : MonoBehaviour {

    public GameObject clouds;
    private float movementSpeed = 0.2f;
    private float leftBoarderLimit = -5.0f;
    private Vector3 basePosition;

    void Start() {
        basePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
    }

    void Update() {
        clouds.transform.Translate(Vector3.left * Time.deltaTime * movementSpeed);
        
        if(clouds.transform.position.x < leftBoarderLimit) {
            transform.position = basePosition;
            //Debug.Log(clouds.transform.position.x);
        }
    }
}
