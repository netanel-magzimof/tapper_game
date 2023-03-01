using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCollider : MonoBehaviour
{

    #region Inspector
    [SerializeField] private GameManager gameManager;
    #endregion

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Drunk"))
        {
            gameManager.DrunkReachedEnd(col.gameObject);
        }
        else if (col.CompareTag("EmptyMug"))
        {
            gameManager.EmptyMugReachedEnd();
        }
    }
}
