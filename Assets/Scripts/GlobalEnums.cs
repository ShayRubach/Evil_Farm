using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementDirections {
    UP,
    DOWN,
    LEFT,
    RIGHT,
    NONE
}

public enum Indicators {
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public enum SoldierTeam {
    ENEMY,
    PLAYER
}

public enum TileStatus {
    PLAYER_SOLDIER,     //soldier of our own team 
    ENEMY_SOLDIER,      //soldier of the rival team
    VALID_OPPONENT,     //soldier of the rival team which is valid to start a fight with (close to our current soldier within a distance of 1 tile)
    NON_TRV_TILE,       //a non-traversable tile to move to
    TRV_TILE            //a traversable tile to move to
}

public enum SoldierType {
    PITCHFORK,  // pitchfork > axe
    CLUB,       // club > pitchfork
    AXE,        // axe > club
    SCYTHE,     // scythe == throwing weapon??
    SHIELD,     //kamikazee . kill himself and his rival
    SCARECROW,  //dummy player - only revelas his rival
    FARMER,     //king
    NO_TYPE
}

public enum MatchStatus {
    PLAYER_WON_THE_MATCH,   //player won this match
    PLAYER_WON_THE_GAME,    //player found the farmer (king)
    PLAYER_REVEALED,        //player has faced the scarecrow (dummy)
    ENEMY_WON_THE_MATCH,    //enemy won this match
    ENEMY_WON_THE_GAME,     //enemy found the farmer (king)
    ENEMY_REVEALED,         //enemy has faced the scarecrow (dummy)
    TIE,                    //both soldiers had the same weapon - forces rematch
    UNDEFINED
}