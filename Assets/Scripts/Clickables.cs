using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clickables {
    PLAYER_SOLDIER,     //soldier of our own team 
    ENEMY_SOLDIER,      //soldier of the rival team
    VALID_OPPONENT,     //soldier of the rival team which is valid to start a fight with (close to our current soldier within a distance of 1 tile)
    NON_TRV_TILE,       //a non-traversable tile to move to
    TRV_TILE            //a traversable tile to move to
}
