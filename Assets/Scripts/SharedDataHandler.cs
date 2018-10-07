using UnityEngine;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client;
using AssemblyCSharp;
using System;

/*
 * a simple utility class to transfer data between scenes
 */
public static class SharedDataHandler {

    public delegate void OnConnenctivityHandler(bool isSuccess);
    public static event OnConnenctivityHandler OnConnect;
    public static event OnConnenctivityHandler OnDisconnect;

    public delegate void RoomHandler(bool isSuccess, string roomId);
    public static event RoomHandler OnJoinRoom;
    public static event RoomHandler OnCreateRoom;

    public delegate void OnMoveCompletedHandler(MoveEvent move);
    public static event OnMoveCompletedHandler OnMoveCompleted;

    public delegate void OnRoomsInRangeHandler(bool isSuccess, MatchedRoomsEvent eventObj);
    public static event OnRoomsInRangeHandler OnRoomsInRange;

    public delegate void OnGameStartedHandler(string sender, string thisRoomId, string nextTurn);
    public static event OnGameStartedHandler OnGameStarted;

    public delegate void OnUserJoinRoomHandler(RoomData eventObj, string userName);
    public static event OnUserJoinRoomHandler OnUserJoinRoom;

    public delegate void OnGetLiveRoomInfoHandler(LiveRoomInfoEvent eventObj);
    public static event OnGetLiveRoomInfoHandler OnGetLiveRoomInfo;

    public delegate void OnChatReceivedHandler(string sender, string msg);
    public static event OnChatReceivedHandler OnPrivateChatReceived;

    public delegate void GlobalSoundHandler (string sliderName, float value);
    public static event GlobalSoundHandler OnGlobalSoundValueChanged;


    public static string nextScreenRequested {get;set;}
    public static string username { get; set; }
    public static string enemyUsername { get; set; }

    public static float globalSfxVolume { get; set; }
    public static float globalBgmVolume { get; set; }



    public static int wins {get;set;}
    public static int loses {get;set;}

    public static bool isMultiplayer = false;
    public static bool isPlayerStarting = true;

    public static WarpClient client;
    public static Listener listener = new Listener();

    internal static string userNameKey = "UserName";
    internal static string dataKey = "Data";

    static SharedDataHandler() {
        nextScreenRequested = SC_MenuModel.INITIAL_SCENE;
        ListenToEvents();
    }

    private static void ListenToEvents() {
        Listener.OnConnect += OnConnectBroadcast;
        Listener.OnRoomsInRange += OnRoomsInRangeBroadcast;
        Listener.OnCreateRoom += OnCreateRoomBroadcast;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfoBroadcast;
        Listener.OnJoinRoom += OnJoinRoomBroadcast;
        Listener.OnUserJoinRoom += OnUserJoinRoomBroadcast;
        Listener.OnGameStarted += OnGameStartedBroadcast;
        Listener.OnDisconnect += OnDisconnectBroadcast;
        Listener.OnMoveCompleted += OnMoveCompletedBroadcast;
        Listener.OnPrivateChatReceived += OnPrivateChatReceivedBroadcast;
    }

    public static void AddEvents(string apiKey, string secretKey) {

        WarpClient.initialize(apiKey, secretKey);
        client = WarpClient.GetInstance();
        WarpClient.GetInstance().AddConnectionRequestListener(listener);
        WarpClient.GetInstance().AddChatRequestListener(listener);
        WarpClient.GetInstance().AddUpdateRequestListener(listener);
        WarpClient.GetInstance().AddLobbyRequestListener(listener);
        WarpClient.GetInstance().AddNotificationListener(listener);
        WarpClient.GetInstance().AddRoomRequestListener(listener);
        WarpClient.GetInstance().AddZoneRequestListener(listener);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listener);
        WarpClient.GetInstance().AddChatRequestListener(listener);
    }

    private static void OnMoveCompletedBroadcast(MoveEvent move) {
        //Debug.Log("SharedDataHandler: OnMoveCompletedBroadcast called");
        if (OnMoveCompleted != null)
            OnMoveCompleted(move);
    }

    private static void OnDisconnectBroadcast(bool isSuccess) {
        //Debug.Log("SharedDataHandler: OnDisconnectBroadcast called");
        if (OnDisconnect != null)
            OnDisconnect(isSuccess);
    }

    private static void OnGameStartedBroadcast(string sender, string thisRoomId, string nextTurn) {
        //Debug.Log("SharedDataHandler: OnGameStartedBroadcast called");
        if (OnGameStarted != null)
            OnGameStarted(sender, thisRoomId, nextTurn);
    }

    private static void OnUserJoinRoomBroadcast(RoomData eventObj, string userName) {
        //Debug.Log("SharedDataHandler: OnUserJoinRoomBroadcast called");
        if (OnUserJoinRoom != null)
            OnUserJoinRoom(eventObj, userName);
    }

    private static void OnJoinRoomBroadcast(bool isSuccess, string roomId) {
        //Debug.Log("SharedDataHandler: OnJoinRoomBroadcast called");
        if (OnJoinRoom != null)
            OnJoinRoom(isSuccess, roomId);
    }

    private static void OnGetLiveRoomInfoBroadcast(LiveRoomInfoEvent eventObj) {
        //Debug.Log("SharedDataHandler: OnGetLiveRoomInfoBroadcast called");
        if (OnGetLiveRoomInfo != null)
            OnGetLiveRoomInfo(eventObj);
    }

    private static void OnCreateRoomBroadcast(bool isSuccess, string createdRoomId) {
        //Debug.Log("SharedDataHandler: OnCreateRoomBroadcast called");
        if (OnCreateRoom != null)
            OnCreateRoom(isSuccess, createdRoomId);
    }

    private static void OnConnectBroadcast(bool isSuccess) {
        //Debug.Log("SharedDataHandler: OnConnectBroadcast called");
        if (OnConnect != null)
            OnConnect(isSuccess);
    }

    private static void OnRoomsInRangeBroadcast(bool isSuccess, MatchedRoomsEvent eventObj) {
        //Debug.Log("SharedDataHandler: OnRoomsInRangeBroadcast called");
        if (OnRoomsInRange != null)
            OnRoomsInRange(isSuccess, eventObj);
    }

    private static void OnPrivateChatReceivedBroadcast(string sender, string msg) {
        //Debug.Log("SharedDataHandler: OnPrivateChatReceived called");
        if (OnPrivateChatReceived != null)
            OnPrivateChatReceived(sender, msg);
    }

    internal static void SetMultiplayerMode(bool mode) {
        isMultiplayer = mode;
    }

    internal static void UpdateGlobalSoundValues(string textValueName, float value) {
        if (textValueName == SC_MenuModel.SFX_VALUE_VAR_NAME)
            globalSfxVolume = value;
        else
            globalBgmVolume = value;

        if(OnGlobalSoundValueChanged != null)
            OnGlobalSoundValueChanged(textValueName, value);
    }
}
