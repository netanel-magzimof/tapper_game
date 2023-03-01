using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
[RequireComponent(typeof(Collider2D),typeof(Rigidbody2D),typeof(Animator))]
public class Drunk : MonoBehaviour
{
    #region Inspector
    [SerializeField] private float speed;
    [SerializeField] private SpriteRenderer wantedColorSprite;
    [SerializeField] private float speedUpBy;
    [SerializeField] private AudioClip catchMug, outOfDoor, gotIn;
    #endregion

    #region Fields

    private AudioSource audioSource;
    private Rigidbody2D physics;
    private bool shouldMove;
    public Animator animator;
    private Mug curMug;
    private bool toSendEmpty;
    private GameManager.DrinkColors wantedColor;

    #endregion
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        shouldMove = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // when drunk dont already have a mug and mug is full
        if (col.CompareTag("Mug") && curMug == null && !col.gameObject.transform.parent)
        {
            audioSource.clip = catchMug;
            audioSource.Play();
            Mug temp = col.gameObject.GetComponent<Mug>();
            if (wantedColor.Equals(temp.color))
            {
                ScoreAndLivesManager.Shared.AddDrunkScore();
                curMug = temp;
                curMug.CaughtByDrunk(this);
                animator.SetBool("GotMug", true);
            }
        }
        //when drunk reached end and dont want to drink anymore
        if (col.CompareTag("Respawn") && !toSendEmpty)
        {
            audioSource.clip = outOfDoor;
            audioSource.Play();
            if (curMug) curMug.Release();
            animator.SetBool("GotMug", false);

            DrunksPool.Shared.Release(this);
        }
        //when drunk reached end and want to drink anymore
        if (col.CompareTag("Respawn") && toSendEmpty)
        {
            toSendEmpty = Random.Range(1, 10) >= 8;
            animator.SetTrigger("Drink");
            if (curMug) curMug.Drinking();
            physics.velocity = Vector2.zero;
            shouldMove = false;
            StartCoroutine(EndDrinking());
        }
        //when drunk got to the end without getting a drink
        if (col.CompareTag("Finish"))
        {
            StartCoroutine(GotToEndOfLane());
        }
    }

    private IEnumerator EndDrinking()
    {
        yield return new WaitForSeconds(0.8f);
        if (curMug != null)curMug.ReturnEmpty();
        curMug = null;
        if (!animator.GetBool("Freeze"))
        {
            shouldMove = true;
        }
        animator.SetBool("GotMug", false);
        

    }
 
    private IEnumerator GotToEndOfLane()
    {
        yield return new WaitForSeconds(0.5f);
        transform.localScale = new Vector3(-0.55f, 0.55f, 1);
        physics.velocity = Vector2.left * 5;
    }

    private void Update()
    {
        if (shouldMove)
        {
            physics.velocity = Vector2.right * (speed + speedUpBy * (ScoreAndLivesManager.Shared.getScore()/1000)) ;
            
            if (curMug)
            {
                 physics.velocity = Vector2.left * (5*speed);
            }
        }
    }

    public void FreezeDrunk()
    {
        physics.velocity = Vector2.zero;
        animator.SetBool("Freeze", true);
        shouldMove = false;
    }

    public void SetColor(GameManager.DrinkColors color, Sprite sprite)
    {
        wantedColor = color;
        wantedColorSprite.sprite = sprite;
    }

    private void OnEnable()
    {
        audioSource.clip = gotIn;
        audioSource.Play();
        curMug = null;
        toSendEmpty =  Random.Range(1, 10) >= 8;
        shouldMove = true;
        transform.localScale = new Vector3(0.55f, 0.55f, 1);
    }

    public void ReleaseMug()
    {
        if (curMug)
        {
            curMug.transform.parent = null;
        }
    }
}
