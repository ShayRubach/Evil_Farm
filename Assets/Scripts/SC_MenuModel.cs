using AssemblyCSharp;
using System.Collections.Generic;
using UnityEngine;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using UnityEngine.UI;

public class SC_MenuModel : MonoBehaviour {
    
    public static int VALUE_RATIO = 200;
    public static readonly int SLIDER_STARTING_VALUE = 50;
    public static readonly int INITIAL_ROOM_IDX = 0;
    public static readonly int MAX_TURN_TIME = 120;

    public static readonly string SLIDER_SFX_VAR_NAME = "slider_sfx";
    public static readonly string SLIDER_BG_MUSIC_VAR_NAME = "slider_bg_music";
    public static readonly string SFX_VALUE_VAR_NAME = "sfx_value";
    public static readonly string BG_MUSIC_VALUE_VAR_NAME = "bg_music_value";
    public static readonly string USER_CONN_VAR_NAME = "txt_conn_username";
    public static readonly string BTN_FIND_MATCH_VAR_NAME = "btn_auto_find_room";
    public static readonly string CONNECTED_GRP_VAR_NAME = "txt_connected_grp";
    public static readonly string CONNECTING_TO_SERVER_VAR_NAME = "txt_connecting";
    public static readonly string WAITING_FOR_PLAYER_VAR_NAME = "txt_waiting_for_player";
    public static readonly string MENU_SCRIPTS_VAR_NAME = "scripts_menu";
    public static readonly string USERNAME_INPUT_FILED_VAR_NAME = "inf_username";
    public static readonly string PASSWORD_INPUT_FILED_VAR_NAME = "inf_pass";
    
    public static readonly string ROOM_ID_WILDCARD = "?";
    public static readonly string WAITING_FOR_PLAYER_PREFIX = "Joined room ? \nWaiting for another player...";
    public static readonly string CONNECTED_AS_PREFIX = "Connected as\n";
    public static readonly string INITIAL_SCENE = "Login";
    public static readonly string INITIAL_ROOM_ID = "";
    public static readonly string LOGGED_IN_POPUP_MSG = "Logged in.";
    public static readonly string WRONG_DETAILS_POPUP_MSG = "Wrong username or password.";
    public static readonly string SCENE_PREFIX = "Scene";
    public static readonly string MENU_OBJECTS_STR_NAME = "MenuObjects";
    public static readonly string ROOM_GRP_PASSWORD_KEY = "Password";
    public static readonly string ROOM_GRP_PASSWORD_VAL = "shenkar";

    public static readonly string GITHUB_URL = "https://github.com/ShayRubach/Evil_Garden";

    private string apiKey = "33d4ff44dbe11a5a2c994c9aeae94e6de31abd37c85c797d09d30398318f4876";
    private string secretKey = "cbd6ccd273f31f5753eb384f92e072d932d3b99c34bf886d1795f579ca425a4e";
        
    private string username = "";
    private string passowrd = "";

    public Dictionary<string, GameObject> objects { get; set; }
    public Dictionary<string, object> MatchRoomData { get; set; }
    public List<string> Rooms { get; set; }

    private void Awake() {
        objects = null;
        Init();
    }

    private void Init() {
        Rooms = new List<string>();
        InitMatchRoomData(ROOM_GRP_PASSWORD_KEY, ROOM_GRP_PASSWORD_VAL);

        //only fetch all tagged game objects once:
        if (objects == null) {
            objects = new Dictionary<string, GameObject>();

            GameObject[] menuObjects = GameObject.FindGameObjectsWithTag(MENU_OBJECTS_STR_NAME);

            foreach (GameObject obj in menuObjects) {
                objects.Add(obj.name, obj);
            }
        }
    }

    private void InitMatchRoomData(string key, string value) {
        MatchRoomData = new Dictionary<string, object>();
        MatchRoomData.Add(key, value);
    }

    public Dictionary<string, GameObject> GetObjects() {
        return objects;
    }

    public bool VerifyUsernameAndPassword(string un, string pass) {
        return username.Equals(un) && passowrd.Equals(pass);
    }

    public void RegisterNewUser(string usernameStr, string passwordStr) {
        username = usernameStr;
        passowrd = passwordStr;

        SharedDataHandler.username = username;
    }

    internal void SearchRoom(int roomIndex) {

        if (roomIndex < Rooms.Count) {
            Debug.Log("Getting room Details (" + Rooms[roomIndex] + ")");
            SharedDataHandler.client.GetLiveRoomInfo(Rooms[roomIndex]);

        }
        else {
            Debug.Log("Creating Room ...");
            SharedDataHandler.client.CreateTurnRoom("Test" + username, username, 2, MatchRoomData, MAX_TURN_TIME);
            Debug.Log("Room created");
        }
    }

    public string GetUserName() {
        return username;
    }

    internal string GetAPIKey() {
        return apiKey;
    }

    internal string GetSecretKey() {
        return secretKey;
    }
}
