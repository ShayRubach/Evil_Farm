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
    public static event ClickAction OnClickedSoldier;
    public static event ClickAction OnClickedTile;

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

    public MatchStatus EvaluateMatchResult(GameObject player, GameObject enemy) {
        Debug.Log("EvaluateMatchResult: called");
        Debug.Log(player.name + ": " + player.GetComponent<SC_Soldier>());
        Debug.Log(enemy.name + ": " + enemy.GetComponent<SC_Soldier>());

        //todo: add all weapons in the future:

        switch (player.GetComponent<SC_Soldier>().Type) {
            case SoldierType.PITCHFORK:
                return PitchforkVs(enemy.GetComponent<SC_Soldier>().Type);
            case SoldierType.AXE:
                return AxeVs(enemy.GetComponent<SC_Soldier>().Type);
            case SoldierType.CLUB:
                return ClubVs(enemy.GetComponent<SC_Soldier>().Type);
            case SoldierType.FARMER:
            case SoldierType.CRYSTAL:
                return CrystalVs(enemy.GetComponent<SC_Soldier>().Type);
        }

        return MatchStatus.UNDEFINED;
    }

    private MatchStatus PitchforkVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.PITCHFORK: return MatchStatus.TIE;
            case SoldierType.AXE: return MatchStatus.PLAYER_WON_THE_MATCH;
            case SoldierType.CLUB: return MatchStatus.ENEMY_WON_THE_MATCH;
            case SoldierType.CRYSTAL: return MatchStatus.PLAYER_WON_THE_GAME;
        }
        return MatchStatus.UNDEFINED;
    }

    private MatchStatus AxeVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.PITCHFORK: return MatchStatus.ENEMY_WON_THE_MATCH;
            case SoldierType.AXE: return MatchStatus.TIE;
            case SoldierType.CLUB: return MatchStatus.PLAYER_WON_THE_MATCH;
            case SoldierType.CRYSTAL: return MatchStatus.PLAYER_WON_THE_GAME;
        }
        return MatchStatus.UNDEFINED;
    }

    private MatchStatus ClubVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.PITCHFORK: return MatchStatus.PLAYER_WON_THE_MATCH;
            case SoldierType.CLUB: return MatchStatus.TIE;
            case SoldierType.AXE: return MatchStatus.ENEMY_WON_THE_MATCH;
            case SoldierType.CRYSTAL: return MatchStatus.PLAYER_WON_THE_GAME;
        }
        return MatchStatus.UNDEFINED;
    }

    private MatchStatus CrystalVs(SoldierType enemyType) {
        switch (enemyType) {
            case SoldierType.CRYSTAL:
            case SoldierType.FARMER:
                return MatchStatus.UNDEFINED;
            default: return MatchStatus.ENEMY_WON_THE_GAME;
        }
    }
}
