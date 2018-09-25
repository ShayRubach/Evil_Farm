using AssemblyCSharp;
using System.Collections.Generic;
using UnityEngine;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public sealed class MenuModel {

    public static int VALUE_RATIO = 200;
    public static readonly int SLIDER_STARTING_VALUE = 50;
    public static readonly int INITIAL_ROOM_IDX = 0;
    public static readonly string INITIAL_SCENE = "Login";
    public static readonly string INITIAL_ROOM_ID = "";
    public static readonly string SLIDER_SFX_VAR_NAME = "slider_sfx";
    public static readonly string SLIDER_BG_MUSIC_VAR_NAME = "slider_bg_music";
    public static readonly string LOGGED_IN_POPUP_MSG = "Logged in.";
    public static readonly string WRONG_DETAILS_POPUP_MSG = "Wrong username or password.";

    private string apiKey = "33d4ff44dbe11a5a2c994c9aeae94e6de31abd37c85c797d09d30398318f4876";
    private string secretKey = "cbd6ccd273f31f5753eb384f92e072d932d3b99c34bf886d1795f579ca425a4e";

    private static MenuModel instance = null;
    private static readonly System.Object lockingObj = new System.Object();
    
    private string username = "sh";
    private string passowrd = "sh";

    private Dictionary<string, GameObject> unityObjects;
    private Dictionary<string, object> matchRoomData;
    private List<string> rooms;

    private Listener listener;
    private int roomIndex = INITIAL_ROOM_IDX;
    private string roomId = INITIAL_ROOM_ID;

    private MenuModel() {}	

    public static MenuModel GetInstance {
        get {
            if(instance == null) {
                lock (lockingObj) {
                    instance = (instance == null) ? new MenuModel() : instance;
                }
            }
            return instance;
        }
    }
    public bool VerifyUsernameAndPassword(string un, string pass) {
        return username.Equals(un) && passowrd.Equals(pass);
    }

    internal void RegisterNewUser(string usernameStr, string passwordStr) {
        username = usernameStr;
        passowrd = passwordStr;
    }

    public string getUserName() {
        return username;
    }
}
