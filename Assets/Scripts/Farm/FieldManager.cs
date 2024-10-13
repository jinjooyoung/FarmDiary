using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    // �ʵ� ID�� �ر� ������ �����ϴ� Dictionary, ������ ������ (���� x, �� x) ���·� ����
    private Dictionary<int, (Vector3Int, Vector3Int)> fieldAreas = new Dictionary<int, (Vector3Int, Vector3Int)>();
    // �ʵ� ID�� �ر� ����� �����ϴ� Dictionary
    private Dictionary<int, int> fieldUnlockCosts = new Dictionary<int, int>();
    // �ʵ� ID�� �ر� ���¸� �����ϴ� Dictionary
    private Dictionary<int, bool> unlockedFields = new Dictionary<int, bool>();

    private void Start()
    {
        fieldUnlockCosts[1] = 200;
        fieldUnlockCosts[-1] = 200;
        fieldUnlockCosts[2] = 300;
        fieldUnlockCosts[-2] = 300;
        fieldUnlockCosts[3] = 500;
        fieldUnlockCosts[-3] = 500;

        // ó���� �߾� �ʵ�� �رݵ� ���·� ����
        unlockedFields[0] = true;
    }

    // �ر� ������ �����ϴ� �޼���
    public void InitializeFieldAreas()
    {
        // �߾� ���� �ر� (������ �� ����ȭ�鿡 ���̴� ����)
        int centralFieldID = 0;
        fieldAreas[centralFieldID] = (new Vector3Int(-18, -10, 0), new Vector3Int(17, -7, 0));

        // ��� �ִ� ������ ���� (����)
        for (int i = 1; i <= 3; i++)
        {
            fieldAreas[-i] = (new Vector3Int(-18 - (i * 10), -10, 0), new Vector3Int(-9 - (i * 10), -7, 0));
        }

        // ��� �ִ� ������ ���� (������)
        for (int i = 1; i <= 3; i++)
        {
            fieldAreas[i] = (new Vector3Int(9 + (i * 10), -10, 0), new Vector3Int(18 + (i * 10), -7, 0));
        }
    }

    // Ư�� �ʵ带 �ر��ϴ� �޼���
    public void StartUnlockField(int fieldID)
    {
        if (fieldAreas.ContainsKey(fieldID))
        {
            UnlockField(fieldID);
        }
        else
        {
            Debug.LogWarning("�ʵ� ID�� �������� �ʽ��ϴ�.");
        }
    }

    // �ʵ带 �ر��ϴ� �޼���
    public void UnlockField(int fieldID)
    {
        // �ʵ尡 �̹� �رݵǾ����� Ȯ��
        if (unlockedFields.ContainsKey(fieldID) && unlockedFields[fieldID])
        {
            Debug.Log("�̹� �رݵ� ���Դϴ�.");
            return;
        }

        // �ر� ����� �����Ǿ� �ִ��� Ȯ��
        if (!fieldUnlockCosts.ContainsKey(fieldID))
        {
            Debug.Log("�ش� �ʵ��� �ر� ����� �������� �ʾҽ��ϴ�.");
            return;
        }

        int unlockCost = fieldUnlockCosts[fieldID];  // �ʵ� ID�� �ش��ϴ� �ر� ��� ��������

        // ������ ������� Ȯ��
        if (GameManager.coin >= (ulong)unlockCost)
        {
            // ������ �����ϰ� �ʵ带 �ر�
            GameManager.SubtractCoins(unlockCost);
            unlockedFields[fieldID] = true;
            Debug.Log($"�� {fieldID} �ر� �Ϸ�. ���� ����: {GameManager.coin}");
            // �ش� ������ ��ȣ�ۿ� Ȱ��ȭ
        }
        else
        {
            Debug.Log("������ �����մϴ�.");
        }
    }

    // �ʵ尡 �رݵǾ����� ���θ� ��ȯ�ϴ� �޼���
    public bool IsFieldUnlocked(int fieldID)
    {
        return unlockedFields.ContainsKey(fieldID) && unlockedFields[fieldID];
    }

    // �ʵ� �ر� ����� �����ϴ� �޼��� (�ʿ�� �ܺο��� ���� ����)
    public void SetUnlockCost(int fieldID, int cost)
    {
        fieldUnlockCosts[fieldID] = cost;
    }
}
