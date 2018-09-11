using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_CountdownTimer : MonoBehaviour {

    public delegate void PreparationTimeEvent();
    public static event PreparationTimeEvent PreparationTimeOver;
    public static event PreparationTimeEvent PreparationTimeStarted;

    [SerializeField]
    private int countdownTime = 10;

    private int timeLeft;

    private TextMesh countdownText;

    // Use this for initialization
    void Start() {

    }
    
    void OnEnable() {
        if (PreparationTimeStarted != null) {
            PreparationTimeStarted();
        }
        timeLeft = countdownTime;
        countdownText = gameObject.GetComponent<TextMesh>();
        StartCoroutine(LoseTime());
    }

    // Update is called once per frame
    void Update() {
        countdownText.text = (timeLeft.ToString());

        if (timeLeft <= 0) {
            //StopCoroutine(LoseTime());
            timeLeft = countdownTime;
            countdownText.text = "Times Up!";
            if (PreparationTimeOver != null)
                PreparationTimeOver();
            gameObject.SetActive(false);
        }
    }

    IEnumerator LoseTime() {
        while (true) {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
    }
}
