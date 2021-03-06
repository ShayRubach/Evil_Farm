﻿using System;
using com.shephertz.app42.gaming.multiplayer.client.events;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * the MVC's controller for our app.
 * it will pass commands to logic and view.
 */

public class SC_GameController : MonoBehaviour {

    private GameObject focusedPlayerParent;                 //parent GameObject that holds the soldier and its weapon
    private GameObject previewSoldierPlayer;                //the enlarged previewed soldier user sees on a soldier focus (click)
    private SC_GameModel model;                             //our game logic model

    [SerializeField]
    private GameObject countdownManager;
    [SerializeField]
    private GameObject shuffleHandler;
    [SerializeField]
    private GameObject audioManagerObject;
    private SC_AudioManager audioManager;

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
        model = GameObject.Find(SC_GameModel.GAME_MODEL_NAME_VAR).GetComponent<SC_GameModel>();

        //save references for some general objects we need to control
        soldierSpotlight = model.GetObjects()[SC_GameModel.SPOTLIGHT_NAME_VAR].GetComponent<SC_Spotlight>();
        announcerAnimator = model.GetObjects()[SC_GameModel.ANNOUNCER_VAR_NAME].GetComponent<Animator>();
        endGameOptionsAnimator = model.GetObjects()[SC_GameModel.END_GAME_OPTIONS_VAR_NAME].GetComponent<Animator>();
        battleAnimator = model.GetObjects()[SC_GameModel.BATTLE_ANIMATOR_VAR_NAME].GetComponent<Animator>();

        previewSoldierPlayer = model.GetObjects()[SC_GameModel.PREVIEW_SOLDIER_NAME_VAR];
        previewSoldierAnimator = previewSoldierPlayer.GetComponent<Animator>();

