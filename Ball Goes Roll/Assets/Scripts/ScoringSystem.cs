using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    // The player can get points for the number of resets. The coins collected and the time. The player is graded from 0 to 1 on each area.
    

    [Header("Resets")]
    [SerializeField] private  int minResets = 0;     // the number resets the player is allowed before they start losing points
    [SerializeField] private  int maxResets = 10;    // if the player gets this or more resets they will get no points
    private float resetsScore = 0;

    [Header("Coins")]
    [SerializeField] private int minCoins = -1;      // if the player gets this no. of coins or lower they will get no points
    [SerializeField] private int maxCoins = -1;       // if the player gets this or more coins they will get max points. this will be set to the no.of coins in level by default
    private float coinsScore = 0;

    [Header("Time")]
    [SerializeField] private int minSeconds = 120;    // if the player takes this many secs or lower they will get max points
    [SerializeField] private int maxSeconds = 420;      // if the player takes this many secs or higher they will get no points  
    private float timeScore = 0;


    #region Properties
    public int MinResets {
        get { return minResets; }
        set { minResets = value; }
    }
    public int MaxResets {
        get { return maxResets; }
        set { maxResets = value; }
    }

    public int MinCoins {
        get { return minCoins; }
        set { minCoins = value; }
    }
    public int MaxCoins {
        get { return maxCoins; }
        set { maxCoins = value; }
    }

    public int MinSeconds {
        get { return minSeconds; }
        set { minSeconds = value; }
    }
    public int MaxSeconds {
        get { return maxSeconds; }
        set { maxSeconds = value; }
    } 
    #endregion



    // This method takes in the relevant information from the game manager and calculates the scores. It return Vector3 in the format(x=resetsScore,y=coinsScore,z=timeScore)
    public Scores CalculateScore(int resets, int coinsCollected, int totalCoins, float timeInSecs) {
        resetsScore = CalculateResetsScore(resets);
        coinsScore = CalculateCoinsScore(coinsCollected, totalCoins);
        timeScore = CalculateTimeScore(timeInSecs);
        //return new Vector3(resetsScore, coinsScore, timeScore);
        return new Scores(resetsScore, coinsScore, timeScore);


    }
    public float CalculateResetsScore(int resets) {
        
        if (resets <= minResets) {      // if the player's resets are equal to or less than the minimum resets then the player gets full points
            return 1;
        }
        else if (resets >= maxResets) {  // if the player's resets are greater the maximum resets then the player gets no points
            return 0;
        }
        else {   // The player is within the min - max bounds. the points they recieve lerps between 0 and 1 based on the number of resets they used
            int minMaxDifference = maxResets - minResets;
            int resetsBetweenMinMax = resets - minResets;               // all resets before the min value shouldnt coint towards decreasing the score
            float score = (float)resetsBetweenMinMax / (float)minMaxDifference;      // the proportion of "allowed" resets the player used
            return 1 -score;
        }
        
    }

    public float CalculateCoinsScore(int coinsCollected, int totalCoins) {
        if (minCoins == -1) {           // if the user has not specified the minCoins. the use 0 as default
            minCoins = 0;
        }
        if (maxCoins == -1) {           // if the user has not specified the maxCoins. the use the total coins available
            maxCoins = totalCoins;
        }
        if (maxCoins>totalCoins) {
            maxCoins = totalCoins;
        }
        if (coinsCollected <= minCoins) {      // if the player's equal to or less than the minimum coins then the player gets no points
            return 0;
        }
        else if (coinsCollected >= maxCoins) {  // if the player greater the maximum coins then the player gets full points
            return 1;
        }
        else {   // The player is within the min - max bounds. the points they recieve lerps between 0 and 1 based on the number of coins they collected
            int minMaxDifference = maxCoins - minCoins;
            int coinsBetweenMinMax = coinsCollected - minCoins;                 // all coins collected before the min value shouldnt count towards decreasing the score
            float score = (float)coinsBetweenMinMax / (float)minMaxDifference;      // the proportion of coins the player collected
            return score;
        }
    }

    public float CalculateTimeScore(float timeInSecs) {
        if (timeInSecs <= minSeconds) {      // if the player's time is equal to or less than the minimum time then the player gets full points
            return 1;
        }
        else if (timeInSecs >= maxSeconds) {  // if the player's time are greater the maximum time then the player gets no points
            return 0;
        }
        else {   // The player is within the min - max bounds. the points they recieve lerps between 0 and 1 based on the number of resets they used
            float minMaxDifference = maxSeconds - minSeconds;
            float timeBetweenMinMax = timeInSecs - minSeconds;                 // all time collected before the min value shouldnt count towards decreasing the score

            float score = timeBetweenMinMax / minMaxDifference;      // the proportion of "allowed" resets the player used
            return 1- score;
        }
    }

    // Takes in the current record and the new score and compares them Returns true if the record  score has been changed and will need to be saved
    public bool CheckIfNewScoreRecord(ref Scores currentRecord, Scores newScore) {
        Scores newRecord = currentRecord;        // this will be update with the higher scores if they exist
        bool isRecordBeaten = false;
        // if any of the new scores are greater than the record score then the record should be updated and the score should be saved
        if (newScore.resetScore > currentRecord.resetScore) {
            newRecord.resetScore = newScore.resetScore;
            isRecordBeaten = true;
        }
        if (newScore.coinScore > currentRecord.coinScore) {
            newRecord.coinScore = newScore.coinScore;
            isRecordBeaten = true;

        }
        if (newScore.timeScore > currentRecord.timeScore) {
            newRecord.timeScore = newScore.timeScore;
            isRecordBeaten = true;
        }
        return isRecordBeaten;
    }
}
public struct Scores {
    public float resetScore;
    public float coinScore;
    public float timeScore;

    public Scores(float resetScore, float coinScore, float timeScore) {
        this.resetScore = resetScore;
        this.coinScore = coinScore;
        this.timeScore = timeScore;
    }
}
