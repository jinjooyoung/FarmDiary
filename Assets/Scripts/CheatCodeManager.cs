using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{
    private string inputBuffer = ""; // �Էµ� ���ڿ� ����
    private string cheatCode = "clxmzhemanswkddmfanjfhgkwl"; // ��� �ڵ� ġƮ�ڵ幮������������

    private void Awake()
    {
        if (PlayerPrefs.GetInt("CheatedKey", 0) == 1)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown) // � Ű�� ������ ���� ����
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
