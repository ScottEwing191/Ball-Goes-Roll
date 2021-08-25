using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoSingleton<MainMenuManager>, ISaveable {
    List<Scores> levelScores = new List<Scores>();

    public List<Scores> LevelScores {
        get { return levelScores; }
    }



    // Not sure What this does or if it is being used
    public void StartGame() {
        UIMainMenuManager.Instance.ScreenFade(0, 1);
        StartCoroutine(LoadScene(1, UIMainMenuManager.Instance.ScreenFadeTime * 1.1f));     // multiply by 1.1 just to make sure there is enough time to fade completely to black
    }

    protected override void Awake() {
        base.Awake();
        GetScores();                    // Start Loading all the Scores from each scene in thr build index and add it to the scores array
    }

    private void GetScores() {
        int buildCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 1; i < buildCount; i++) {      // start at i = 1 to skip the main menu scene
            string sceneFilePath = SceneUtility.GetScenePathByBuildIndex(i);
            
                
            //Scene scene = SceneManager.GetSceneByBuildIndex(0);
            string sceneName = Path.GetFileNameWithoutExtension(sceneFilePath);
            SaveDataManager.LoadJsonData(this, sceneName);
        }
    }

    // Loads the specified scene. with a delay time before loading. Normally to give time for the screen to fade to black
    private IEnumerator LoadScene(int sceneID, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneID);      // Load Level 1

    }

    public void SelectLevel() {
        UIMainMenuManager.Instance.ScreenFadeOutIn();
        UIMainMenuManager.Instance.Invoke("DisableMainMenuScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);  // add a little extra time to make sure the screen is completely black before enabling/disabling
        UIMainMenuManager.Instance.Invoke("EnableSelectLevelScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);
        UIMainMenuManager.Instance.Level1Button.Select();
        //UIMainMenuManager.Instance.


    }
    public void Options() {
        UIMainMenuManager.Instance.ScreenFadeOutIn();
        UIMainMenuManager.Instance.Invoke("DisableMainMenuScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);
        UIMainMenuManager.Instance.Invoke("EnableOptionsScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);
        UIMainMenuManager.Instance.OptionsBackButton.Select();
    }
    public void ExitGame() {
        Application.Quit();
    }

    public void Back() {
        UIMainMenuManager.Instance.ScreenFadeOutIn();
        UIMainMenuManager.Instance.Invoke("EnableMainMenuScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);
        UIMainMenuManager.Instance.Invoke("DisableSelectLevelScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);
        UIMainMenuManager.Instance.Invoke("DisableOptionsScreen", UIMainMenuManager.Instance.ScreenFadeTime * 1.1f);
        UIMainMenuManager.Instance.StartButton.Select();

    }

    // this method will be called by the select Level button. The will pass in the ID of the scene they want to load
    public void ChooseLevel(int sceneID) {
        UIMainMenuManager.Instance.ScreenFade(0, 1);
        StartCoroutine(LoadScene(sceneID, UIMainMenuManager.Instance.ScreenFadeTime * 1.1f));     // multiply by 1.1 just to make sure there is enough time to fade completely to black
    }

    public void PopulateSaveData(SaveData saveData) {
        throw new System.NotImplementedException();
    }

    public void LoadFromSaveData(SaveData saveData) {
        Scores scores = new Scores();
        scores.resetScore = saveData.ResetScore;
        scores.coinScore = saveData.CoinScore;
        scores.timeScore = saveData.TimeScore;
        levelScores.Add(scores);

    }
}
