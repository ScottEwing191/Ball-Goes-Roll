using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTip : MonoBehaviour {
    [SerializeField] private Sprite[] tutorialImagesArray;        // an array of tutorial images     
    [SerializeField] private GameObject imageObject;
    [SerializeField] private Image tutorialImage;                              // the image component on the canvas will be displayed to the user
    private bool showingTip = false;

    private void Update() {
        print("Cancel");
        if (Input.GetButtonDown("Cancel")) {
            
        }    
        if (showingTip && Input.GetButtonDown("Cancel")) {
            StartCoroutine(TryHideTutorial());
        }
    }


    // Takes in the index of the image that is to be displayed
    public void DisplayTutorial(int imageIndex) {
        if (imageIndex < tutorialImagesArray.Length) {
            //Time.timeScale = 0;
            showingTip = true;

            UILevelManager.Instance.HudScreen.SetActive(false);
            PlayerSingleton.Instance.PlayerController.enabled = false;

            imageObject.SetActive(true);
            StartCoroutine(FadeImage());
            PlayerSingleton.Instance.PlayerController.StopBall();

            tutorialImage.sprite = tutorialImagesArray[imageIndex];
        }
        else {
            Debug.LogError("Input on tutorial trigger is exeeds tutorial images array bounds");
        }

    }
    public IEnumerator TryHideTutorial() {
        Time.timeScale = 1;
        showingTip = false;

        yield return StartCoroutine(FadeImage(true));
        UILevelManager.Instance.HudScreen.SetActive(true);
        imageObject.SetActive(false);
        PlayerSingleton.Instance.PlayerController.enabled = true;



    }
    // Fades in by default. Fades out if true is passed in
    private IEnumerator FadeImage(bool fadeOut = false) {
        float fromAlpha = 0;
        float toAlpha = 1;
        float fadeTime = 0.25f;
        if (fadeOut) {
            fromAlpha = 1;
            toAlpha = 0;
        }
        float time = 0;
        // FADE OUT from ball to opaque colour
        Color newColor = tutorialImage.color;
        while (time <= fadeTime) {
            newColor.a = Mathf.Lerp(fromAlpha, toAlpha, time / fadeTime);
            tutorialImage.color = newColor;
            time += Time.deltaTime;
            yield return null;
        }
        newColor.a = toAlpha;
        tutorialImage.color = newColor;
        //showingTip = true;

    }
}
