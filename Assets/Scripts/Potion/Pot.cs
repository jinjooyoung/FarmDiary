using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public enum PotState
    {
        Empty,                  // 1. �ƹ��͵� ������� ���� ����
        Selecting,              // 2. ��� ���� ��
        ReadyToStart,           // 3. ���� ���� ��� ����
        Crafting,               // 4. ���� ��
        Completed               // 5. ���� �Ϸ� ����
    }

    private GameObject panel;   // ���� ���� �г�
    private bool isInitialized = false; // �ʱ�ȭ ���� �÷���
    public int id = -1; // ���� ���� ID

    [Header("���� ���� ����")]
    public PotState currentState = PotState.Empty; // ���� ����

    [Header("���õ� �۹�")]
    public int magicID = -1;    // ������ ���� �۹� ID
    public List<int> basicMaterial = new List<int>(new int[3]);     // ������ �Ϲ� �۹� ID �迭

    [Header("���� ���� �ð�")]
    public float totalCraftingTime; // ���� ���ۿ� �ɸ��� �� �ð�
    [Header("���� �ð�")]
    public float remainingTime; // ���� ���� �ð�

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

    private void Update()
    {
        if (currentState == PotState.Crafting)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime; // �ð� ����
            }
            else
            {
                remainingTime = 0; // ���� �ð��� 0���� ����
                ChangeState(PotState.Completed);
            }
        }
    }

    public void ChangeState(PotState newState)
    {
        currentState = newState;
        PotionUIManager.instance.InitializePotionUI();
        Debug.Log($"�� ���� ����: {newState}");
    }

    public void Initialize()    // �� �ʱ�ȭ
    {
        isInitialized = true; // ��ġ �Ϸ� �� ȣ��
        panel = UIManager.instance.PotionPanel;
    }

    // Ŭ�� �̺�Ʈ ó��
    private void OnMouseDown()
    {
        if (!isInitialized) return;     // ��ġ���ڸ��� �г� ������ �� ����

        if (PlacementSystem.Instance.IsDeleteModeActive()) return;

        if (PotionManager.instance != null && UIManager.instance != null)
        {
            Debug.Log($"�� Ŭ����. ID: {id}");
            PotionUIManager.instance.currentPotID = id;     // ���� UI �Ŵ����� Ŭ���� ���� ID�� ����
            UIManager.instance.TogglePanel(panel);          // �г��� ����
        }
    }
}
