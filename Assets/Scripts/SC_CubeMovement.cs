using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CubeMovement : MonoBehaviour {

    public GameObject cube;
    public GameObject weapon;
    private Animator cubeAnimator;

    public float tumblingDuration = 0.2f;
    bool goingUp = true;


    void Awake() {
        cubeAnimator = GetComponent<Animator>();
    }

    void Start() {

    }


    void Update() {


        //var dir = Vector3.zero;

        //if (Input.GetKey(KeyCode.UpArrow))
        //    dir = Vector3.forward;

        //if (Input.GetKey(KeyCode.DownArrow))
        //    dir = Vector3.back;

        //if (Input.GetKey(KeyCode.LeftArrow))
        //    dir = Vector3.left;

        //if (Input.GetKey(KeyCode.RightArrow))
        //    dir = Vector3.right;

        //if (dir != Vector3.zero && !isTumbling) {
        //    transform.Rotate(0, 0, 0);
        //    StartCoroutine(Tumble(dir));
        //}

        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    cubeAnimator.SetBool("isMoving", !(cubeAnimator.GetBool("isMoving")));
        //}

        //if (cubeAnimator.GetBool("isMoving")) {
        //    Debug.Log("isMoving");
        //}
    }

    bool isTumbling = false;
    IEnumerator Tumble(Vector3 direction) {
        isTumbling = true;

        var rotAxis = Vector3.Cross(Vector3.up, direction);
        var pivot = (transform.position + Vector3.down * 0.5f) + direction * 0.5f;

        var startRotation = transform.rotation;
        var endRotation = Quaternion.AngleAxis(90.0f, rotAxis) * startRotation;

        var startPosition = transform.position;
        var endPosition = startPosition + direction;

        var rotSpeed = 90.0f / tumblingDuration;
        var t = 0.0f;

        while (t < tumblingDuration) {
            t += Time.deltaTime;
            transform.RotateAround(pivot, rotAxis, rotSpeed * Time.deltaTime);
            weapon.transform.Translate(direction * 0.08f);
            yield return null;
        }

        

        transform.rotation = endRotation;
        transform.position = endPosition;



        isTumbling = false;
    }

}
