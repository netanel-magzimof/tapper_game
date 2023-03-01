using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class ScoreAndLivesManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Text scoreLabel;
    [SerializeField] private Text highScoreLabel;
    [SerializeField] public GameObject[] LImages;
    #endregion
    
    #region Fields
    private const string KEY_HIGH_SCORE = "high-score";
    public static ScoreAndLivesManager Shared { get; private set; }
    private int score;
    private const int SCORE_EMPTY_MUG = 100;
    private const int SCORE_DRUNK = 50;
    private int lives = 2;
    private int highScore;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        Shared = this;
        score = 0;
        highScore = PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0);
        highScoreLabel.text = $"HIGH SCORE: {highScore}";
    }
    #endregion
    #region Methods
    public void LostRound()
    {
        lives--;
        if (lives >= 0)
        {
            LImages[lives].SetActive(false);
        }
    }

    public void AddDrunkScore()
    {
        AddScore(SCORE_DRUNK);
    }

    public void AddEmptyMugScore()
    {
        AddScore(SCORE_EMPTY_MUG);
    }
    private void AddScore(int amount)
    {
        score += amount;
        scoreLabel.text = score.ToString();
        if (score>highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(KEY_HIGH_SCORE, highScore);
            highScoreLabel.text = $"HIGH SCORE: {highScore}";
        }
    }

    public int getScore()
    {
        return score;
    }
    #endregion
}
