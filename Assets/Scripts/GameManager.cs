using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] private BarTender barTender;
    [SerializeField] private Image getReadyImage;
    [SerializeField] private Text gameOver;
    [SerializeField] private int amountOfDrunksInTheBeginning;
    #endregion

    #region Enums
    public enum DrinkColors { Yellow, Purple, Green, Red }
    #endregion
    
    #region Fields

    private AudioSource audioSource;
    private const float getReadyLength = 2f;
    private bool shouldSpawn;
    private float nextTimeToSpawnDrunk;
    private int lives = 2;
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        shouldSpawn = false;
        getReadyImage.enabled = true;
        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        yield return new WaitForSeconds(getReadyLength);
        getReadyImage.enabled = false;
        for (int i = 0; i < amountOfDrunksInTheBeginning; i++) DrunksPool.Shared.Get();
        nextTimeToSpawnDrunk = Random.Range(2f, 3f);
        shouldSpawn = true;
        barTender.StartPlaying();
    }


    void Update()
    {
        nextTimeToSpawnDrunk -= Time.deltaTime;
        if (shouldSpawn && nextTimeToSpawnDrunk <= 0)
        {
            DrunksPool.Shared.Get();
            nextTimeToSpawnDrunk = Random.Range(2f, 3f);
        }
    }

    public void FullMugReachedEnd()
    {
        RoundLost();
        barTender.MugFell();
    }

    public void DrunkReachedEnd(GameObject drunk)
    {
        barTender.DrunkReachedEnd(drunk);
        RoundLost();
        transform.localScale = new Vector3(-1, 1, 1);
    }

    public void EmptyMugReachedEnd()
    {
        RoundLost();
        barTender.MugFell();
    }

    private void RoundLost()
    {
        audioSource.Play();
        shouldSpawn = false;
        MugPool.Shared.FreezeAll();
        DrunksPool.Shared.FreezeAll();
        lives--;
        if (lives < 0)
        {
            gameOver.enabled = true;
            StartCoroutine(GameOver());
        }
        else
        {
            ScoreAndLivesManager.Shared.LostRound();
            StartCoroutine(ResetToNewRound());
        }
    }

    private IEnumerator ResetToNewRound()
    {
        yield return new WaitForSeconds(2.5f);
        getReadyImage.enabled = true;
        yield return new WaitForSeconds(getReadyLength);
        getReadyImage.enabled = false;
        ResetRound();
    }

    private void ResetRound()
    {
        DrunksPool.Shared.ReleaseAll();
        MugPool.Shared.ReleaseAll();
        barTender.transform.parent = null;
        barTender.enabled = true;
        barTender.StartPlaying();
        shouldSpawn = true;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
