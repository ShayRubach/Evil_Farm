using System;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * the MVC's controller for our app.
 * it will pass commands to logic and view.
 */

public class SC_GameController : MonoBehaviour {

    private GameObject focusedPlayerParent;                 //parent GameObject that holds the soldier and its weapon
    private GameObject previewSoldierPlayer;                //the enlarged previewed soldier user sees on a soldier focus (click)
    private GameModel model;                                //our game logic model

    [SerializeField]
    private GameObject countdownManager;
    [SerializeField]
    private GameObject shuffleHandler;


    
    private SC_Spotlight soldierSpotlight;                  //highlights the rival on drag
    private Vector3 nextPosition, startDragPos, endDragPos;
    private Animator soldierAnimator, previewSoldierAnimator;
    private Animator announcerAnimator, endGameOptionsAnimator;
    private Animator battleAnimator;


    private bool isMyTurn = true;
    private bool duringTie = false;
    private bool canPlay = false;
    private bool AIAlreadyPlaying = false;


    private MovementDirections soldierMovementDirection;
    private string previewAnimationTrigger = "";



    void Start() {

        //get reference to our model class
        model = GameObject.Find(GameModel.GAME_MODEL_NAME_VAR).GetComponent<GameModel>();

        //save references for some general objects we need to control
        soldierSpotlight = model.GetObjects()[GameModel.SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        announcerAnimator = model.GetObjects()[GameModel.ANNOUNCER_VAR_NAME].GetComponent<Animator>();
        endGameOptionsAnimator = model.GetObjects()[GameModel.END_GAME_OPTIONS_VAR_NAME].GetComponent<Animator>();
        battleAnimator = model.GetObjects()[GameModel.BATTLE_ANIMATOR_VAR_NAME].GetComponent<Animator>();

        previewSoldierPlayer = model.GetObjects()[GameModel.PREVIEW_SOLDIER_NAME_VAR];
        previewSoldierAnimator = previewSoldierPlayer.GetComponent<Animator>();

        Init();
    }

    private void Init() {
        HidePreviewSoldier();
        countdownManager.SetActive(true);
        shuffleHandler.SetActive(true);
        //battleAnimator.gameObject.SetActive(false);
    }

    void FixedUpdate() {

        //for debugging purposes only:
        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (System.Collections.Generic.KeyValuePair<string, GameObject> obj in model.GetObjects()) {
                if (obj.Value.name.Contains("tile")) {
                    if (obj.Value.GetComponent<SC_Tile>().soldier != null)
                        Debug.Log(obj.Value.name + " has " + obj.Value.GetComponent<SC_Tile>().soldier);
                }

            }
        }

        if(!isMyTurn && !duringTie && canPlay) {
            if (!AIAlreadyPlaying)
                PlayAsAI();
        }
    }

    private void PlayAsAI() {
        AIAlreadyPlaying = true;
        model.PlayAsAI();
        //isMyTurn = true;
    }

