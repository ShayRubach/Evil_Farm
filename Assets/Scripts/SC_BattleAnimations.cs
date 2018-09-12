using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_BattleAnimations : MonoBehaviour {

    public delegate void NotifyToController();
    public static event NotifyToController BattleAnimationFinish;

    public void OnBattleAnimationFinish() {
        if(BattleAnimationFinish != null) {
            BattleAnimationFinish();
        }
    }
}
