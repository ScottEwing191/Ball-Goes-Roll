using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {
    [SerializeField] private float resetScore;
    [SerializeField] private float coinScore;
    [SerializeField] private float timeScore;

    #region Properties
    public float ResetScore {
        get { return resetScore; }
        set { resetScore = value; }
    }


    public float CoinScore {
        get { return coinScore; }
        set { coinScore = value; }
    }

    public float TimeScore {
        get { return timeScore; }
        set { timeScore = value; }
    }
    #endregion

    /*#region Contructors
    public SaveData() {

    }
    public SaveData(float resetScore, float coinScore, float timeScore) {
        this.resetScore = resetScore;
        this.coinScore = coinScore;
        this.timeScore = timeScore;
    }
    public SaveData(Vector3 scores) {
        this.resetScore = scores.x;
        this.coinScore = scores.y;
        this.timeScore = scores.z;
    } 
    #endregion*/

    public string ToJson() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json) {
        JsonUtility.FromJsonOverwrite(json, this);
    }

}
public interface ISaveable {
    void PopulateSaveData(SaveData saveData);
    void LoadFromSaveData(SaveData saveData);

}
