using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDataManager {
    public static void SaveJsonData(Scores scores, GameManager gameManager, string fileName) {
        //SaveData saveData = new SaveData(scores);
        string fileExtension = ".dat";
        SaveData saveData = new SaveData();
        //gameManager.PopulateSaveData(saveData);
        saveData.ResetScore = scores.resetScore;
        saveData.CoinScore = scores.coinScore;
        saveData.TimeScore = scores.timeScore;

        if (FileManager.WriteToFile(fileName.ToString() + fileExtension.ToString(), saveData.ToJson())) {
            Debug.Log("SaveSuccesful");
        }

    }

    public static void LoadJsonData(GameManager gameManager, string fileName) { 

        string fileExtension = ".dat";

        if (FileManager.LoadFromFile(fileName.ToString() + fileExtension.ToString(), out var json)) {
            SaveData saveData = new SaveData();
            saveData.LoadFromJson(json);
            gameManager.LoadFromSaveData(saveData);
            Debug.Log("LoadComplete");
        }
    }

    public static void LoadJsonData(MainMenuManager mainMenuManager, string fileName) {

        string fileExtension = ".dat";

        if (FileManager.LoadFromFile(fileName.ToString() + fileExtension.ToString(), out var json)) {
            SaveData saveData = new SaveData();
            saveData.LoadFromJson(json);
            mainMenuManager.LoadFromSaveData(saveData);
            Debug.Log("LoadComplete");
        }
        else {
            mainMenuManager.LoadFromSaveData(new SaveData());
            Debug.Log("Empty save Data class created for level with no save file");
        }
    }
}
