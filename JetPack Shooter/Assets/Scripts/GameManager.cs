using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] Slider healthBar;
    [SerializeField] float healthBarChangeTime = 0.5f;
    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] PlayerController playerController;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] Text scoreText;
    [SerializeField] GameObject calibrateScreen;
    [SerializeField] AudioClip background;
    [SerializeField] AdsManager ads;
    [SerializeField] Sprite audioSprite;
    [SerializeField] Sprite muteSprite;
    [SerializeField] Button audioButton;

    public bool audioOn = true;
    AudioSource audioSource;
    bool isDead = false;
    int difficultyLevel = 1;
    int maxDifficultyLevel = 10;
    int scoreToNextLevel = 10;
    float score;
    void Start()
    {
        ads.HideBanner();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void Mute()
    {
        if(audioOn)
        {
            audioButton.image.sprite= muteSprite;
            //off all audio
            playerController.audioSource.Stop();
            audioSource.Pause();
            audioOn = false;
        }
        else if(!audioOn)
        {

            audioButton.image.sprite = audioSprite;
            //on all audio
            playerController.audioSource.Play();
            audioSource.Play();
            audioOn = true;
        }
    }

    void LevelUp()
    {
        if (difficultyLevel == maxDifficultyLevel)
            return;

        scoreToNextLevel *= 2;
        difficultyLevel++;
        enemyManager.LevelUp();
    }

    public void Death()
    {
        isDead = true;
        if(PlayerPrefs.GetFloat("HighScore")<score)
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }
    public void ChangeHealthBar(int maxHealth, int currentHealth)
    {
        if (currentHealth < 0)
            return;

        if(currentHealth==0)
        {
            Invoke("OpenDeathMenu", healthBarChangeTime);
        }
        float healthPct = currentHealth / (float)maxHealth;
        StartCoroutine(SmooothHealthbarChange(healthPct));
    }

    IEnumerator SmooothHealthbarChange(float newFloat)
    {
        float elapsed = 0f;
        float oldFloat = healthBar.value; 
        while(elapsed<=healthBarChangeTime)
        {
            elapsed += Time.deltaTime;
            float cureentFillPct = Mathf.Lerp(oldFloat, newFloat, elapsed / healthBarChangeTime);
            healthBar.value = cureentFillPct;
            yield return null;
        }
    }
    public void OnFireButton()
    {
        playerController.FireBullets();
    }
    void Update()
    {
        if(isDead)
        {
            return;
        }

        if(score>=scoreToNextLevel)
        {
            LevelUp();
            EnemyLogic[] e = FindObjectsOfType<EnemyLogic>();
            foreach(EnemyLogic ei in e)
            {
                ei.LevelUp();
            }
        }
        score += (Time.deltaTime * difficultyLevel)/2;
        scoreText.text = ((int)score).ToString();
    }


    public void OnMenuButtonClicked()
    {
        Time.timeScale = 1f;
        ads.ShowInterstitial();
        SceneManager.LoadScene("Menu");
    }

    public void OnPauseButtonClicked()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        audioSource.Pause();
        ads.ShowBanner();
    }

    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        if (audioOn)
        {
            audioSource.Play();
        }
        ads.HideBanner();
    }

    public void OnRestartButtonClicked()
    {
        //save game;
        ads.ShowRewardedVideo();
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void OpenDeathMenu()
    {
        Time.timeScale = 0f;
        deathMenu.SetActive(true);
        ads.ShowBanner();
        audioSource.Pause();
    }

    public void Calibrate()
    {
        playerController.Calibrate();
        calibrateScreen.SetActive(true);
    }    

    public void BackCalibrate()
    {
        calibrateScreen.SetActive(false);
    }

}
