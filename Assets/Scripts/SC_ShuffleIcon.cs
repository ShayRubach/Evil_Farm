using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_ShuffleIcon : MonoBehaviour {

    public delegate void ShuffleClicked();
    public static event ShuffleClicked OnShuffleClicked;

    [SerializeField]
    private TextMesh text;

    private Animator animator;

	void Start () {
        text = gameObject.GetComponent<TextMesh>();
        animator = gameObject.GetComponent<Animator>();
	}

    void OnMouseDown() {
        animator.SetBool(GameModel.SHUFFLE_TRIGGER,true);
        if(OnShuffleClicked != null)
            OnShuffleClicked();

    }

    public void ResetAnimation() {
        animator.SetBool(GameModel.SHUFFLE_TRIGGER, false);
    }
}
