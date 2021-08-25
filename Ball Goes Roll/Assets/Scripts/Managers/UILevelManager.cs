using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




public class UILevelManager : MonoSingleton<UILevelManager> {
    // SEPARATE SCREENS ON CANVAS
    [Header("Separate Screen On Canvas")]
    [SerializeField] private GameObject hudScreen;               // the HUD Game object on the canvas
    [SerializeField] private GameObject levelCompleteScreen;     // the Level Complete Screen Object onnt eh canvas
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject controlsScreen;


    // HUD VARIABLES
    [Header("HUD Variables")]
    [SerializeField] private TextMeshProUGUI resetText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject skipCheckpointText;
    [SerializeField] private Image skipCheckpointImage;
    [SerializeField] private GameObject interactPanel;

    [Header("HUD Slider Variables")]
    private Slider resetsSlider;
    private Slider coinsSlider;
    private Slider timeSlider;

    private Image resetFillImage;
    private Image coinFillImage;
    private Image timeFillImage;



    //LEVEL COMPLETE VARIABLES
    [Header("Level Complete Variables")]
    [SerializeField] private TextMeshProUGUI resetScoreText;        // displays the same values as the other reset/coins/time textbox but on the level complete screen
    [SerializeField] private TextMeshProUGUI coinsScoreText;
    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private Button defaultSelectedButton;          // the button which will be selected by default when the Level Completes screen comes on

    [SerializeField] private float fillStarWaitTime = 2;            // The time between the UI displaying and the stars begining to fill
    [SerializeField] private float starFillTime = 2;                // The time it takes to fill up a full star
    [SerializeField] private float waitBetweenStarsTime = 1;        // The time waited between filling one star and the next

    [SerializeField] HighScoreStars recordStars;                  // the stars which show the players high score for the level
    // The star images on the level Complete Screen
    private Image resetsStarImage;
    private Image coinsStarImage;
    private Image timeStarImage;

