using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmField : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.Addfield(this);
        }
    }
}
