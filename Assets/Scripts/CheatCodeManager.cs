using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{
    private string inputBuffer = ""; // 입력된 문자열 저장
    private string cheatCode = "clxmzhemanswkddmfanjfhgkwl"; // 비밀 코드 치트코드문장을뭐로하지

    private void Awake()
    {
        if (PlayerPrefs.GetInt("CheatedKey", 0) == 1)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown) // 어떤 키든 눌렸을 때만 실행
        {
            foreach (char c in Input.inputString)
            {
                inputBuffer += c;

                if (inputBuffer.Contains(cheatCode))
                {
                    ActivateCheat();
                    inputBuffer = "";
                }

                if (inputBuffer.Length > cheatCode.Length)
                {
                    inputBuffer = inputBuffer.Substring(inputBuffer.Length - cheatCode.Length);
                }
            }
        }
    }

    void ActivateCheat()
    {
        GameManager.AddCoins(5000000);
        this.gameObject.SetActive(false);
        PlayerPrefs.SetInt("CheatedKey", 1);
        PlayerPrefs.Save();
    }
}
