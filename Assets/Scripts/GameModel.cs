using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point {
    public int x { get; set; }
    public int y { get; set; }
}

/* 
 * the Board. it will hold all needed data and calculations
 * for our gameplay and be responsible for the pre-instantiations
 * of the GameObjects.
*/

public class GameModel : MonoBehaviour {

    public delegate void Announcement (SoldierTeam winner);
    public static event Announcement FinishGame;

    public delegate void NotifyToController();
    public static event NotifyToController AIMoveFinished;
    public static event NotifyToController CallTieBreaker;

    private bool playingVsAI = true;
    private static readonly int COLS = 7;
    private static readonly int ROWS = 6;

    private static readonly string UNITY_OBJECTS_TAG = "UnityObject";

    private static readonly int LEFT_BOARD_EDGE_IDX = 0;
    private static readonly int RIGHT_BOARD_EDGE_IDX = COLS - 1;
    private static readonly int TOP_BOARD_EDGE_IDX = 0;
    private static readonly int BTM_BOARD_EDGE_IDX = ROWS - 1;
    private static readonly int LEGAL_MOVES_COUNT = 4;
    private static readonly float MINIMUM_DRAG_DISTANCE = 40.0f;
    private static readonly float THINKING_TIME_IN_SECONDS = 1.0f;


    public static readonly string NO_SOLDIER_NAME_VAR = "no_soldier";
    public static readonly string PLAYER_NAME_VAR = "soldier_player";
    public static readonly string ENEMY_NAME_VAR = "soldier_enemy";
    public static readonly string TILE_NAME_VAR = "tile_";
    public static readonly string SPOTLIGHT_NAME_VAR = "spotlight";
    public static readonly string PREVIEW_SOLDIER_NAME_VAR = "preview_soldier_player";
    public static readonly string PATH_INDICATORS_NAME_VAR = "path_indicators";
    public static readonly string LEAF_INDICATOR_NAME_VAR = "leaf";
    public static readonly string TIE_WEAPONS_P_VAR_NAME = "tie_weapon_options";
    public static readonly string PREVIEW_ANIMATION_TRIGGER_PREFIX = "Preview";

    public static readonly int REVEAL_SPOTLIGHT_CHILD_IDX = 1;

    public GameObject FocusedEnemy { get; set; }
    public GameObject FocusedPlayer { get; set; }
    public MovementDirections nextMovement;

    private GameObject pathIndicators;
    private Vector3 relativePos;
    private Point nextMoveCoord;

    List<GameObject> players = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();

    private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    private void Awake() {

        GameObject[] objectsArray = GameObject.FindGameObjectsWithTag(UNITY_OBJECTS_TAG);

        foreach (GameObject obj in objectsArray) {
            try {
                objects.Add(obj.name, obj);

                //save all enemies and players for later use:
                if (obj.name.Contains(PLAYER_NAME_VAR)) {
                    players.Add(obj);
                }
                if (obj.name.Contains(ENEMY_NAME_VAR)) {
                    enemies.Add(obj);
                }
            }
            catch (ArgumentException e) {
                Debug.Log("there's already " + obj.name + " in the dictionary!");
            }
            
        }

        objects[TIE_WEAPONS_P_VAR_NAME].SetActive(false);
        pathIndicators = objects[PATH_INDICATORS_NAME_VAR];

        nextMoveCoord.x = 0;
        nextMoveCoord.y = 0;

    }

    internal void PlayAsAI() {
        Debug.Log("Playing as AI");
        
        FocusedPlayer = ChooseValidRandomSoldier();
        StartCoroutine(SimulateThinkingTime(THINKING_TIME_IN_SECONDS));
    }

    private IEnumerator SimulateThinkingTime(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        if (IsPossibleMatch(nextMoveCoord)) {
            Match();
        }
        else {
            MoveSoldier(FocusedPlayer.transform.parent.gameObject, nextMovement);
        }
        if(AIMoveFinished != null)
            AIMoveFinished();

    }

    public bool IsPossibleMatch(Point move) {
        return GetNextTileStatus() == TileStatus.VALID_OPPONENT;
    }

