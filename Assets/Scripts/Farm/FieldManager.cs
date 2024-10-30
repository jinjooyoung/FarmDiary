using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FieldManager : MonoBehaviour
{
    // �ʵ� ID�� �ر� ������ �����ϴ� Dictionary, ������ ������ (���� x, �� x) ���·� ����
    private Dictionary<int, (Vector3Int, Vector3Int)> fieldAreas = new Dictionary<int, (Vector3Int, Vector3Int)>();
    // �ʵ� ID�� �ر� ����� �����ϴ� Dictionary
    private Dictionary<int, int> fieldUnlockCosts = new Dictionary<int, int>();
    // �ʵ� ID�� �ر� ���¸� �����ϴ� Dictionary
    private Dictionary<int, bool> unlockedFields = new Dictionary<int, bool>();

    private List<Vector3Int> unlockedAreas; // �رݵ� ���� ���

    [SerializeField] private GameObject[] fieldObjects;     // ��ݵ� �ʵ带 �����ִ� ������Ʈ
    [SerializeField] private GameObject[] buttons;

    private void Start()
    {
        // ��� �ʵ� ��� ����
        SetUnlockCosts();

        // ó���� �߾� �ʵ�� �رݵ� ���·� ����
        unlockedFields[0] = true;
        InitializeFieldAreas();         // �ر� ���� ����
        InitializeUnlockedFields();     // �ر� �ʱ�ȭ (�׽�Ʈ�� �ڵ�, ���߿� �����ؾ���)
        LoadUnlockedFields();           // �ر� ���� �ҷ�����
    }

    // �ر� ������ �����ϴ� �޼���
    public void InitializeFieldAreas()
    {
        // �߾� ���� �ر� (������ �� ����ȭ�鿡 ���̴� ����)
        int centralFieldID = 0;
        fieldAreas[centralFieldID] = (new Vector3Int(-18, -10, 0), new Vector3Int(17, -7, 0));

        // ��� �ִ� ������ ���� (����)
        fieldAreas[-1] = (new Vector3Int(-27, -10, 0), new Vector3Int(-19, -7, 0)); // ID -1: -27���� -19���� (9ĭ)
        for (int i = 2; i <= 11; i++)
        {
            fieldAreas[-i] = (new Vector3Int(-18 - (i * 10), -10, 0), new Vector3Int(-9 - (i * 10), -7, 0));
        }

        // ��� �ִ� ������ ���� (������)
        fieldAreas[1] = (new Vector3Int(18, -10, 0), new Vector3Int(26, -7, 0)); // ID +1: 18���� 26���� (9ĭ)
        for (int i = 2; i <= 11; i++)
        {
            fieldAreas[i] = (new Vector3Int(7 + (i * 10), -10, 0), new Vector3Int(16 + (i * 10), -7, 0));
        }
    }

    /*// ID�� �Ҵ�� ������ ��� �׸��� �� ��ǥ�� ���� ��ųʸ��� ��ȯ�ϴ� �޼���
    public Dictionary<int, List<Vector3Int>> AreasList(int ID)
    {
        // ��ȯ�� ��ųʸ�
        Dictionary<int, List<Vector3Int>> result = new Dictionary<int, List<Vector3Int>>();

        // Ű�� �ִ��� Ȯ���ϰ�, �����Ѵٸ� �� ������� area�� ����
        if (fieldAreas.TryGetValue(ID, out (Vector3Int start, Vector3Int end) area))
        {
            // ��ȯ�� ��ųʸ��� ��������� ����� ����Ʈ ����
            List<Vector3Int> areaCoordinates = new List<Vector3Int>();

            // ���� ���� ��� ��ǥ�� ���
            for (int x = area.start.x; x <= area.end.x; x++)
            {
                for (int y = area.start.y; y <= area.end.y; y++)
                {
                    areaCoordinates.Add(new Vector3Int(x, y, 0));
                }
            }

            // ��� ��ųʸ��� �߰�
            result.Add(ID, areaCoordinates);
        }
        else
        {
            Debug.Log($"ID : {ID} �� �������� �ʽ��ϴ�.");
        }

        return result;
    }*/

    private void DisableFieldObject(int fieldID)
    {
        // �ʵ� ID�� �´� �ε����� ���
        int index = fieldID < 0 ? fieldID + 11 : fieldID + 10; // ������ ���� 11�� ���ϰ�, ����� ���� 10�� ���մϴ�.

        if (fieldObjects[index] != null && buttons[index] != null)
        {
            fieldObjects[index].SetActive(false); // �ش� ������Ʈ ��Ȱ��ȭ
            buttons[index].SetActive(false);
            Debug.Log($"�� {fieldID} �ر� �Ϸ�. ������Ʈ ��Ȱ��ȭ��.");
        }
    }

    // �ʵ带 �ر��ϴ� �޼���
    public void UnlockField(int fieldID)
    {
        if (!fieldAreas.ContainsKey(fieldID))
        {
            Debug.LogWarning("�ʵ� ID�� �������� �ʽ��ϴ�.");
            return;
        }

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
        if (GameManager.currentCoin >= unlockCost)
        {
            // ������ �����ϰ� �ʵ带 �ر�
            GameManager.SubtractCoins(unlockCost);
            unlockedFields[fieldID] = true;

            // �ر� ���¸� PlayerPrefs�� ����
            SaveUnlockedField(fieldID, true);

            DisableFieldObject(fieldID);

            Debug.Log($"�� {fieldID} �ر� �Ϸ�. ���� ����: {GameManager.currentCoin}");
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

    // �ʵ� �ر� ����� �����ϴ� �޼���
    public void SetUnlockCost(int fieldID, int cost)
    {
        fieldUnlockCosts[fieldID] = cost;
    }

    // ��� �ʵ忡 ����� �����Ű�� �޼���
    private void SetUnlockCosts()
    {
        int[] costs = new int[] { 200, 300, 400, 600, 800, 1100, 1400, 1800, 2300, 2800, 3500 }; // ��� �迭

        for (int i = 1; i <= 11; i++) // 1���� 3���� �ݺ�
        {
            SetUnlockCost(i, costs[i - 1]);     // ��� �ʵ� ID�� ���� ��� ����
            SetUnlockCost(-i, costs[i - 1]);    // ���� �ʵ� ID�� ���� ��� ����
        }
    }

    // �ر������� �ҷ��ͼ� �����Ű�� �޼���
    private void LoadUnlockedFields()
    {
        for (int i = -11; i <= -1; i++)
        {
            // PlayerPrefs���� �ر� ���¸� ������ (�⺻��: false)
            bool isUnlocked = PlayerPrefs.GetInt($"UnlockedFieldID_{i}", 0) == 1;
            unlockedFields[i] = isUnlocked;
        }

        for (int i = 1; i <= 11; i++)
        {
            // PlayerPrefs���� �ر� ���¸� ������ (�⺻��: false)
            bool isUnlocked = PlayerPrefs.GetInt($"UnlockedFieldID_{i}", 0) == 1;
            unlockedFields[i] = isUnlocked;
        }
    }

    // �ر��� �����ϴ� �޼���
    private void SaveUnlockedField(int fieldID, bool isUnlocked)
    {
        // Dictionary�� �ر� ���� ����
        unlockedFields[fieldID] = isUnlocked;

        // PlayerPrefs�� �ر� ���� ���� (1: �رݵ�, 0: �رݵ��� ����)
        PlayerPrefs.SetInt($"UnlockedFieldID_{fieldID}", isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    // �ر��� �ʱ�ȭ ��Ű�� �޼���
    private void InitializeUnlockedFields()
    {
        for (int i = -11; i <= -1; i++)
        {
            PlayerPrefs.SetInt($"UnlockedFieldID_{i}", 0);
        }

        for (int i = 1; i <= 11; i++)
        {
            PlayerPrefs.SetInt($"UnlockedFieldID_{i}", 0);
        }
    }
}
