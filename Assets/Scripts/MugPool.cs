using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MugPool : MonoBehaviour
{
    #region Inspector

    [SerializeField] private Mug template = default;
    [SerializeField] private RuntimeAnimatorController greenMugAnimation;
    [SerializeField] private RuntimeAnimatorController redMugAnimation;
    [SerializeField] private RuntimeAnimatorController purpleMugAnimation;
    [SerializeField] private RuntimeAnimatorController yellowMugAnimation;
    [SerializeField] private Transform playersPos;
    #endregion


    #region Fields

    private ObjectPool<Mug> pool;
    private List<Mug> activeMugs;
    #endregion


    #region Properties
    public static MugPool Shared { get; private set; }

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        Shared = this;
        pool = new ObjectPool<Mug>(Create, OnActivate, OnDeactivate);
        activeMugs = new List<Mug>();
    }

    #endregion

    #region Methods

    public Mug Get(GameManager.DrinkColors color)
    {
        Mug mug = pool.Get();
        activeMugs.Add(mug);
        mug.animator.runtimeAnimatorController = color switch
        {
            GameManager.DrinkColors.Green => greenMugAnimation,
            GameManager.DrinkColors.Purple => purpleMugAnimation,
            GameManager.DrinkColors.Yellow => yellowMugAnimation,
            GameManager.DrinkColors.Red => redMugAnimation,
            _ => mug.animator.runtimeAnimatorController
        };
        mug.color = color;
        return mug;
    }

    public void Release(Mug mug)
    {
        if (mug)
        {
            pool.Release(mug);
            activeMugs.Remove(mug);
        }
    }

    private Mug Create()
    {
        Mug mug = Instantiate(template);
        mug.gameObject.SetActive(false);
        return mug;
    }

    private void OnActivate(Mug mug)
    {
        mug.gameObject.SetActive(true);
        Vector3 pos = playersPos.position;
        pos.x -= 1;
        pos.y += 0.4f;
        mug.transform.position = pos;
    }

    private void OnDeactivate(Mug mug)
    {
        mug.gameObject.SetActive(false);
    }

    public void FreezeAll()
    {
        foreach (Mug m in activeMugs)
        {
            m.FreezeMug();
        }
    }
    public void ReleaseAll()
    {
        foreach (Mug m in activeMugs)
        {
            pool.Release(m);
        }
        activeMugs.Clear();
    }

    #endregion

    
}