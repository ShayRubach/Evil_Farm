using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CoinSpawner : MonoBehaviour {


    private static SC_CoinSpawner instance = null;
    private static readonly Object lockingObj = new Object();
    private bool canMove = true;
    private float rotateSpeed = 5.0f;
    private float moveSpeed = 5.0f;

    public Vector3 startPos;

    private SC_CoinSpawner() {}

    public static SC_CoinSpawner GetInstance {
        get {
            if (instance == null) {
                lock (lockingObj) {
                    instance = (instance == null) ? new SC_CoinSpawner() : instance;
                }
            }
            return instance;
        }
    }

    public void SpawnCoins() {
        Debug.Log(this);
    }

    void OnTriggerEnter2D() {
        transform.localPosition = startPos;
    }

	void FixedUpdate () {
        if (canMove) {
            transform.Translate(Vector3.down * moveSpeed);
            transform.Rotate(Vector3.down * rotateSpeed);
        }
	}

}
