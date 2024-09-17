using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera mainCam;
    
    private void Awake()
    {
        instance = this;
    }
}
