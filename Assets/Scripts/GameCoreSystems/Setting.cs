using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Image pauseImage;
    private bool isPause = false;   // 게임이 일시 정지 상태인지 확인하는 변수

    void Update()
    {
        // isPause == false인거 체크하고 스페이스바를 누르면
        if (Input.GetKeyDown(KeyCode.Space) && isPause == false)
        {
            // 일시정지 이미지가 나오면서 게임이 멈추고 일시정지 상태가 된다.
            pauseImage.gameObject.SetActive(true);
            Time.timeScale = 0;
            isPause = true;
        }
        else if (isPause)   // 일시정지 상태일때
        {
            // 한번더 스페이스바를 누르면
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 일시정지 상태가 풀린다.
                pauseImage.gameObject.SetActive(false);
                Time.timeScale = 1;
                isPause = false;
            }
        }
    }
}
