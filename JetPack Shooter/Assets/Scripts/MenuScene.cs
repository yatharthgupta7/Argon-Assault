using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    int highscore;
    [SerializeField] Text score;
    [SerializeField] AdsManager ads;
    [SerializeField] Slider slider;
    [SerializeField] Text progressText;
    [SerializeField] GameObject loadingScreen;
    void Start()
    {
        highscore =(int) PlayerPrefs.GetFloat("HighScore");
        score.text = "HighScore : " + highscore;
        ads.ShowBanner();
    }
    public void ToGame()
    {
        StartCoroutine(LoadAsynchronously("Game"));
    }
    IEnumerator LoadAsynchronously(string scene)
    {
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while (!operation.isDone)
        {
            float progress = (operation.progress / .9f);
            slider.value = progress;
            progressText.text = ((int)progress * 100f) + " %";
            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
    void Update()
    {
        
    }
}