    private GameObject ChooseValidRandomSoldier() {

        System.Random rand = new System.Random();
        int MAX_ATTEMPTS = 50;
        int randomSoldier = 0, attempts = 0;
        FocusedPlayer = null;
        MovementDirections movement = MovementDirections.NONE;


        for (int i = 0; i < MAX_ATTEMPTS; i++) {
            //Debug.Log("ChooseValidRandomSoldier: attempts = " + i);
            randomSoldier = rand.Next(0, enemies.Count);
            FocusedPlayer = enemies[randomSoldier];
            movement = GetAvailableMove();
            
            if (movement != MovementDirections.NONE) {
                //Debug.Log("randomed " + randomSoldier + " , focusedSoldier = " + FocusedPlayer + " , move = " + movement);
                break;
            }
        }
    

        if (FocusedPlayer != null) {
            //Debug.Log("playing as AI with " + FocusedPlayer + " and he's moving " + movement);
            //save next movement for later use:
            nextMovement = movement;
        }

        return FocusedPlayer;
    }

    private MovementDirections GetAvailableMove() {
        MovementDirections[] moves = { MovementDirections.UP, MovementDirections.DOWN, MovementDirections.LEFT, MovementDirections.RIGHT };
        System.Random rand = new System.Random();
        int MAX_ATTEMPTS = 8;
        int randomMove = 0;

        //while (keepLooking) {
        for(int i=0; i < MAX_ATTEMPTS; i++) { 
            //Debug.Log("GetAvailableMove: attempts = " + i);
            randomMove = rand.Next(0, LEGAL_MOVES_COUNT);
            if(IsValidMove(FocusedPlayer.transform.position, moves[randomMove])) {
                return moves[randomMove];
            }
        }
        
        return MovementDirections.NONE;
    }

    /*
     * returns a reference to a tile by vector pos
     */
    public GameObject PointToTile(Vector3 pos) {
        float x=0, y=0;
        x = Mathf.Abs(pos.x);
        y = Mathf.Abs(pos.z);
        return objects[TILE_NAME_VAR + x + y];
    }

    public Dictionary<string,GameObject> GetObjects() {
        return objects;
    }

    public GameObject GetObject(string name) {
        return objects[name];
    }

    public MovementDirections GetSoldierMovementDirection(Vector3 startPos, Vector3 endPos) {
        relativePos = endPos - startPos;

        //Debug.Log("distance: " + Vector3.Distance(startPos, endPos));
        if (Vector3.Distance(startPos, endPos) < MINIMUM_DRAG_DISTANCE) {
            return MovementDirections.NONE;
        }
        
        float angle = Mathf.Atan2(-relativePos.y, -relativePos.x) * Mathf.Rad2Deg;
        return CalculateMovementDirectionByAngle(angle);
    }

    private MovementDirections CalculateMovementDirectionByAngle(float angle) {
        MovementDirections movement = MovementDirections.NONE;

        if (angle >= -45.0f && angle <= 45.0f) {
            movement = MovementDirections.LEFT;
        }
        else if (angle >= -135.0f && angle <= -45.0f) {
            movement = MovementDirections.UP;
        }
        else if (angle >= 45.0f && angle <= 135.0f) {
            movement = MovementDirections.DOWN;
        }
        else {
            movement = MovementDirections.RIGHT;
        }


        //Debug.Log("movement direction = " + movement);
        return movement;
    }

    /*
     * this function moves the player to a new desired position and updates
     * both tile and soldier with their new references.
     * pay attention there's the 'focusedSoldierP' which is the actual soldier parent (wrapper in scene)
     * and there's the 'exactSoldierObj' which is the actual soldier GameObject and the 1st child of 'focusedSoldierP'.
     * the parent is used to move all soldier and it's children relatively on board
     */ 
    public void MoveSoldier(GameObject focusedSoldierP, MovementDirections soldierMovementDirection) {
        Debug.Log("MoveSoldier: focusedSoldierParent = " + focusedSoldierP + " , direcion = " + soldierMovementDirection);
        //start as default position just for initialization:
        Vector3 newPosition = focusedSoldierP.transform.position;
        GameObject exactSoldierObj = focusedSoldierP.transform.GetChild(0).gameObject;

        //save reference of curr tile:
        GameObject currTile = exactSoldierObj.GetComponent<SC_Soldier>().Tile;

        if(focusedSoldierP == null) {
            Debug.Log("MoveSoldier: focusedSoldierP is null.");
            return;
        }
        Debug.Log("exactSoldierObj.transform.position: " + exactSoldierObj.transform.position);

        switch (soldierMovementDirection) {
            case MovementDirections.UP:
                newPosition = new Vector3(exactSoldierObj.transform.position.x, exactSoldierObj.transform.position.y, exactSoldierObj.transform.position.z + 1);
                break;
            case MovementDirections.DOWN:
                newPosition = new Vector3(exactSoldierObj.transform.position.x, exactSoldierObj.transform.position.y, exactSoldierObj.transform.position.z - 1);
                break;
            case MovementDirections.LEFT:
                newPosition = new Vector3(exactSoldierObj.transform.position.x - 1, exactSoldierObj.transform.position.y, exactSoldierObj.transform.position.z);
                break;
            case MovementDirections.RIGHT:
                newPosition = new Vector3(exactSoldierObj.transform.position.x + 1, exactSoldierObj.transform.position.y, exactSoldierObj.transform.position.z);
                break;
        }

        //get the new tile by new position
        GameObject newTile = PointToTile(newPosition);
        Debug.Log("newTile to move to is " + newTile.name);

        ResetTileReference(currTile);
        UpdateTileAndSoldierRefs(newTile, exactSoldierObj, true, false);

        //physically move the soldier
        exactSoldierObj.transform.position = newPosition;
    }

