using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarTender : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private SpriteRenderer[] handles;
    [SerializeField] private AudioClip throwMug, shift, fillMug;
    #endregion

    #region Fields

    private AudioSource audioSource;
    private float fillTime, shiftTime;
    private readonly Vector3[] bartenderPos = new Vector3[]
    {
        new Vector3(6.49f, -3.49f, 0),
        new Vector3(5.84f, -1.23f, 0),
        new Vector3(4.89f, 0.88f, 0),
        new Vector3(4.11f, 2.48f, 0)
    };
    private int curLine, filledLine;
    private bool canMove;
    private float curFrameInFilling;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        curLine = 1;
        transform.position = bartenderPos[curLine];
        curFrameInFilling = 0;
        GetAnimationClipTimes();
        canMove = false;
        filledLine = 5;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("canMove", canMove);
        if (canMove)
        {
            HandleMugFilling();
            HandleVerticalMovement();
            //HandleHorizontalMovement();
            animator.SetInteger("CurLine", curLine);
        }
    }

    private void HandleMugFilling()
    {
        if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                audioSource.clip = fillMug;
                audioSource.Play();
            }
            if (filledLine != curLine && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightArrow)))
            {
                curFrameInFilling = 0;
                filledLine = curLine;
                animator.SetInteger("FilledLine", filledLine);
            }
            curFrameInFilling += 2 * Time.deltaTime;
            animator.SetBool("FillMug", true);
            ReturnTapHandle(curLine, false);
            animator.SetFloat("FillTime", Mathf.Min(curFrameInFilling/fillTime,0.99f));
            return;
        }
        animator.SetBool("FillMug", false);
        if ((Input.GetKeyDown(KeyCode.Z)||Input.GetKeyDown(KeyCode.LeftArrow))&& curFrameInFilling >= fillTime)
        {
            MugPool.Shared.Get((GameManager.DrinkColors)filledLine);
            audioSource.clip = throwMug;
            audioSource.Play();
            animator.SetTrigger("ThrowMug");
            ReturnTapHandle(curLine, true);
            StartCoroutine(ResetCurTimeInFilling());
        }
        animator.SetFloat("FillTime", Mathf.Min(curFrameInFilling/fillTime,0.99f));
    }

    private IEnumerator ResetCurTimeInFilling()
    {
        yield return new WaitForSeconds(0.13f);
        curFrameInFilling = 0;
    }

    private void HandleHorizontalMovement()
    {
        Vector3 pos = transform.position;
        float posX = pos.x;
        bool isLeftOrRightPressed = false;
        bool isLeftPressed = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            posX -= speed * Time.deltaTime;
            isLeftOrRightPressed = !isLeftOrRightPressed;
            isLeftPressed = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            posX += speed * Time.deltaTime;
            isLeftOrRightPressed = !isLeftOrRightPressed;
        }

        if (isLeftOrRightPressed)
        {
            curFrameInFilling = 0;
                ReturnTapHandle(curLine, true);
        }
        if (Input.GetKey(KeyCode.X))
        {
            posX = bartenderPos[curLine].x;
        }
        int xScale = isLeftPressed && posX < pos.x? -1 : 1;
        animator.SetBool("Run" , isLeftOrRightPressed);
        if (xScale == 1) StartCoroutine(FlipPlayer(xScale));
        else transform.localScale = new Vector3(xScale, 1, 1);
        
        pos.x = Mathf.Clamp(posX, -8, bartenderPos[curLine].x);
        transform.position = pos;
        
    }

    private void HandleVerticalMovement()
    {
        bool  isMoving = false;
        int lineAfterChange = curLine;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            lineAfterChange = (lineAfterChange + 1) % 4;
            isMoving = !isMoving;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            lineAfterChange = (lineAfterChange - 1) % 4;
            if (lineAfterChange == -1)
            {
                lineAfterChange = 3;
            }
            isMoving = !isMoving;
        }

        for (int i = 0; i < handles.Length; i++)
        {
            if (i != lineAfterChange)
            {
                handles[i].enabled = true;
            }
        }
        if (isMoving)
        {
            ReturnTapHandle(curLine, true);
            animator.SetTrigger("Shift");
            MoveLine(lineAfterChange);
        }
    }

    private void MoveLine(int lineAfterChange)
    {
        curLine = lineAfterChange;
        transform.position = bartenderPos[curLine];
    }
    private IEnumerator FlipPlayer(int xScale)
    {
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(xScale, 1, 1);
    }
    
    private void GetAnimationClipTimes()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "Player_Fill_Active_Yellow":
                    fillTime = clip.length;
                    break;
                case "Player_Shift":
                    shiftTime = clip.length;
                    break;
            }
        }
    }

    public void DrunkReachedEnd(GameObject drunk)
    {
        animator.SetTrigger("Shocked");
        StartCoroutine(MoveToDrunk(drunk));
        canMove = false;
    }

    private IEnumerator MoveToDrunk(GameObject drunk)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 drunkPos = drunk.transform.position;
        drunkPos.y -= 0.5f;
        transform.position = drunkPos;
        transform.SetParent(drunk.transform);
        animator.SetTrigger("Dragged");
    }

    private void ReturnTapHandle(int curLane, bool toEnable)
    {
        handles[curLane].enabled = toEnable;
    }
    
    public void MugFell()
    {
        animator.SetTrigger("Shocked");
        canMove = false;
    }

    public void StartPlaying()
    {
        canMove = true;
        curLine = 1;
        transform.position = bartenderPos[curLine];
    }
}