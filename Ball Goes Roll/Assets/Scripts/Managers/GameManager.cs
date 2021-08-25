using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>, ISaveable {

    [SerializeField] private ScoringSystem scoringSystem;
    private int resetCounter = 0;
    private float timeOffset = 0;   // the time offset from Time.time that you wish to display. Decrease the time offset to increase the displayed time
    private float timeInSecs = 0;
    private bool runTimer = true;
    private GameObject[] coins;
    private int totalCoinCount;
    private int coinsCollected;
    private bool isLevelCompleted = false;
    private bool isGamePaused = false;
    private Scores currentScoresRecord;             // the current record for the scores on this level
    private Scores oldScoresRecord;                 // Just to keep a copy of the old record once the new one is updated. Might not be needed
    private Scores currentScores;                    // the score the player has achieved at the end of the level

    private bool checkpointSkipped = false;         // if the player has skipped a checkpoint then the score for this run should be invalid. NOT IMPLEMENTED YET

    public bool RunTimer {
        get { return runTimer; }
        set { runTimer = value; }
    }
    public bool IsGamePaused {
        get { return isGamePaused; }
    }
    /*public bool IsLevelCompleted {
        get { return isLevelCompleted; }
    }*/

    protected override void Awake() {
        base.Awake();
        /*scoringSystem = FindObjectOfType<ScoringSystem>();
        if (scoringSystem == null) {
            Debug.LogError("Scoring System Not Found");
        }*/

    }
    // Start is called before the first frame update
    void Start() {
        InitializeCoins();
        UILevelManager.Instance.SetResetText(resetCounter);      // Set reset ball text on startup

        StartTimer();
        // Set the slider values on startup
        UILevelManager.Instance.SetResetSlider(scoringSystem.CalculateResetsScore(resetCounter));
        UILevelManager.Instance.SetCoinSlider(scoringSystem.CalculateCoinsScore(coinsCollected, totalCoinCount));
        UILevelManager.Instance.SetTimeSlider(scoringSystem.CalculateTimeScore(timeInSecs));

        SaveDataManager.LoadJsonData(this, SceneManager.GetActiveScene().name);     // gets the data from file and sets the current best scores variable
        //print("Resets Score: " + currentScoresRecord.resetScore);
        //print("Coins Score: " + currentScoresRecord.coinScore);
        //print("Time Score: " + currentScoresRecord.timeScore);
        
    }

    private void Update() {
        DoPauseGame();


    }

    // PAUSE GAME
    private void DoPauseGame() {
        if (Input.GetButtonDown("Pause") && !isLevelCompleted) {
            //Pause Game
            if (!isGamePaused) {
                Time.timeScale = 0;
                UILevelManager.Instance.PauseGame();
                isGamePaused = true;
            }
            // UnPause Game
            else {
                Time.timeScale = 1;
                UILevelManager.Instance.UnpauseGame();
                isGamePaused = false;
            }
        }
    }

    //=== COINS START ===
    private void InitializeCoins() {
        coins = GameObject.FindGameObjectsWithTag("Coin");
        totalCoinCount = coins.Length;
        coinsCollected = 0;
        UILevelManager.Instance.SetCoinText(coinsCollected, totalCoinCount);
    }

    public void CollectCoin() {
        coinsCollected++;
        UILevelManager.Instance.SetCoinText(coinsCollected, totalCoinCount);
        UILevelManager.Instance.SetCoinSlider(scoringSystem.CalculateCoinsScore(coinsCollected, totalCoinCount));
    }
    //=== COINS END ===

    //=== RESET BALL START ===
    public void ResetBall() {
        resetCounter++;
        UILevelManager.Instance.SetResetText(resetCounter);
        UILevelManager.Instance.SetResetSlider(scoringSystem.CalculateResetsScore(resetCounter));

    }

    private void StartTimer() {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer() {
        timeOffset = Time.time;
        while (runTimer) {
            timeInSecs = Time.time - timeOffset;
            UILevelManager.Instance.SetTimeText(GetTimeAsString(timeInSecs));
            UILevelManager.Instance.SetTimeSlider(scoringSystem.CalculateTimeScore(timeInSecs));


            yield return new WaitForSeconds(1);
        }
    }
    // Takes in the time in seconds and converts it to a string in the format MM:SS
    private string GetTimeAsString(float timeInSeconds) {
        int minutes = (int)timeInSecs / 60;
        int seconds = (int)timeInSecs % 60;
        return "" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
    //==== RESET BALL END ===

    //=== LEVEL COMPLETE START ===
    public void LevelComplete() {
        //print("LEVEL COMPLETE");
        isLevelCompleted = true;
        runTimer = false;       // stop the timer
        PlayerSingleton.Instance.PlayerController.enabled = false;
        // start the routine which will get the UI to fade out/in, display the players resets, coins, time and show the player how many stars they got
        
        currentScores = scoringSystem.CalculateScore(resetCounter, coinsCollected, totalCoinCount, timeInSecs);        // Get the Scores for the Level

        // Start the routine which will handle fading to the UI screen, Displaying resets,coins,time info and display the stars filling up
        // The old scores record is passed in because i want to display the previous high scores to the player but by this point in the program the currentScoresRecord..
        // .. variable has been updated with the currentScores value 
        UILevelManager.Instance.StartCoroutine(UILevelManager.Instance.LevelCompleteSequence(resetCounter, coinsCollected, totalCoinCount, GetTimeAsString(timeInSecs), currentScores, oldScoresRecord));


        
        

        //Save Score
        if (scoringSystem.CheckIfNewScoreRecord(ref currentScoresRecord, currentScores)) {      // check if the new score beats the old score

            SaveDataManager.SaveJsonData(currentScores, this, SceneManager.GetActiveScene().name);
        }

    }
    //=== LEVEL COMPLETE END ===

    // LOAD LEVEL
    public enum ButtonType { Retry, MainMenu, NextLevel }
    public void LoadScene(int buttonType) {
        switch ((ButtonType)buttonType) {
            case ButtonType.Retry:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case ButtonType.MainMenu:
                SceneManager.LoadScene(0);

                break;
            case ButtonType.NextLevel:
                // Load the next scene if it exists otherwise load the main menu
                int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
                //SceneManager.sceneCountInBuildSettings;
                if (currentBuildIndex < SceneManager.sceneCountInBuildSettings - 1) {
                    SceneManager.LoadScene(currentBuildIndex + 1);
                }
                else {
                    SceneManager.LoadScene(0);
                }
                break;
            default:
                Debug.LogError("Wrong int is entered on the Retry, Main Menu, or Next Level Buttons");
                break;
        }
    }

    public void ExitGame() {
        Application.Quit();
    }



    // === SAVING AND LOADING START ===
    /*private static void SaveJsonData(Vector3 scores, GameManager gameManager) {
        //SaveData saveData = new SaveData(scores);
        SaveData saveData = new SaveData();
        //gameManager.PopulateSaveData(saveData);
        saveData.ResetScore = scores.x;
        saveData.CoinScore = scores.y;
        saveData.TimeScore = scores.z;
        string fileName = SceneManager.GetActiveScene().name;
        string dataString = saveData.ToJson();
        if (FileManager.WriteToFile(fileName, dataString)) {
            Debug.Log("SaveSuccesful");
        }

    }*/
    public void PopulateSaveData(SaveData saveData) {
        throw new System.NotImplementedException();
    }



    public void LoadFromSaveData(SaveData saveData) {

        currentScoresRecord.resetScore = saveData.ResetScore;
        currentScoresRecord.coinScore = saveData.CoinScore;
        currentScoresRecord.timeScore = saveData.TimeScore;
        oldScoresRecord = currentScoresRecord;
    }

    // === SAVING AND LOADING END ===
}
