using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EndOptions : MonoBehaviour {

    public delegate void EndGameOptions(EndGameOption choice);
    public static event EndGameOptions OnEndGameOptionChoice;
    private readonly string RESTART_STR_NAME = "restart";

    void OnMouseDown() {
        
        EndGameOption choice = gameObject.name.Contains(RESTART_STR_NAME) ? EndGameOption.RESTART : EndGameOption.BACK_TO_MENU;

        if (OnEndGameOptionChoice != null)
            OnEndGameOptionChoice(choice);
    }
}
