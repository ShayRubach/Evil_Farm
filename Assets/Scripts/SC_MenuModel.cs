using AssemblyCSharp;
using System.Collections.Generic;
using UnityEngine;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;

public class SC_MenuModel : MonoBehaviour {

    public static int VALUE_RATIO = 200;
    public static readonly int SLIDER_STARTING_VALUE = 50;
    public static readonly int INITIAL_ROOM_IDX = 0;
    public static readonly string INITIAL_SCENE = "Login";
    public static readonly string INITIAL_ROOM_ID = "";
    public static readonly string SLIDER_SFX_VAR_NAME = "slider_sfx";
    public static readonly string SLIDER_BG_MUSIC_VAR_NAME = "slider_bg_music";
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

    private Dictionary<string, GameObject> objects = null;
    public Dictionary<string, object> matchRoomData { get; set; }
    public List<string> rooms { get; set; }

    private int roomIndex = INITIAL_ROOM_IDX;
    private string roomId = INITIAL_ROOM_ID;


    private void Awake() {
        Init();
    }

    private void Init() {

        //only fetch all tagged game objects once:
        if (objects == null) {
            objects = new Dictionary<string, GameObject>();

            GameObject[] menuObjects = GameObject.FindGameObjectsWithTag(MENU_OBJECTS_STR_NAME);

            foreach (GameObject obj in menuObjects) {
                objects.Add(obj.name, obj);
            }

            InitMatchRoomData(ROOM_GRP_PASSWORD_KEY, ROOM_GRP_PASSWORD_VAL);
        }
    }

    private void InitMatchRoomData(string key, string value) {
        matchRoomData = new Dictionary<string, object>();
        matchRoomData.Add(key, value);
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
