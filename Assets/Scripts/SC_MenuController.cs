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

    public Slider progressBar;
    public Text progressTxtValue;

    void Start () {
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

        InitSliderValues();
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
        MoveToScene(Scenes.SinglePlayer.ToString());
    }

    public void OnClickedGithubPage() {
        Application.OpenURL(SC_MenuModel.GITHUB_URL);
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
