using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_MenuController : MonoBehaviour {

    private static Dictionary<string, GameObject> dictionaryMenu;
    private static readonly string SCENE_PREFIX = "Scene";
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

    private MenuModel       menuModel;
    private SC_CoinSpawner  coinSpawner;

    void Start () {
        menuModel = MenuModel.GetInstance;
        dictionaryMenu = new Dictionary<string, GameObject>();
        GameObject[] menuObjects = GameObject.FindGameObjectsWithTag("MenuObjects");

        foreach (GameObject obj in menuObjects) {
            dictionaryMenu.Add(obj.name, obj);
        }
    
       InitSliderValues();
    }

    private void InitSliderValues() {
        
        //only invoke if Scene is "Scene_Settings":
        if(currScene.Equals(Scenes.Settings.ToString())) {
            Debug.Log("updating sliders value..");
            dictionaryMenu["slider_sfx"].GetComponent<Slider>().value = sfxValue;
            dictionaryMenu["slider_bg_music"].GetComponent<Slider>().value = bgMusicValue;
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
        MoveToScene(Scenes.Gameplay.ToString());
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
        dictionaryMenu[sliderName].GetComponent<Slider>().value = 0;
    }

    public void OnClickedSettings() {
        MoveToScene(Scenes.Settings.ToString());
    }

    public void OnClickedStudentInfo() {
        MoveToScene(Scenes.StudentInfo.ToString());
    }

    public void ChangeSettingsSliderValue(string sliderName, string textValueName, ref int permanentValueHolder) {
        Slider slider = dictionaryMenu[sliderName].GetComponent<Slider>();
        dictionaryMenu[textValueName].GetComponent<Text>().text = slider.value.ToString();
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
        lastScene = currScene;
        currScene = nextScene;
        SceneManager.LoadScene(SCENE_PREFIX + currScene);
    }

    private void ExtractUsernameAndPassword() {
        usernameStr = dictionaryMenu["inf_username"].GetComponent<InputField>().text;
        passwordStr = dictionaryMenu["inf_pass"].GetComponent<InputField>().text;
    }



    void FixedUpdate() {

    }
}
