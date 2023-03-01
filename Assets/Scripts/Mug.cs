using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Mug : MonoBehaviour
{
    #region Inspector
    [SerializeField] private float throwSpeed;
    [SerializeField] private float returnSpeed;
    
    #endregion

    #region Fields
    private Rigidbody2D physics;
    private Drunk drunk;
    private SpriteRenderer spriteRenderer;
    private bool shouldMove;
    public Animator animator;
    private bool isFalling;
    private int state;
    private const int FULL_MUG = 1;
    private const int EMPTY_MUG = 2;
    public GameManager.DrinkColors color;
    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        physics = GetComponent<Rigidbody2D>();
        physics.velocity = Vector2.left * throwSpeed;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        //when the mug is returning and the bartender touches it.
        if (col.CompareTag("Player") && state == EMPTY_MUG)
        {
            ScoreAndLivesManager.Shared.AddEmptyMugScore();
            Release();
        }
        //when the empty mug return to the end and not getting caught by the player. 
        if (col.CompareTag("Finish") && state == EMPTY_MUG)
        {
            animator.SetTrigger("EmptyMugFall");
            isFalling = true;
            physics.velocity = Vector2.down * 2.5f;
            StartCoroutine(StopFalling());
        }
        // when full mug reached the end of lane
        if (col.CompareTag("Respawn"))
        {
            isFalling = true;
            animator.SetTrigger("FullMugFall"); 
            physics.velocity = Vector2.down*2.5f;
            StartCoroutine(StopFalling());
        }
    }

    void Update()
    {
        if (!transform.parent && shouldMove)
        {
            if (state == FULL_MUG)
            {
                physics.velocity = Vector2.left * throwSpeed;
            }
            else if (state == EMPTY_MUG)
            {
                physics.velocity = Vector2.right * returnSpeed;
            }
        }
    }
    
    private void OnEnable()
    {
        animator.SetTrigger("FullMug");
        if (physics) physics.velocity = Vector2.left * throwSpeed;
        tag = "Mug";
        state = FULL_MUG;
        isFalling = false;
        transform.parent = null;
        shouldMove = true;
    }
    #endregion

    #region Methods
    private IEnumerator StopFalling()
    {
        yield return new WaitForSeconds(0.6f);
        physics.velocity = Vector2.zero;
    }

    

    public void FreezeMug()
    {
        if (!isFalling)
        {
            physics.velocity = Vector2.zero;
        }
        shouldMove = false;
    }

    public void CaughtByDrunk(Drunk drunk1)
    {
        drunk = drunk1;
        physics.velocity = Vector2.zero;
        transform.SetParent( drunk.transform);
    }

    public void ReturnEmpty()
    {
        tag = "EmptyMug";
        animator.SetTrigger("EmptyMug");
        state = EMPTY_MUG;
        transform.SetParent(null);
        spriteRenderer.enabled = true;
    }

    public void Drinking()
    {
        spriteRenderer.enabled = false;
    }

    public void Release()
    {
        transform.parent = null;
        MugPool.Shared.Release(this);
    }
    
    #endregion
}