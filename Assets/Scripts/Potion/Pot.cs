using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    private GameObject panel;
    public int id = -1; // 고유 ID

    // ID를 설정하는 메서드
    public void SetID(int newID)
    {
        id = newID;
    }

    // ID를 해제하는 메서드
    public void ClearID()
    {
        id = -1;
    }

    public int GetID()
    {
        return id;
    }

    private void Awake()
    {
        panel = UIManager.instance.PotionPanel;
    }

    // 클릭 이벤트 처리
    private void OnMouseDown()
    {
        if (PotionManager.instance != null && UIManager.instance != null)
        {
            Debug.Log($"솥 클릭됨. ID: {id}");
            UIManager.instance.TogglePanel(panel);
            // UI 초기화
        }
    }
}
