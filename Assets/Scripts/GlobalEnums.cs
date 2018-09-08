using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementDirections {
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3,
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
    PLAYER,
    NO_TEAM
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
    SHIELD,     // kamikazee . kill himself and his rival
    SCARECROW,  // dummy player - only revelas his rival
    FARMER,     // king
    CRYSTAL,    // king
    NO_TYPE
}

public enum MatchStatus {
    INITIATOR_WON_THE_MATCH,    // initiator won this match
    INITIATOR_WON_THE_GAME,     // initiator found the farmer (king)
    INITIATOR_REVEALED,         // initiator has faced the scarecrow (dummy)
    VICTIM_WON_THE_MATCH,       // victim won this match
    VICTIM_WON_THE_GAME,        // victim found the farmer (king)
    VICTIM_REVEALED,            // victim has faced the scarecrow (dummy)
    BOTH_LOST_MATCH,            // facing a shield soldier, both are eliminated
    TIE,                        // both soldiers had the same weapon - forces rematch
    UNDEFINED                   // still not set
}