        Init();
    }

    private void Init() {
        audioManager = audioManagerObject.GetComponent<SC_AudioManager>();
        HidePreviewSoldier();
        countdownManager.SetActive(true);
        shuffleHandler.SetActive(true);
        isMyTurn = SharedDataHandler.isPlayerStarting;

        //audioManager.sounds.Find(sound => sound.name == SC_GameModel.BG_MUSIC_GAMEPLAY_VAR_NAME).volume = SharedDataHandler.globalBgmVolume;
        InitSoundValues();
        audioManager.Play(SC_GameModel.BG_MUSIC_GAMEPLAY_VAR_NAME);
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

        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }

        if (!SharedDataHandler.isMultiplayer) {
            if (!isMyTurn && !duringTie && canPlay) {
                if (!AIAlreadyPlaying)
                    PlayAsAI();
            }
        }

    }

    private void PlayAsAI() {
        AIAlreadyPlaying = true;
        model.PlayAsAI();
    }
    
    void OnEnable() {
        SC_GameModel.FinishGame += FinishGame;
        SC_GameModel.AIMoveFinished += AIMoveFinished;
        SC_GameModel.CallTieBreaker += TieBreaker;
        SC_GameModel.OnMatchStarted += OnMatchStarted;
        SC_GameModel.OnMatchFinished += OnMatchFinished;
        SC_GameModel.OnSoldierMovementComplete += OnSoldierMovementComplete;
        SC_GameModel.Shuffling += Shuffling;
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
        SharedDataHandler.OnMoveCompleted += OnMoveCompleted;
        SharedDataHandler.OnPrivateChatReceived += OnPrivateChatReceived;
        SC_Cart.GodMode += GodMode;
    }

    void OnDisable() {
        SC_GameModel.FinishGame -= FinishGame;
        SC_GameModel.AIMoveFinished -= AIMoveFinished;
        SC_GameModel.CallTieBreaker -= TieBreaker;
        SC_GameModel.OnMatchStarted -= OnMatchStarted;
        SC_GameModel.OnMatchFinished -= OnMatchFinished;
        SC_GameModel.OnSoldierMovementComplete -= OnSoldierMovementComplete;
        SC_GameModel.Shuffling -= Shuffling;
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
        SharedDataHandler.OnMoveCompleted -= OnMoveCompleted;
        SharedDataHandler.OnPrivateChatReceived -= OnPrivateChatReceived;
        SC_Cart.GodMode -= GodMode;
    }

    private void InitSoundValues() {
        foreach (AudioSource s in audioManager.sounds) {
            //refactor:
            if (s.name.Contains("bg"))
                s.volume = SharedDataHandler.globalBgmVolume;
            else {
                s.volume = SharedDataHandler.globalSfxVolume;
            }
        }
    }

    private void OnSoldierMovementComplete() {
        PlayMoveSound();
    }

    private void Shuffling() {
        PlayShuffleSound();
    }

    private void PlayShuffleSound() {
        float MAX_SHUFFLE_SFX_PITCH = 3f;
        float BASE_SHUFFLE_SFX_PITCH = 1f;
        float PITCH_INC_VALUE = .1f;

        audioManager.Play(SC_GameModel.SHUFFLE_VAR_NAME);
        AudioSource source = audioManager.sounds.Find(sound => sound.name == SC_GameModel.SHUFFLE_VAR_NAME);
        if(source.pitch >= MAX_SHUFFLE_SFX_PITCH) {
            source.pitch = BASE_SHUFFLE_SFX_PITCH;
        }
        else {
            source.pitch += PITCH_INC_VALUE;
        }
    }

    private void OnPrivateChatReceived(string sender, string msg) {
        if(sender != null && sender != SharedDataHandler.username) {
            model.HandlePrivateMsgAck(msg);
        }
    }

    private void OnMoveCompleted(MoveEvent move) {
        isMyTurn = model.HandleMsgAck(move);
    }

    private void OnMatchStarted() {
        canPlay = false;
        battleAnimator.gameObject.SetActive(true);
        ResetAnimatorParameters(battleAnimator);
        EnableBattleAnimationParameters();
    }

    private void EnableBattleAnimationParameters() {
        string parameter = model.GetCurrentBattleAnimationParameters();
        
        //if game ended, dont show anymore battle animations
        if (parameter.Contains(SC_GameModel.CRYSTAL_VAR_NAME)) {
            battleAnimator.gameObject.SetActive(false);
        }
        else {
            battleAnimator.SetBool(parameter, true);
        }
        
    }

    private void OnBattleAnimationFinish() {
        canPlay = true;
        ResetAnimatorParameters(battleAnimator);
    }

    private void OnShuffleClicked() {
        model.ShuffleTeam(SoldierTeam.PLAYER);
    }

    private void PreparationTimeStarted() {
        canPlay = false;
    }

    private void PreparationTimeOver() {
        shuffleHandler.SetActive(false);
        canPlay = true;
    }

    private void OnEndGameOptionChoice(EndGameOption choice) {

        audioManager.Stop(SC_GameModel.BG_MUSIC_GAMEPLAY_VAR_NAME);

        if (choice == EndGameOption.RESTART) {
            endGameOptionsAnimator.SetBool(SC_GameModel.END_GAME_TRIGGER, false);
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
    }

    private void ResetAnimatorParameters(Animator animator) {

        for (int i = 0; i < animator.parameterCount ; i++) {
            //if(animator.GetParameter(i).GetType() == typeof(bool))
            animator.SetBool(animator.GetParameter(i).name, false);
        }

    }

    private void OnNewWeaponChoice(SoldierType newWeapon) {

        model.iPickedNewWeapon = true;
        ShowTieWeapons(false);

        if (SharedDataHandler.isMultiplayer) {
            model.FocusedPlayer.GetComponent<SC_Soldier>().RefreshWeapon(newWeapon, false);
            if (model.rivalPickedNewWeapon) {
                //Debug.LogError("OnNewWeaponChoice: both players picked new weapons. invoking Match()");
                model.SendTieAck(((int)newWeapon).ToString());
                model.DisplayTurnIndicator();
                model.ResetWeaponPicksStatus();
                Match(false);
            }
            else {
                model.SendTieAck(((int)newWeapon).ToString());
            }
        }
        else {
            //vs. AI will result in an instant rematch. no need to wait for the other player choice of weapon.
            model.GetPlayerSoldier().GetComponent<SC_Soldier>().RefreshWeapon(newWeapon);
            Match();
        }

        duringTie = false;
    }

    /* 
    * callback after a match ends (without a tie)
    */
    private void OnMatchFinished(SoldierTeam matchWinningTeam) {
        if(matchWinningTeam == SoldierTeam.PLAYER)
            announcerAnimator.SetBool(SC_GameModel.ANNOUNCER_WIN_TRIGGER, true);
        if(matchWinningTeam == SoldierTeam.ENEMY)
            announcerAnimator.SetBool(SC_GameModel.ANNOUNCER_LOSE_TRIGGER, true);
        if(matchWinningTeam == SoldierTeam.NO_TEAM)
            announcerAnimator.SetBool(SC_GameModel.ANNOUNCER_LOSE_TRIGGER, true);

    }

    /* 
    * callback for a tied match
    */
    private void TieBreaker() {
        announcerAnimator.SetBool(SC_GameModel.ANNOUNCER_TIE_TRIGGER, true);
        duringTie = true;
        ShowTieWeapons(true);
    }

    private void ShowTieWeapons(bool isVisible) {
        model.GetObjects()[SC_GameModel.TIE_WEAPONS_P_VAR_NAME].SetActive(isVisible);
    }
    
    //called when AI is finished with his move (inc. animations):
    private void AIMoveFinished() {
        AIAlreadyPlaying = false;
        isMyTurn = true;
    }

    private void FinishGame(SoldierTeam winner) {
        canPlay = false;
        endGameOptionsAnimator.SetBool(SC_GameModel.END_GAME_TRIGGER, true);

        if (winner == SoldierTeam.PLAYER)
            announcerAnimator.SetBool(SC_GameModel.ANNOUNCER_VICTORY_TRIGGER, true);
        else
            announcerAnimator.SetBool(SC_GameModel.ANNOUNCER_DEFEAT_TRIGGER, true);        
    }

    private void MarkSoldier(GameObject soldier) {
        //todo: remove this and turn the inner light in enemy's pumpkin on and off
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

        //use the exact soldier position (not parent):
        Vector3 exactSoldierPosition = model.FocusedPlayer.transform.position;

        isMyTurn = PerformMove(focusedPlayerParent, exactSoldierPosition, soldierMovementDirection);
    }

    private bool PerformMove(GameObject focusedPlayerParent, Vector3 exactSoldierPosition, MovementDirections soldierMovementDirection) {
        return model.PerformMove(focusedPlayerParent, exactSoldierPosition, soldierMovementDirection);
    }

    private void Match(bool sendAck = true) {
        model.Match(sendAck);
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

    private void PlayMoveSound() {
        string moveSound = model.playerTurnIndicator.activeSelf ? SC_GameModel.PLAYER_MOVE_VAR_NAME : SC_GameModel.ENEMY_MOVE_VAR_NAME;
        audioManager.Play(moveSound);
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
        previewSoldierAnimator.SetBool(previewAnimationTrigger, false);
    }

    private void ShowDuelSoldier(SoldierType weapon) {
        previewAnimationTrigger = model.GetPreviewAnimationTriggerByWeapon(weapon);
        previewSoldierAnimator.SetBool(previewAnimationTrigger, true);
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
