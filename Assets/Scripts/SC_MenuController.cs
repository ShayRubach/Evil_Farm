using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_MenuController : MonoBehaviour {

    [SerializeField]
    private GameObject menuModelObject;
    private SC_MenuModel model;
    private Listener listener;

    private static Dictionary<string, GameObject> objects;
    private SC_CoinSpawner  coinSpawner;
    private List<GameObject> scenes;

    private static string currScene = Scenes.Login.ToString();
    private static string lastScene = Scenes.Login.ToString();
    private string usernameStr, passwordStr;
    private static int sfxValue = SC_MenuModel.SLIDER_STARTING_VALUE;
    private static int bgMusicValue = SC_MenuModel.SLIDER_STARTING_VALUE;
    private static int coinsValue = SC_MenuModel.SLIDER_STARTING_VALUE;
    private static int valueRatio = SC_MenuModel.VALUE_RATIO;

    private int roomIndex = SC_MenuModel.INITIAL_ROOM_IDX;
    private string roomId = SC_MenuModel.INITIAL_ROOM_ID;

    public Slider progressBar;
    public Text progressTxtValue;

    private void OnEnable() {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
    }

    private void OnDisable() {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
    }

    void Start () {
        Debug.Log("Start from" + gameObject);
        Init();
    }

    private void Init() {
        model = menuModelObject.GetComponent<SC_MenuModel>();
        objects = model.GetObjects();
        scenes = new List<GameObject>();

        foreach (KeyValuePair<string,GameObject> obj in objects) {

            if (obj.Value.name.Contains(SC_MenuModel.SCENE_PREFIX))
                scenes.Add(obj.Value);

            if (obj.Value.name.StartsWith(SC_MenuModel.SCENE_PREFIX) && !(obj.Value.name.Contains(SharedDataHandler.nextScreenRequested)))
                obj.Value.SetActive(false);
        }

        InitGameObjectInitialStates();
        ListenToRoomEvents();
    }

    private void InitGameObjectInitialStates() {
        InitSliderValues();
        InitConnectionStatusMsgs();
    }

    private void InitConnectionStatusMsgs() {
        objects[SC_MenuModel.CONNECTED_GRP_VAR_NAME].SetActive(true);
        objects[SC_MenuModel.CONNECTING_TO_SERVER_VAR_NAME].SetActive(false);

        objects[SC_MenuModel.WAITING_FOR_PLAYER_VAR_NAME].GetComponent<Text>().text = SC_MenuModel.WAITING_FOR_PLAYER_PREFIX;
        objects[SC_MenuModel.WAITING_FOR_PLAYER_VAR_NAME].SetActive(false);

    }

    private void ListenToRoomEvents() {
        if (listener == null)
            listener = new Listener();

        WarpClient.initialize(model.GetAPIKey(), model.GetSecretKey());
        WarpClient.GetInstance().AddConnectionRequestListener(listener);
        WarpClient.GetInstance().AddChatRequestListener(listener);
        WarpClient.GetInstance().AddUpdateRequestListener(listener);
        WarpClient.GetInstance().AddLobbyRequestListener(listener);
        WarpClient.GetInstance().AddNotificationListener(listener);
        WarpClient.GetInstance().AddRoomRequestListener(listener);
        WarpClient.GetInstance().AddZoneRequestListener(listener);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listener);
    }

    private void InitSliderValues() {
        
        //only invoke if Scene is "Scene_Settings":
        if(currScene.Equals(Scenes.Settings.ToString())) {
            objects[SC_MenuModel.SLIDER_SFX_VAR_NAME].GetComponent<Slider>().value = sfxValue;
            objects[SC_MenuModel.SLIDER_BG_MUSIC_VAR_NAME].GetComponent<Slider>().value = bgMusicValue;
        }
    }

    public void OnClickedLoginButton() {
        ExtractUsernameAndPassword();
        RegisterNewUser(usernameStr, passwordStr);
        MoveToScene(Scenes.MainMenu.ToString());

        //VerifyUserAndPass(usernameStr, passwordStr);
    }

    private void RegisterNewUser(string usernameStr, string passwordStr) {
        model.RegisterNewUser(usernameStr, passwordStr);
        
    }

    
    private void VerifyUserAndPass(string usernameStr, string passwordStr) {
        if (model.VerifyUsernameAndPassword(usernameStr, passwordStr)) {
            Debug.Log(SC_MenuModel.LOGGED_IN_POPUP_MSG);
            MoveToScene(Scenes.MainMenu.ToString());
        }
        else {
            Debug.Log(SC_MenuModel.WRONG_DETAILS_POPUP_MSG);
        }
    }

    public void OnClickedLogoutButton() {
        MoveToScene(Scenes.Login.ToString());
    }

    public void OnClickedSinglePlayer() {
        SharedDataHandler.SetMultiplayerMode(false);
        MoveToScene(Scenes.SinglePlayer.ToString());
    }

    public void OnClickedMultiplayer() {
        SharedDataHandler.SetMultiplayerMode(true);

        WarpClient.GetInstance().Connect(model.GetUserName());
        Debug.Log("Connecting...");

        //MoveToScene(Scenes.SinglePlayer.ToString());
    }

    public void OnClickedGithubPage() {
        Application.OpenURL(SC_MenuModel.GITHUB_URL);
    }

    public void OnClickedBack() {
        MoveToScene(Scenes.MainMenu.ToString());
    }

    public void OnClickedMuteBgMusic() {
        Mute(SC_MenuModel.SLIDER_BG_MUSIC_VAR_NAME);
    }

    public void OnClickedMuteSfx() {
        Mute(SC_MenuModel.SLIDER_SFX_VAR_NAME);
    }

    public void Mute(string sliderName) {
        objects[sliderName].GetComponent<Slider>().value = 0;
    }

    public void OnClickedSettings() {
        MoveToScene(Scenes.Settings.ToString());
    }

    public void OnClickedStudentInfo() {
        MoveToScene(Scenes.StudentInfo.ToString());
    }

    public void ChangeSettingsSliderValue(string sliderName, string textValueName, ref int permanentValueHolder) {
        Slider slider = objects[sliderName].GetComponent<Slider>();
        objects[textValueName].GetComponent<Text>().text = slider.value.ToString();
        permanentValueHolder = (int)slider.value;
    }

    public void OnSfxValueChanged() {
        ChangeSettingsSliderValue(SC_MenuModel.SLIDER_SFX_VAR_NAME, SC_MenuModel.SFX_VALUE_VAR_NAME, ref sfxValue);
    }

    public void OnBgMusicValueChanged() {
        ChangeSettingsSliderValue(SC_MenuModel.SLIDER_BG_MUSIC_VAR_NAME, SC_MenuModel.BG_MUSIC_VALUE_VAR_NAME, ref bgMusicValue);
    }

    private void MoveToScene(string nextScene) {

        //save our last scene
        lastScene = currScene;
        currScene = nextScene;

        //only actually change scene for single/multi player:
        if (nextScene.Contains(Scenes.SinglePlayer.ToString()))
            StartCoroutine(LoadAsyncScene(SC_MenuModel.SCENE_PREFIX + nextScene));
        //else just display the requested menu screen:
        else {
            objects[SC_MenuModel.SCENE_PREFIX + lastScene].SetActive(false);
            objects[SC_MenuModel.SCENE_PREFIX + nextScene].SetActive(true);
        }
            
    }

    private void DisplayLoadingScreen() {
        foreach(GameObject scene in scenes) {
            if(scene.name.Contains(Scenes.Loader.ToString()))
                scene.SetActive(true);
            else
                scene.SetActive(false);
        }
    }

    IEnumerator LoadAsyncScene(string sceneName) {

        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        DisplayLoadingScreen();

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressBar.value = progress;
            progressTxtValue.text = progress * 100f + "%";
            yield return null;
        }

        SceneManager.MoveGameObjectToScene(objects[SC_MenuModel.MENU_SCRIPTS_VAR_NAME], SceneManager.GetSceneByName(sceneName));
        SceneManager.UnloadSceneAsync(currentScene);
    }

    private void ExtractUsernameAndPassword() {
        usernameStr = objects[SC_MenuModel.USERNAME_INPUT_FILED_VAR_NAME].GetComponent<InputField>().text;
        passwordStr = objects[SC_MenuModel.PASSWORD_INPUT_FILED_VAR_NAME].GetComponent<InputField>().text;
    }

    private void OnConnect(bool isSuccess) {
        Debug.Log(isSuccess);
        if (isSuccess) {
            Debug.Log("Connected!");
            model.objects[SC_MenuModel.USER_CONN_VAR_NAME].GetComponent<Text>().text = SC_MenuModel.CONNECTED_AS_PREFIX + model.GetUserName();
            objects[SC_MenuModel.BTN_FIND_MATCH_VAR_NAME].GetComponent<Button>().interactable = true;
            MoveToScene(Scenes.Multiplayer.ToString());
        }
    }

    public void OnClickedFindMatch() {
        
        //disable button interaction
        objects[SC_MenuModel.BTN_FIND_MATCH_VAR_NAME].GetComponent<Button>().interactable = false;

        DisplayConnectingToServerMsg();

        WarpClient.GetInstance().GetRoomsInRange(1, 2);
        Debug.Log("Searching for room..");
    }

    private void DisplayConnectingToServerMsg() {
        objects[SC_MenuModel.CONNECTED_GRP_VAR_NAME].SetActive(false);
        objects[SC_MenuModel.CONNECTING_TO_SERVER_VAR_NAME].SetActive(true);
    }

    public void OnRoomsInRange(bool isSuccess, MatchedRoomsEvent eventObj) {
        if (isSuccess) {
            Debug.Log("Parsing rooms..");
            foreach (var roomData in eventObj.getRoomsData()) {
                Debug.Log("Room Id: " + roomData.getId());
                Debug.Log("Room Owner: " + roomData.getRoomOwner());
                model.Rooms.Add(roomData.getId());
            }

            roomIndex = 0;
            SearchRoom(roomIndex);
        }
    }

    private void SearchRoom(int roomIndex) {
        model.SearchRoom(roomIndex);
    }

    private void OnCreateRoom(bool isSuccess, string createdRoomId) {
        if (isSuccess) {
            Debug.Log("Room Created! " + createdRoomId);
            this.roomId = createdRoomId;
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
        else { 
            Debug.Log("Error create room");
        }
    }

    public void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj) {
        Debug.Log("OnGetLiveRoomInfo called");

        Dictionary<string, object> properties = eventObj.getProperties();
        Debug.Log(properties[SC_MenuModel.ROOM_GRP_PASSWORD_KEY]); ;
        if (properties[SC_MenuModel.ROOM_GRP_PASSWORD_KEY].ToString() == model.MatchRoomData[SC_MenuModel.ROOM_GRP_PASSWORD_KEY].ToString()) {
            roomId = eventObj.getData().getId();
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
        else {
            roomIndex++;
            model.SearchRoom(roomIndex);
        }
    }

    public void OnJoinRoom(bool isSuccess, string joinedRoomId) {
        if (isSuccess) {
            Debug.Log("Joined Room: " + joinedRoomId);
            DisplayJoinedRoomMsg(joinedRoomId);
        }
    }

    private void DisplayJoinedRoomMsg(string joinedRoomId) {
        objects[SC_MenuModel.CONNECTING_TO_SERVER_VAR_NAME].SetActive(false);
        objects[SC_MenuModel.WAITING_FOR_PLAYER_VAR_NAME].SetActive(true);

        string fixedString = SC_MenuModel.WAITING_FOR_PLAYER_PREFIX;
        objects[SC_MenuModel.WAITING_FOR_PLAYER_VAR_NAME].GetComponent<Text>().text = fixedString.Replace(SC_MenuModel.ROOM_ID_WILDCARD, joinedRoomId);
        
    }

    public void OnUserJoinRoom(RoomData eventObj, string userJoinedName) {
        Debug.Log("User: " + userJoinedName + " joined the room");
        if (userJoinedName != model.GetUserName()) {
            Debug.Log("OnUserJoinRoom ");
            WarpClient.GetInstance().startGame();
        }
    }

    public void OnGameStarted(string sender, string thisRoomId, string nextTurn) {
        Debug.Log("OnGameStarted called");
        MoveToScene(Scenes.SinglePlayer.ToString());
    }

    private void OnMoveCompleted(MoveEvent _Move) {
        //if (OnMoveCompletedNotify != null)
        //    OnMoveCompletedNotify();
    }
}
