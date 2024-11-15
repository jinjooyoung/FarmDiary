using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsDebugTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"해금된 작물 인덱스 : {PlayerPrefs.GetInt("UnlockPlant")}");
    }
}
