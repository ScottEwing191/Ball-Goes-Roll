using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuManager : MonoSingleton<UIMainMenuManager> {

    // Menu Screens
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject selectLevelScreen;
    [SerializeField] private GameObject optionsScreen;

    [SerializeField] HighScoreStars highScoreStars;

    [SerializeField] private Image solidColourImage;
    [SerializeField] private float screenFadeTime = 0.25f;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsBackButton;


    private bool isFading = false;

    // Properties

    public GameObject MainMenuScreen {
        get { return mainMenuScreen; }
    }
    public GameObject SelectLevelScreen {
        get { return selectLevelScreen; }
    }
    public GameObject OptionsScreen {
        get { return optionsScreen; }
    }
    public float ScreenFadeTime {
        get { return screenFadeTime; }
    }
    public Button Level1Button {
        get { return level1Button; }
    }
    public Button StartButton {
        get { return startButton; }
    }
    public Button OptionsBackButton {
        get { return optionsBackButton; }
    }



    #region Enable/ Disable Screens
    // Main Menu Screen
    public void EnableMainMenuScreen() {
        mainMenuScreen.SetActive(true);
    }
    public void DisableMainMenuScreen() {
        mainMenuScreen.SetActive(false);
    }
    // Select Level Screen
    public void EnableSelectLevelScreen() {
        selectLevelScreen.SetActive(true);
    }
    public void DisableSelectLevelScreen() {
        selectLevelScreen.SetActive(false);
    }
    // Options Screen
    public void EnableOptionsScreen() {
        optionsScreen.SetActive(true);
    }
    public void DisableOptionsScreen() {
        optionsScreen.SetActive(false);
    }
    #endregion



    private void Start() {
        highScoreStars.SetStarsScoreFull(MainMenuManager.Instance.LevelScores.ToArray());
    }

    /// <summary>
    /// Fade the screen "mask" from one alpha value to another. Can be used to fade out to black and then fade from black back in
    /// </summary>
    public void ScreenFade(int fromAlpha, int toAlpha) {
        if (solidColourImage == null) {
            return;
        }
        if (!isFading) {
            fromAlpha = Mathf.Clamp(fromAlpha, 0, 1);
            toAlpha = Mathf.Clamp(toAlpha, 0, 1);
            StartCoroutine(ScreenFadeRoutine(fromAlpha, toAlpha, 0));
        }
    }

    // One method to fade the screen out then in. Also be used to fade in then out
    public void ScreenFadeOutIn(int fromAlpha = 0, int toAlpha = 1) {
        if (solidColourImage == null) {
            return;
        }
        if (!isFading) {
            fromAlpha = Mathf.Clamp(fromAlpha, 0, 1);
            toAlpha = Mathf.Clamp(toAlpha, 0, 1);
            StartCoroutine(ScreenFadeRoutine(fromAlpha, toAlpha, 0));
            StartCoroutine(ScreenFadeRoutine(toAlpha, fromAlpha, 2 * screenFadeTime));        // alpha vlaues passed in are swapped so that the screen opposite to what it did before
        }
    }

    private IEnumerator ScreenFadeRoutine(int fromAlpha, int toAlpha, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        isFading = true;
        float time = 0;
        // FADE OUT from ball to opaque colour
        Color newColor = solidColourImage.color;
        while (time <= screenFadeTime) {
            newColor.a = Mathf.Lerp(fromAlpha, toAlpha, time / screenFadeTime);
            solidColourImage.color = newColor;
            time += Time.deltaTime;
            yield return null;
        }
        newColor.a = toAlpha;
        solidColourImage.color = newColor;
        isFading = false;
    }
}
