using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreStars : MonoBehaviour
{
    //[SerializeField] private Image[] starImages;        // stars must be placed in the order reset, coin time. And on the level Select Screen in the order of levels
    [SerializeField] StarImages[] starImagesArray;

    // Called from the level complete screen where there is only one set of high score stars to show
    public void SetStarsScoreSingle(Scores scores) {
        SetStarsScore(scores,starImagesArray[0]);
        

    }

    // Both the level complete screen on each and the level select screen from the main meny direct to this method.It sets the fill falue on the give images to he given score
    private void SetStarsScore(Scores scores, StarImages starImages ) {
        starImages.resetStarFill.fillAmount = scores.resetScore;
        starImages.coinStarFill.fillAmount = scores.coinScore;
        starImages.timeStarFill.fillAmount = scores.timeScore;

    }

    // This method is called from the level select screen since all the Scores for all levels are visable in one place
    public void SetStarsScoreFull(Scores[] levelScores) {
        if (levelScores.Length != starImagesArray.Length) {
            Debug.LogError("The number of level Scores does not match the number of level Stars.");
        }
        for (int i = 0; i < levelScores.Length; i++) {
            SetStarsScore(levelScores[i], starImagesArray[i]);
        }
    }
}
[System.Serializable]
public struct StarImages {
    [SerializeField] internal Image resetStarFill;
    [SerializeField] internal Image coinStarFill;
    [SerializeField] internal Image timeStarFill;

}
