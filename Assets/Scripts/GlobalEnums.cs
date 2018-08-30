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

public enum Clickables {
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
    GUARDIAN,   //kamikazee . kill himself and his rival
    SCARECROW,  //dummy player - only revelas his rival
    FARMER      //king
}