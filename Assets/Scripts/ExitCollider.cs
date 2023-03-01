using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCollider : MonoBehaviour
{

    #region Inspector
    [SerializeField] private GameManager gameManager;
    #endregion
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Mug"))
        {
            gameManager.FullMugReachedEnd();
        }
    }
}