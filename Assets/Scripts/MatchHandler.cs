using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * handler that is responsible for the match logic used by our model.
 * it will evaluate the scenarios presented by the model and return the call.
 */
public class MatchHandler {
    
    private static MatchHandler instance = null;
    private static readonly System.Object lockingObj = new System.Object();

    public delegate void ClickAction(GameObject obj);
    //public static event ClickAction OnClickedSoldier;
    //public static event ClickAction OnClickedTile;

    private MatchHandler() { }

    public static MatchHandler GetInstance {
        get {
            if (instance == null) {
                lock (lockingObj) {
                    instance = (instance == null) ? new MatchHandler() : instance;
                }
            }
            return instance;
        }
    }

    public MatchStatus EvaluateMatchResult(GameObject initiator, GameObject opponent) {

        Debug.Log("player=" + initiator.GetComponent<SC_Soldier>() + "\nenemy=" + opponent.GetComponent<SC_Soldier>());
        //todo: add all weapons in the future:

        switch (initiator.GetComponent<SC_Soldier>().Type) {
            case SoldierType.PITCHFORK:
                return PitchforkVs(opponent.GetComponent<SC_Soldier>().Type);
            case SoldierType.AXE:
                return AxeVs(opponent.GetComponent<SC_Soldier>().Type);
            case SoldierType.CLUB:
                return ClubVs(opponent.GetComponent<SC_Soldier>().Type);
            case SoldierType.SHIELD:
                return ShieldVs(opponent.GetComponent<SC_Soldier>().Type);
            case SoldierType.FARMER:
            case SoldierType.CRYSTAL:
                return CrystalVs(opponent.GetComponent<SC_Soldier>().Type);
        }

        return MatchStatus.UNDEFINED;
    }

    private MatchStatus ShieldVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.FARMER:
            case SoldierType.CRYSTAL:
                return MatchStatus.VICTIM_WON_THE_GAME;
            default: return MatchStatus.BOTH_LOST_MATCH;
        }
    }

    private MatchStatus PitchforkVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.PITCHFORK: return MatchStatus.TIE;
            case SoldierType.AXE: return MatchStatus.INITIATOR_WON_THE_MATCH;
            case SoldierType.CLUB: return MatchStatus.VICTIM_WON_THE_MATCH;
            case SoldierType.SHIELD: return MatchStatus.BOTH_LOST_MATCH;
            case SoldierType.CRYSTAL: return MatchStatus.INITIATOR_WON_THE_GAME;
        }
        return MatchStatus.UNDEFINED;
    }

    private MatchStatus AxeVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.PITCHFORK: return MatchStatus.VICTIM_WON_THE_MATCH;
            case SoldierType.AXE: return MatchStatus.TIE;
            case SoldierType.CLUB: return MatchStatus.INITIATOR_WON_THE_MATCH;
            case SoldierType.SHIELD: return MatchStatus.BOTH_LOST_MATCH;
            case SoldierType.CRYSTAL: return MatchStatus.INITIATOR_WON_THE_GAME;
        }
        return MatchStatus.UNDEFINED;
    }

    private MatchStatus ClubVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.PITCHFORK: return MatchStatus.INITIATOR_WON_THE_MATCH;
            case SoldierType.CLUB: return MatchStatus.TIE;
            case SoldierType.AXE: return MatchStatus.VICTIM_WON_THE_MATCH;
            case SoldierType.SHIELD: return MatchStatus.BOTH_LOST_MATCH;
            case SoldierType.CRYSTAL: return MatchStatus.INITIATOR_WON_THE_GAME;
        }
        return MatchStatus.UNDEFINED;
    }

    private MatchStatus CrystalVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.CRYSTAL:
            case SoldierType.FARMER:
                return MatchStatus.UNDEFINED;
            default: return MatchStatus.VICTIM_WON_THE_GAME;
        }
    }
}
