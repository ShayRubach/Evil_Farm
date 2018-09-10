using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_MenuController : MonoBehaviour {

    private static Dictionary<string, GameObject> objects;
    private MenuModel       menuModel;
    private SC_CoinSpawner  coinSpawner;
    private List<GameObject> scenes;

    private static readonly int SLIDER_STARTING_VALUE = 50;
    private static int VALUE_RATIO = 200;

    private static readonly string url = "https://github.com/ShayRubach";
    private static string currScene = Scenes.Login.ToString();
    private static string lastScene = Scenes.Login.ToString();
    private string usernameStr, passwordStr;
    private static int sfxValue = SLIDER_STARTING_VALUE;
    private static int bgMusicValue = SLIDER_STARTING_VALUE;
    private static int coinsValue = SLIDER_STARTING_VALUE;
    private static int valueRatio = VALUE_RATIO;

    public static readonly string SCENE_PREFIX = "Scene";
    public static readonly string MENU_OBJECTS_STR_NAME = "MenuObjects";
    public Slider progressBar;
    public Text progressTxtValue;

    void Start () {
        menuModel = MenuModel.GetInstance;
        objects = new Dictionary<string, GameObject>();
        scenes = new List<GameObject>();
        GameObject[] menuObjects = GameObject.FindGameObjectsWithTag(MENU_OBJECTS_STR_NAME);

        foreach (GameObject obj in menuObjects) {
            objects.Add(obj.name, obj);

            if (obj.name.Contains(SCENE_PREFIX))
                scenes.Add(obj);

            if (obj.name.StartsWith(SCENE_PREFIX) && !obj.name.Contains(SharedDataHandler.nextScreenRequested))
                obj.SetActive(false);
        }
    
       InitSliderValues();
    }

    private void InitSliderValues() {
        
        //only invoke if Scene is "Scene_Settings":
        if(currScene.Equals(Scenes.Settings.ToString())) {
            Debug.Log("updating sliders value..");
            objects["slider_sfx"].GetComponent<Slider>().value = sfxValue;
            objects["slider_bg_music"].GetComponent<Slider>().value = bgMusicValue;
        }
    }

    public void OnClickedLoginButton() {
        ExtractUsernameAndPassword();

        if (menuModel.VerifyUsernameAndPassword(usernameStr, passwordStr)) {
            Debug.Log("Logged in.");
            MoveToScene(Scenes.MainMenu.ToString());
        }
        else
            Debug.Log("Wrong username or password.");
    }

    public void OnClickedLogoutButton() {
        MoveToScene(Scenes.Login.ToString());
    }

    public void OnClickedSinglePlayer() {
        MoveToScene(Scenes.SinglePlayer.ToString());
    }

    public void OnClickedMultiplayer() {
        MoveToScene(Scenes.Multiplayer.ToString());
    }

    public void OnClickedGithubPage() {
        Application.OpenURL(url);
    }

    public void OnClickedBack() {
        MoveToScene(Scenes.MainMenu.ToString());
    }

    public void OnClickedMuteBgMusic() {
        Mute("slider_bg_music");
    }

    public void OnClickedMuteSfx() {
        Mute("slider_sfx");
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
        ChangeSettingsSliderValue("slider_sfx", "sfx_value", ref sfxValue);
    }

    public void OnBgMusicValueChanged() {
        ChangeSettingsSliderValue("slider_bg_music", "bg_music_value", ref bgMusicValue);
    }

    public void OnCoinsValueChanged() {
        ChangeSettingsSliderValue("slider_coins", "coins_value", ref coinsValue);
    }

    private void MoveToScene(string nextScene) {
        //save our last scene
        lastScene = currScene;
        currScene = nextScene;

        //only actually change scene for single/multi player:
        if (nextScene.Contains(Scenes.SinglePlayer.ToString()))
            StartCoroutine(LoadAsyncScene(SCENE_PREFIX + nextScene));
        //else just display the requested menu screen:
        else {
            objects[SCENE_PREFIX + lastScene].SetActive(false);
            objects[SCENE_PREFIX + nextScene].SetActive(true);
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
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        DisplayLoadingScreen();

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressBar.value = progress;
            progressTxtValue.text = progress * 100f + "%";
            //Debug.Log("progressBar.value = " + progressBar.value + " \t progressTxtValue.text = " + progressTxtValue.text);

            yield return null;
        }

    }

    private void ExtractUsernameAndPassword() {
        usernameStr = objects["inf_username"].GetComponent<InputField>().text;
        passwordStr = objects["inf_pass"].GetComponent<InputField>().text;
    }

}