    IEnumerator Fade() {
        GameObject leaf = GameObject.Find("leaf_test");
        Renderer sr = leaf.GetComponent<Renderer>();

        for (float f = 1f; f >= 0; f -= 0.1f) {
            Color c = sr.material.color;
            c.a = f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnEnable() {
        GameModel.FinishGame += FinishGame;
        GameModel.AIMoveFinished += AIMoveFinished;
        GameModel.CallTieBreaker += TieBreaker;
        GameModel.OnMatchStarted += OnMatchStarted;
        GameModel.OnMatchFinished += OnMatchFinished;
        SC_Soldier.OnStartDragging += OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging += OnFinishDraggingSoldier;
        SC_Soldier.OnSoldierMovementAnimationEnd += OnSoldierMovementAnimationEnd;
        SC_Soldier.MarkSoldier += MarkSoldier;
        SC_Soldier.UnmarkSoldier += UnmarkSoldier;
        SC_TieWeapon.OnNewWeaponChoice += OnNewWeaponChoice;
        SC_AnnouncerManager.FinishAnnouncementEvent += FinishAnnouncementEvent;
        SC_AnnouncerManager.StartAnnouncementEvent += StartAnnouncementEvent;
        SC_EndOptions.OnEndGameOptionChoice += OnEndGameOptionChoice;
        SC_CountdownTimer.PreparationTimeOver += PreparationTimeOver;
        SC_CountdownTimer.PreparationTimeStarted += PreparationTimeStarted;
        SC_ShuffleIcon.OnShuffleClicked += OnShuffleClicked;
        SC_BattleAnimations.BattleAnimationFinish += OnBattleAnimationFinish;
        SC_Cart.GodMode += GodMode;
    }

    void OnDisable() {
        GameModel.FinishGame -= FinishGame;
        GameModel.AIMoveFinished -= AIMoveFinished;
        GameModel.CallTieBreaker -= TieBreaker;
        GameModel.OnMatchStarted -= OnMatchStarted;
        GameModel.OnMatchFinished -= OnMatchFinished;
        SC_Soldier.UnmarkSoldier -= UnmarkSoldier;
        SC_Soldier.OnStartDragging -= OnStartDraggingSoldier;
        SC_Soldier.OnFinishDragging -= OnFinishDraggingSoldier;
        SC_Soldier.OnSoldierMovementAnimationEnd -= OnSoldierMovementAnimationEnd;
        SC_Soldier.MarkSoldier -= MarkSoldier;
        SC_TieWeapon.OnNewWeaponChoice -= OnNewWeaponChoice;
        SC_AnnouncerManager.FinishAnnouncementEvent -= FinishAnnouncementEvent;
        SC_AnnouncerManager.StartAnnouncementEvent -= StartAnnouncementEvent;
        SC_EndOptions.OnEndGameOptionChoice -= OnEndGameOptionChoice;
        SC_CountdownTimer.PreparationTimeOver -= PreparationTimeOver;
        SC_CountdownTimer.PreparationTimeStarted -= PreparationTimeStarted;
        SC_ShuffleIcon.OnShuffleClicked -= OnShuffleClicked;
        SC_BattleAnimations.BattleAnimationFinish -= OnBattleAnimationFinish;
        SC_Cart.GodMode -= GodMode;
    }

    //private void OnMatchStarted(SoldierType playerType, SoldierType enemyType) {
    private void OnMatchStarted() {
        canPlay = false;
        battleAnimator.gameObject.SetActive(true);
        ResetAnimatorParameters(battleAnimator);
        EnableBattleAnimationParameters();
    }

    private void EnableBattleAnimationParameters() {
        string parameter = model.GetCurrentBattleAnimationParameters();
        
        //if game ended, dont show anymore battle animations
        if (parameter.Contains(GameModel.CRYSTAL_VAR_NAME)) {
            battleAnimator.gameObject.SetActive(false);
        }
        else {
            battleAnimator.SetBool(parameter, true);
        }
        
    }

    private void OnBattleAnimationFinish() {
        canPlay = true;
        ResetAnimatorParameters(battleAnimator);
        //battleAnimator.gameObject.SetActive(false);
    }

    private void OnShuffleClicked() {
        //todo: fix issue with tile 74 not found in FilterIndicators
        //model.ShuffleTeam(SoldierTeam.PLAYER);
    }

    private void PreparationTimeStarted() {
        canPlay = false;
    }

    private void PreparationTimeOver() {
        shuffleHandler.SetActive(false);
        canPlay = true;
    }

    private void OnEndGameOptionChoice(EndGameOption choice) {
        if(choice == EndGameOption.RESTART) {
            endGameOptionsAnimator.SetBool(GameModel.END_GAME_TRIGGER, false);
            model.RestartGame();
            Init();
        }
        else {
            SharedDataHandler.nextScreenRequested = Scenes.MainMenu.ToString();
            SceneManager.LoadScene(SC_MenuModel.SCENE_PREFIX + Scenes.Login);
        }
    }

    //do not let user interact with game during animations:
    private void StartAnnouncementEvent() {
        canPlay = false;
    }

    //let user interact with game when animation ends:
    private void FinishAnnouncementEvent() {
        ResetAnimatorParameters(announcerAnimator);
        //canPlay = true;
    }

    private void ResetAnimatorParameters(Animator animator) {

        for (int i = 0; i < animator.parameterCount ; i++) {
            //if(animator.GetParameter(i).GetType() == typeof(bool))
            animator.SetBool(animator.GetParameter(i).name, false);
        }

    }

    private void OnNewWeaponChoice(SoldierType newWeapon) {
        model.GetPlayerSoldier().GetComponent<SC_Soldier>().RefreshWeapon(newWeapon);
        ShowTieWeapons(false);

        //rematch:
        duringTie = false;
        Match();
    }

    /* 
    * callback after a match ends (without a tie)
    */
    private void OnMatchFinished(SoldierTeam matchWinningTeam) {
        if(matchWinningTeam == SoldierTeam.PLAYER)
            announcerAnimator.SetBool(GameModel.ANNOUNCER_WIN_TRIGGER, true);
        if(matchWinningTeam == SoldierTeam.ENEMY)
            announcerAnimator.SetBool(GameModel.ANNOUNCER_LOSE_TRIGGER, true);
        if(matchWinningTeam == SoldierTeam.NO_TEAM)
            announcerAnimator.SetBool(GameModel.ANNOUNCER_LOSE_TRIGGER, true);

    }

    /* 
    * callback for a tied match
    */
    private void TieBreaker() {
        announcerAnimator.SetBool(GameModel.ANNOUNCER_TIE_TRIGGER, true);
        duringTie = true;
        ShowTieWeapons(true);
    }

    private void ShowTieWeapons(bool isVisible) {
        model.GetObjects()[GameModel.TIE_WEAPONS_P_VAR_NAME].SetActive(isVisible);
    }
    
    //called when AI is finished with his move (inc. animations):
    private void AIMoveFinished() {
        Debug.Log("AIMoveFinished");
        AIAlreadyPlaying = false;
        isMyTurn = true;
    }

    private void FinishGame(SoldierTeam winner) {
        canPlay = false;
        endGameOptionsAnimator.SetBool(GameModel.END_GAME_TRIGGER, true);

        if (winner == SoldierTeam.PLAYER)
            announcerAnimator.SetBool(GameModel.ANNOUNCER_VICTORY_TRIGGER, true);
        else
            announcerAnimator.SetBool(GameModel.ANNOUNCER_DEFEAT_TRIGGER, true);        
    }

    private void MarkSoldier(GameObject soldier) {
        soldierSpotlight.HighlightSoldier(soldier);
    }

    private void UnmarkSoldier(GameObject soldier) {
        soldierSpotlight.RemoveHighlight();
    }

    private void OnSoldierMovementAnimationEnd() {
        FixPositionAfterAnimation();
    }

    private void OnStartDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        if (canPlay && isMyTurn && !duringTie) {
            GetNextMoveInfo(obj, pos, objTranslatePosition);
        }
    }

    private void GetNextMoveInfo(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        focusedPlayerParent = obj;
        model.FocusedPlayer = obj.transform.GetChild(0).gameObject;
        startDragPos = pos;

        //save soldier's weapon to be displayed on the preview animation:
        SoldierType weapon = obj.transform.GetChild(0).gameObject.GetComponent<SC_Soldier>().Type;

        ShowDuelSoldier(weapon);
        ShowPathIndicators(objTranslatePosition);
    }

    private void OnFinishDraggingSoldier(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        if (canPlay && isMyTurn && !duringTie) {
            PerformNextMove(obj, pos, objTranslatePosition);
        }
    }

    private void PerformNextMove(GameObject obj, Vector3 pos, Vector3 objTranslatePosition) {
        endDragPos = pos;
        HidePreviewSoldier();
        HidePathIndicators();

        soldierMovementDirection = model.GetSoldierMovementDirection(startDragPos, endDragPos);
        if (soldierMovementDirection != MovementDirections.NONE) {

            //use the exact soldier position (not parent):
            Vector3 exactSoldierPosition = model.FocusedPlayer.transform.position;

            //only move if its within board borders:
            if (IsValidMove(exactSoldierPosition, soldierMovementDirection)) {

                //check if next movement will initiate a fight (as landing on a rival tile): 
                if (IsPossibleMatch(GetRequestedMoveCoord())) {
                    Match();
                }
                else {
                    //currrently a static movement, will turn into animation later.
                    MoveSoldier(focusedPlayerParent, soldierMovementDirection);
                }
                isMyTurn = false;
            }

            //Debug.Log("focusedSoldier.transform.GetChild(0).position= " + focusedPlayerParent.transform.GetChild(0).position);

            //Debug.Log("tile landed on is = " + 
            //    model.PointToTile(focusedPlayerParent.transform.GetChild(0).position).transform.position.x + "," +
            //    Mathf.Abs(model.PointToTile(focusedPlayerParent.transform.GetChild(0).position).transform.position.z));

            //Debug.Log("curr position is = " + objTranslatePosition);
            //nextPosition = new Vector3(objTranslatePosition.x, objTranslatePosition.y, objTranslatePosition.z + 1);
            //Debug.Log("new position will be = " + nextPosition);
        }
    }

    private void Match() {
        model.Match();
        soldierSpotlight.RemoveHighlight();
    }

    private void StartMatch() {
        throw new NotImplementedException();
    }

    private bool IsPossibleMatch(Point point) {
        return model.IsPossibleMatch(point);
    }

    private Point GetRequestedMoveCoord() {
        return model.GetNextMoveCoord();
    }

    private bool IsValidMove(Vector3 exactSoldierPosition, MovementDirections soldierMovementDirection) {
        return model.IsValidMove(exactSoldierPosition, soldierMovementDirection);
    }

    private void MoveSoldier(GameObject soldierParent, MovementDirections soldierMovementDirection) {
        model.MoveSoldier(soldierParent, soldierMovementDirection);
    }

    private void DisplayMovementAnimation() {

        //Debug.Log("DisplayMovementAnimation called.");
        //soldierAnimator.SetBool("IsMoving", true);
        //soldierAnimator.Play("soldier_solo_movement_forward");


    }

    public void FixPositionAfterAnimation() {
        //Debug.Log("FixPositionAfterAnimation called.");
        //Debug.Log("nextPosition= " + nextPosition);


        //soldierAnimator.Play("soldier_solo_movement_idle");
        //gameObject.transform.position = nextPosition;
        //soldierAnimator.SetBool("IsMoving", false);
    }
    
    private void HidePreviewSoldier() {
        //previewSoldierPlayer.SetActive(false);
        
        previewSoldierAnimator.SetBool(previewAnimationTrigger, false);
    }

    private void ShowDuelSoldier(SoldierType weapon) {
        previewAnimationTrigger = model.GetPreviewAnimationTriggerByWeapon(weapon);
        previewSoldierAnimator.SetBool(previewAnimationTrigger, true);
        //previewSoldierPlayer.SetActive(true);
    }


    private void ShowPathIndicators(Vector3 pos) {
        model.ShowPathIndicators(pos);
    }

    private void HidePathIndicators() {
        model.HidePathIndicators();
    }

    private void GodMode(bool state) {
        model.GodMode(state);
    }
}