    [Header("Options Variables")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button controlsBackButton;
    //[SerializeField] private MixLevels mixLevels;


    // COMMON VARIABLE
    [Header("Common Variables")]
    [SerializeField] private Image solidColourImage;

    [SerializeField] private float screenFadeTime = 0.25f;       // the time it will take the screen to fade to black. and the from black back in
    private bool isFading = false;



    #region Properties
    public float ScreenFadeTime {
        get { return screenFadeTime; }
    }
    public GameObject HudScreen {
        get { return hudScreen; }
    }
    public GameObject LevelCompleteScreen {
        get { return levelCompleteScreen; }
    }
    public GameObject InteractPanel {
        get { return interactPanel; }
    }
    public Button DefaultSelectedButton {
        get { return defaultSelectedButton; }
    }
    #endregion


    protected override void Awake() {
        base.Awake();

        hudScreen.SetActive(true);
        LevelCompleteScreen.SetActive(true);
        optionsScreen.SetActive(true);

        //Set HUD Fill Images
        resetFillImage = GameObject.Find("Reset Fill Image").GetComponent<Image>();
        coinFillImage = GameObject.Find("Coin Fill Image").GetComponent<Image>();
        timeFillImage = GameObject.Find("Time Fill Image").GetComponent<Image>();
        if (resetFillImage == null || coinFillImage == null || timeFillImage == null) {
            Debug.LogError("UILevelManager: Problem Finding the Fill Image on the HUD Sliders");
        }

        //Set Level Complete Fill Images
        resetsStarImage = GameObject.Find("Resets Star Image").GetComponent<Image>();
        coinsStarImage = GameObject.Find("Coins Star Image").GetComponent<Image>();
        timeStarImage = GameObject.Find("Time Star Image").GetComponent<Image>();
        if (resetsStarImage == null || coinsStarImage == null || timeStarImage == null) {
            Debug.LogError("UILevelManager: Problem Finding the Fill Image on the Level Complete Screen");
        }

        //Set HUD Sliders
        resetsSlider = GameObject.Find("Resets Slider").GetComponent<Slider>();
        coinsSlider = GameObject.Find("Coins Slider").GetComponent<Slider>();
        timeSlider = GameObject.Find("Time Slider").GetComponent<Slider>();
        if (resetsSlider == null || coinsSlider == null || timeSlider == null) {
            Debug.LogError("UILevelManager: Problem Finding the Sliders Image on the HUD");
        }

        //Set Options Screen Volume Sliders
        SetOptionsSliders();

        LevelCompleteScreen.SetActive(false);
        optionsScreen.SetActive(false);

    }



    // This method controls the sequence that occurs when the level is completed
    public IEnumerator LevelCompleteSequence(int resets, int coins, int totalCoinCount, string timeAsString, Scores currentScores, Scores oldScores) {
        recordStars.SetStarsScoreSingle(oldScores);         // Set the high score stars values. Using the oldScores to make sure that the scores from the run the player
                                                            //.. has just do not end up being displayed  
        ScreenFadeOutIn();

        yield return new WaitForSeconds(screenFadeTime * 1.1f);          // Give the screen time to fade out before changing the UI
        LevelCompleteSetText(resets, coins, totalCoinCount, timeAsString);
        hudScreen.SetActive(false);
        levelCompleteScreen.SetActive(true);
        defaultSelectedButton.Select();         // select the default button. Might want to change this so it hapens after the star animations
        // Do Star Stuff
        // Wait a period of time before filling up stars
        yield return new WaitForSeconds(fillStarWaitTime);


        // FILL RESETS STAR -if the reset score is greater than 0
        if (currentScores.resetScore > 0) {
            yield return StartCoroutine(FillStar(resetsStarImage, currentScores.resetScore));
            yield return new WaitForSeconds(waitBetweenStarsTime);
        }
        //FILL COIN STAR
        if (currentScores.coinScore > 0) {
            yield return StartCoroutine(FillStar(coinsStarImage, currentScores.coinScore));
            yield return new WaitForSeconds(waitBetweenStarsTime);
        }
        //FILL TIME STAR
        if (currentScores.timeScore > 0) {
            yield return StartCoroutine(FillStar(timeStarImage, currentScores.timeScore));
            yield return new WaitForSeconds(waitBetweenStarsTime);
        }

        
    }

    // this method is for the coins where the score is higher the more coins where collected
    private IEnumerator FillStar(Image star, float value) {
        float time = 0;
        star.fillAmount = 0;
        while (star.fillAmount < value) {
            yield return null;      // do this first so that if the star is filled up higher that the value there is not a frame to wait before leaving the while loop
            star.fillAmount = Mathf.Lerp(0, 1, time / starFillTime);
            time += Time.deltaTime;

        }
        star.fillAmount = value;
    }
    /*// This method is for the Resets and Time where the score is higher the lower the reset count or time was
    private IEnumerator FillStarFlipped(Image star, float value) {
        if (value == 0) {

        }
        float time = 0;
        star.fillAmount = 0;
        while (star.fillAmount < value) {
            yield return null;      // do this first so that if the star is filled up higher that the value there is not a frame to wait before leaving the while loop
            star.fillAmount = Mathf.Lerp(0, 1, time / starFillTime);
            time += Time.deltaTime;

        }
        star.fillAmount = value;
    }*/

    // This method will fade out to black and then fade into the level complete screen
    // It will Set the default Selected Button
    // It takes in arguments from the GameManager and display the reset/coins/
    private void LevelCompleteSetText(int resets, int coinsCollected, int totalCoinCount, string timeAsString) {
        resetScoreText.SetText("Resets: " + resets);
        coinsScoreText.SetText("Coins: " + coinsCollected + "/" + totalCoinCount);
        timeScoreText.SetText("Time: " + timeAsString);
    }


    //THis method is called by the Retry, Main Menu, and the Next Level Buttons. They pass in an int which distinguishes between the 3. The screen fade and after an amout of
    // the Game manager is asked to change the Scene
    public void GoToLevel(int buttonType) {
        StartCoroutine(GoToLevelRoutine(buttonType));
    }
    private IEnumerator GoToLevelRoutine(int buttonType) {
        ScreenFade();       // Fade
        yield return new WaitForSeconds(ScreenFadeTime * 1.1f);
        GameManager.Instance.LoadScene(buttonType);
    }

    #region Screen Fade
    // FADE IN AND OUT
    /// <summary>
    /// Fade the screen "mask" from one alpha value to another. Can be used to fade out to black and then fade from black back in
    /// </summary>
    public void ScreenFade(int fromAlpha = 0, int toAlpha = 1) {
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
    #endregion


    // SET TEXT
    public void SetResetText(int resetCounter) {
        //resetText.SetText("Resets: " + resetCounter);
        resetText.SetText(resetCounter.ToString());

    }
    public void SetCoinText(int coinsCollected, int totalCoinCount) {
        //coinsText.SetText("Coins: " + coinsCollected + "/" + totalCoinCount);
        coinsText.SetText(coinsCollected + "/" + totalCoinCount);

    }

    public void SetTimeText(string time) {
        //timeText.SetText("Time: " + time);
        timeText.SetText(time);

    }


    #region DISABLE OBJECTS
    // ENABLE / DISABLE OBJECTS
    public void EnableSkipCheckpointText() {
        skipCheckpointText.gameObject.SetActive(true);
        CancelInvoke("DisableSkipCheckpointText");
        Invoke("DisableSkipCheckpointText", 5);
    }
    public void DisableSkipCheckpointText() {
        skipCheckpointText.gameObject.SetActive(false);
    }
    public void EnableInputImage() {
        CancelInvoke("DisableInputImage");
        Invoke("DisableInputImage", 5);
        skipCheckpointImage.gameObject.SetActive(true);
    }
    public void DisableInputImage() {
        skipCheckpointImage.gameObject.SetActive(false);
    }
    #endregion

    // SET FILL IMAGE VALUE
    public void SetSkipCheckpointFillImage(float value) {
        skipCheckpointImage.fillAmount = value;
    }

    #region HUD SLIDERS
    // === SET HUD SLIDERS ===
    public void SetResetSlider(float value) {
        if (value == 1) {
            resetsSlider.value = value;
        }
        else if (value == 0) {
            resetsSlider.value = value;
        }
        else {
            resetsSlider.value = value;
            //resetsSlider.value = 1 - value;      // this account for the fact that the slider starts full an gradually empties as the resets increases. This is different from  
            // the star in the level complete screen which starts empty and fills up 
            resetFillImage.color = Color.Lerp(Color.red, Color.green, resetsSlider.value);
        }

    }
    public void SetCoinSlider(float value) {
        coinsSlider.value = value;
        coinFillImage.color = Color.Lerp(Color.red, Color.green, coinsSlider.value);

    }
    public void SetTimeSlider(float value) {
        if (value == 1) {
            timeSlider.value = value;
        }
        else if (value == 0) {
            timeSlider.value = value;
        }
        else {
            timeSlider.value = value;
            //timeSlider.value = 1 - value;
            timeFillImage.color = Color.Lerp(Color.red, Color.green, timeSlider.value);
        }
    }
    #endregion

    #region OPTIONS SCREEN
    // ==== START OPTIONS SCREEN CODE ===
    private void SetOptionsSliders() {

    }

    public void ControlsButton() {
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(true);
        controlsBackButton.Select();

    }

    public void MainMenuButton() {
        Time.timeScale = 1;
        GameManager.Instance.LoadScene(1);
    }

    public void ExitButton() {
        GameManager.Instance.ExitGame();
    }
    public void BackButton() {
        optionsScreen.SetActive(true);
        controlsScreen.SetActive(false);
        masterVolumeSlider.Select();

    }
    public void PauseGame() {
        hudScreen.SetActive(false);
        optionsScreen.SetActive(true);
        masterVolumeSlider.Select();
        PlayerSingleton.Instance.PlayerController.enabled = false;
        //mixLevels.SetToPauseSnapshot();
    }
    public void UnpauseGame() {
        hudScreen.SetActive(true);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        PlayerSingleton.Instance.PlayerController.enabled = true;

        //mixLevels.SetToPlaySnapshot();



    }

    // === END OPTIONS SCREEN CODE === 
    #endregion

}
