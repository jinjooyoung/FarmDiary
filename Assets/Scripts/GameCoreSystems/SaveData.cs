using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData instance;
    
    [Header("Save Data")]
    public bool verticalMode;
    
    private void Awake()
    {
        instance = this;
    }
}