    internal string GetPreviewAnimationTriggerByWeapon(SoldierType weapon) {

        //make the weapon string have first upper case letter to avoid naming conflicts with unity:
        string fixedWeaponStr = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weapon.ToString().ToLower());
        return PREVIEW_ANIMATION_TRIGGER_PREFIX + fixedWeaponStr;
    }

    private bool ResetTileReference(GameObject tile) {
        return UpdateTileAndSoldierRefs(tile, null, false, true);
    }

    private bool UpdateTileAndSoldierRefs(GameObject tile, GameObject soldier, bool occupied, bool traversal) {
        if(tile == null) {
            Debug.Log("tile is null");
            return false;
        }

        

        tile.GetComponent<SC_Tile>().soldier = soldier;
        tile.GetComponent<SC_Tile>().IsOcuupied = occupied;
        tile.GetComponent<SC_Tile>().IsTraversal = traversal;

        //if soldier is null, it means we only want to lose reference to a soldier and NOT update new soldier with this current tile.
        //else, we wish to update both tile and soldier with their references.
        if(soldier != null) {
            soldier.GetComponent<SC_Soldier>().Tile = tile;
            Debug.Log("UpdateTileAndSoldierRefs: called with soldier=" + soldier + " and tile=" + tile);
        }
        else {
            Debug.Log("UpdateTileAndSoldierRefs: clearing tile=" + tile);
        }

        
        return true;
    }

    public void Match() {
        Debug.Log("Starting Match...");

        //call our MatchHandler to evaluate the match result:
        MatchStatus result = MatchHandler.GetInstance.EvaluateMatchResult(FocusedPlayer, FocusedEnemy);
        Debug.Log("match status = " + result);

        HandleMatchResult(result);
    }

    /*
     * take the necessary actions by the result: remove losing soldier, call MoveSoldier() ,update new references etc..
     */
        private void HandleMatchResult(MatchStatus result) {
        
        //get the initiator's movement direction:
        MovementDirections direction = CalculateMovementDirectionByAngle(Mathf.Atan2(-relativePos.y, -relativePos.x) * Mathf.Rad2Deg);

        Debug.Log("FocusedPlayer = " + FocusedPlayer.name);
        Debug.Log("FocusedEnemy  = " + FocusedEnemy.name);

        switch (result) {
            case MatchStatus.INITIATOR_WON_THE_MATCH:
                RemoveSoldier(FocusedEnemy);
                RevealSoldier(FocusedPlayer);
                break;
            case MatchStatus.INITIATOR_REVEALED:
                RemoveSoldier(FocusedEnemy);
                RevealSoldier(FocusedPlayer);
                break;
            case MatchStatus.VICTIM_WON_THE_MATCH:
                RemoveSoldier(FocusedPlayer);
                RevealSoldier(FocusedEnemy);
                break;
            case MatchStatus.VICTIM_REVEALED:
                RemoveSoldier(FocusedPlayer);
                RevealSoldier(FocusedEnemy);
                break;
            case MatchStatus.BOTH_LOST_MATCH:
                RemoveSoldier(FocusedPlayer);
                RemoveSoldier(FocusedEnemy);
                break;
            case MatchStatus.TIE:
                //todo: implement a rematch
                TieBreaker();
                break;
            case MatchStatus.INITIATOR_WON_THE_GAME:
                CallFinishGame(FocusedPlayer);
                break;
            case MatchStatus.VICTIM_WON_THE_GAME:
                CallFinishGame(FocusedEnemy);   
                break;
        }

    }

    private void TieBreaker() {
        if (playingVsAI) {
            SoldierType newWeapon = GetRandomWeapon();
            GetAISoldier().GetComponent<SC_Soldier>().RefreshWeapon(newWeapon);

        }

        if (CallTieBreaker != null)
            CallTieBreaker();
    }

    private GameObject GetAISoldier() {
        return FocusedEnemy.GetComponent<SC_Soldier>().Team == SoldierTeam.ENEMY ? FocusedEnemy : FocusedPlayer;
    }

    private SoldierType GetRandomWeapon() {
        SoldierType[] weapons = { SoldierType.AXE, SoldierType.CLUB, SoldierType.PITCHFORK };
        int rand = new System.Random().Next(0, weapons.Length);

        Debug.Log("randomed " + weapons[rand]);

        return weapons[rand];
    }

    private MovementDirections ReverseDirection(MovementDirections direction) {
        switch (direction) {
            case MovementDirections.UP: return MovementDirections.DOWN;
            case MovementDirections.DOWN: return MovementDirections.UP;
            case MovementDirections.RIGHT: return MovementDirections.LEFT;
            case MovementDirections.LEFT: return MovementDirections.RIGHT;
        }
        return MovementDirections.NONE;
    }

    private void RevealSoldier(GameObject soldier) {
        if (soldier.GetComponent<SC_Soldier>().Team == SoldierTeam.PLAYER)
            TurnOnRevealSpotlight(soldier);
        else
            soldier.GetComponent<SC_Soldier>().RevealWeapon();
    }

    private void TurnOnRevealSpotlight(GameObject soldier) {
        soldier.transform.GetChild(REVEAL_SPOTLIGHT_CHILD_IDX).gameObject.SetActive(true);
    }

    private void HideSoldier(GameObject soldier) {
        return;
        soldier.GetComponent<SC_Soldier>().ConcealWeapon(soldier.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);
    }

    private void RemoveSoldier(GameObject soldier) {
        Debug.Log("removing " + soldier);
        ResetTileReference(soldier.GetComponent<SC_Soldier>().Tile);
        soldier.SetActive(false);
    }

    void CallFinishGame(GameObject winner) {
        SoldierTeam winningTeam = winner.GetComponent<SC_Soldier>().Team;

        if (FinishGame != null)
            FinishGame(winningTeam);
    }

    private MatchStatus EvaluateMatchResult() {

        return MatchStatus.TIE;
    }

    public void ShowPathIndicators(Vector3 objectPos) {
        ResetIndicators();                                      //enable and show all indicators.
        pathIndicators.transform.position = objectPos;          //move all indicators so they surround the object.
        FilterIndicators(objectPos);                            //hide non travesal indicators.
        
    }

    private void ResetIndicators() {

        pathIndicators.SetActive(true);

        for (int i = 0; i < pathIndicators.transform.childCount; ++i) {
            pathIndicators.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public TileStatus GetNextTileStatus() {
        //calculate new tile requested
        GameObject tile = objects[TILE_NAME_VAR + nextMoveCoord.x + nextMoveCoord.y];

        //save reference to the opponent for easier access later from the controller:
        FocusedEnemy = tile.GetComponent<SC_Tile>().soldier;

        if (FocusedEnemy != null) {
            //next tile is occupied with a soldier
            if(FocusedEnemy.GetComponent<SC_Soldier>().Team != FocusedPlayer.GetComponent<SC_Soldier>().Team) {   
                return TileStatus.VALID_OPPONENT;
            }
        }
        
        return TileStatus.TRV_TILE;
    }

    /*
     * used to decide which indicators (right,left,up,down) indicators are eligible to be displayed
     * according to the position of the soldier
    */
    private void FilterIndicators(Vector3 pos) {
        Vector3 requestedTilePos;

        //soldier is located in most left side of the border
        requestedTilePos = new Vector3(pos.x-1, pos.y, pos.z);
        if (Mathf.Abs(pos.x) == LEFT_BOARD_EDGE_IDX || RequestTileIsOccupied(PointToTile(requestedTilePos))) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.LEFT).gameObject);
        }

        //soldier is located in most right side of the border
        requestedTilePos = new Vector3(pos.x + 1, pos.y, pos.z);
        if (Mathf.Abs(pos.x) == RIGHT_BOARD_EDGE_IDX || RequestTileIsOccupied(PointToTile(requestedTilePos))) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.RIGHT).gameObject);
        }

        //soldier is located in the top side of the border
        requestedTilePos = new Vector3(pos.x, pos.y, pos.z + 1);
        if (Mathf.Abs(pos.z) == TOP_BOARD_EDGE_IDX || RequestTileIsOccupied(PointToTile(requestedTilePos))) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.UP).gameObject);
        }

        //soldier is located in the bottom side of the border
        requestedTilePos = new Vector3(pos.x, pos.y, pos.z - 1);
        if (Mathf.Abs(pos.z) == BTM_BOARD_EDGE_IDX || RequestTileIsOccupied(PointToTile(requestedTilePos))) {
            HideObjectUnderBoard(pathIndicators.transform.GetChild((int)Indicators.DOWN).gameObject);
        }
    }

    private bool RequestTileIsOccupied(GameObject tile) {
        return tile.GetComponent<SC_Tile>().IsOcuupied;
    }

    private void HideObjectUnderBoard(GameObject obj) {
        //obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 1, obj.transform.position.z);
        obj.SetActive(false);
    }

    public void HidePathIndicators() {
        HideObjectUnderBoard(pathIndicators);
    }

    public bool IsValidMove(Vector3 soldierPos, MovementDirections move) {

        bool isValid = false;
        nextMoveCoord.x = (int)Mathf.Abs(soldierPos.x);
        nextMoveCoord.y = (int)Mathf.Abs(soldierPos.z);


        //the 'z' axis is treated as 'y' on our board, due to camera placement.
        switch (move) {
            case MovementDirections.UP:
                if (Mathf.Abs(soldierPos.z) - 1 >= TOP_BOARD_EDGE_IDX) {
                    nextMoveCoord.y -= 1;
                    if(!OverlayingTeamMember(nextMoveCoord))
                        isValid = true;
                }
                break;
            case MovementDirections.DOWN:
                if (Mathf.Abs(soldierPos.z) + 1 <= BTM_BOARD_EDGE_IDX) { // && !OverlayingTeamMember(nextMoveCoord)) {
                    nextMoveCoord.y += 1;
                    if (!OverlayingTeamMember(nextMoveCoord))
                        isValid = true;
                }
                break;
            case MovementDirections.LEFT:
                if (soldierPos.x - 1 >= LEFT_BOARD_EDGE_IDX) { // && !OverlayingTeamMember(nextMoveCoord)) {
                    nextMoveCoord.x -= 1;
                    if (!OverlayingTeamMember(nextMoveCoord))
                        isValid = true;
                }
                break;
            case MovementDirections.RIGHT:
                if (soldierPos.x + 1 <= RIGHT_BOARD_EDGE_IDX) { // && !OverlayingTeamMember(nextMoveCoord)) {
                    nextMoveCoord.x += 1;
                    if (!OverlayingTeamMember(nextMoveCoord))
                        isValid = true;
                }
                break;
        }

        if (isValid)
            ;// Debug.Log("nextMoveCoord = " + nextMoveCoord.x + "," + nextMoveCoord.y);

        return isValid;
    }

    private bool OverlayingTeamMember(Point moveCoord) {
        SC_Tile nextTile = objects[TILE_NAME_VAR + moveCoord.x + moveCoord.y].GetComponent<SC_Tile>();

        Debug.Log(FocusedPlayer + " wants to move to " + nextTile.name);

        if (nextTile.IsOcuupied) {
            //if the next tile has one of our team members, restirct movement:
            //Debug.Log("nextTile.soldier.GetComponent<SC_Soldier>().Team = " + nextTile.soldier.GetComponent<SC_Soldier>().Team);
            //Debug.Log("FocusedPlayer.GetComponent<SC_Soldier>().Team = " + FocusedPlayer.GetComponent<SC_Soldier>().Team);

            if(nextTile.soldier.GetComponent<SC_Soldier>().Team == FocusedPlayer.GetComponent<SC_Soldier>().Team) {
                Debug.Log(FocusedPlayer + " can NOT move there");
                return true;
            }
        }
        Debug.Log(FocusedPlayer + " can move there");
        return false;
    }

    public Point GetNextMoveCoord() {
        return nextMoveCoord;
    }

    public void GodMode(bool state) {
        foreach(KeyValuePair<string,GameObject> soldier in objects) {
            if (soldier.Value.name.Contains(ENEMY_NAME_VAR)) {
                if (!state)
                    RevealSoldier(soldier.Value);
                else
                    HideSoldier(soldier.Value);
            }
        }
        
    }
}
