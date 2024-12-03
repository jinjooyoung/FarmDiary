using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    private GameObject panel;
    public int id = -1; // ���� ID

    // ID�� �����ϴ� �޼���
    public void SetID(int newID)
    {
        id = newID;
    }

    // ID�� �����ϴ� �޼���
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

    // Ŭ�� �̺�Ʈ ó��
    private void OnMouseDown()
    {
        if (PotionManager.instance != null && UIManager.instance != null)
        {
            Debug.Log($"�� Ŭ����. ID: {id}");
            UIManager.instance.TogglePanel(panel);
            // UI �ʱ�ȭ
        }
    }
}
