using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/ObjectsDatabaseSO", fileName = "ObjectsDatabase")]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;    // ��ġ�� ������Ʈ
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }        // ������Ʈ �̸�

    [field: SerializeField]
    public int ID { get; private set; }         // ������Ʈ �ĺ���

    [field: SerializeField]
    public int BuyPrice { get; private set; }      // ���� ����

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;      // ������Ʈ ũ�� (��ġ ����/ �Ұ��� �Ǻ� ��)

    [field: SerializeField]
    public GameObject Prefab { get; private set; }      // ���ҽ� ������

    // �۹� ���� �Ӽ� (�۹��� �ƴ� ��� ����ΰų� �⺻������)
    [field: SerializeField]
    public bool IsCrop { get; private set; } = false; // �۹� ����

    [field: SerializeField]
    public float[] GrowthTimes { get; private set; } = new float[4]; // �۹��� �ܰ躰 ���� �ð�


    public void DecreaseGrowthTimes()
    {
        GrowthTimes[0] = 2;
        GrowthTimes[1] = 4;
        GrowthTimes[2] = 6;
        GrowthTimes[3] = 8;
    }

    public void InitializeGrowthTimes(int i)
    {
        GrowthTimes[0] = i * 8;
        GrowthTimes[1] = i * 8 * 2;
        GrowthTimes[2] = i * 8 * 3;
        GrowthTimes[3] = i * 8 * 4;
    }

    public void Twice(int price, int i)
    {
        // ID�� �°� ������ ������ ��, ������ ������Ű�� ���� �߰�
        // ����: ID���� ���� ������ ���� ������ �ٸ��� ����
        switch (ID)
        {
            case 0: // 1x1 �� (ID == 0)
                    // ��ġ�� �� ���� ���� ���� (e.g., 1.13��)
                if (i == 2)
                {
                    BuyPrice = (int)(price * 1.13f);
                }
                // ������ �� ���� ���� ���� (e.g., 1/1.13��)
                else if (i == -2)
                {
                    BuyPrice = (int)(price / 1.13f);
                }
                break;

            case 1: // 2x2 �� (ID == 1)
                    // ��ġ�� �� ���� ���� ���� (e.g., 1.15��)
                if (i == 2)
                {
                    BuyPrice = (int)(price * 1.15f);
                }
                // ������ �� ���� ���� ���� (e.g., 1/1.15��)
                else if (i == -2)
                {
                    BuyPrice = (int)(price / 1.15f);
                }
                break;

            case 2: // 3x3 �� (ID == 2)
                    // ��ġ�� �� ���� ���� ���� (e.g., 1.18��)
                if (i == 2)
                {
                    BuyPrice = (int)(price * 1.18f);
                }
                // ������ �� ���� ���� ���� (e.g., 1/1.18��)
                else if (i == -2)
                {
                    BuyPrice = (int)(price / 1.18f);
                }
                break;

            case 3: // 4x4 �� (ID == 3)
                    // ��ġ�� �� ���� ���� ���� (e.g., 1.2��)
                if (i == 2)
                {
                    BuyPrice = (int)(price * 1.2f);
                }
                // ������ �� ���� ���� ���� (e.g., 1/1.2��)
                else if (i == -2)
                {
                    BuyPrice = (int)(price / 1.2f);
                }
                break;
            case 4: // �� (ID == 4)
                    // ��ġ�� �� ���� ���� ���� (e.g., 1.63��)
                if (i == 2)
                {
                    BuyPrice = (int)(price * 1.63f);
                }
                // ������ �� ���� ���� ���� (e.g., 1/1.2��)
                else if (i == -2)
                {
                    BuyPrice = (int)(price / 1.63f);
                }
                break;

            default:
                // �߰����� �� ũ�⿡ ���� ������ �ʿ��ϸ� ���⼭ ó��
                break;
        }

        // �ִ��� 2������ ����
        if (BuyPrice > 200000000)
        {
            BuyPrice = 200000000;
        }
    }

    public void SetBuyPrice(int amount)
    {
        BuyPrice = amount;
    }

    public void ResetPrice(int i)
    {
        if (i == 0)
        {
            BuyPrice = 200;
        }
        else if (i == 1)
        {
            BuyPrice = 600;
        }
        else if (i == 2)
        {
            BuyPrice = 1000;
        }
        else if (i == 3)
        {
            BuyPrice = 2000;
        }
        else if (i == 4)
        {
            BuyPrice = 50000;
        }
    }
}

public static class ObjectsDatabase
{
    private static ObjectsDatabaseSO database;

    public static void Initialize(ObjectsDatabaseSO db)
    {
        database = db;
    }

    public static ObjectData GetObjectByID(int id)
    {
        return database.objectsData.Find(obj => obj.ID == id);
    }

    public static void ResetPrice()
    {
        for (int i = 0; i <= 4; i++)
        {
            ObjectData objectData = GetObjectByID(i);      // �ش� ID�� ������Ʈ �����͸� ����
            objectData.ResetPrice(i);
        }
    }

    // ������ ������Ű�� �޼���
    public static void PriceIncrease(int id)
    {
        Debug.LogWarning("���� ���� ȣ���");
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        int currentPrice = objectData.BuyPrice;         // ���� ������ �޾ƿ�
        objectData.Twice(currentPrice, 2);
    }

    // ������ ���ҽ�Ű�� �޼���
    public static void PriceDecrease(int id)
    {
        Debug.LogWarning("���� ���� ȣ���");
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        int currentPrice = objectData.BuyPrice;         // ���� ������ �޾ƿ�
        objectData.Twice(currentPrice, -2);
    }

    // ������ �����ϴ� �޼���
    public static int CurrentPrice(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        return objectData.BuyPrice;
    }

    // ���� �ð� ������ �����ϴ� �޼���
    public static float[] GetCropGrowthTimes(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        return objectData.GrowthTimes;
    }

    // ������ ���ӿ�����Ʈ�� �����ϴ� �޼���
    public static GameObject GetPrefabByID(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        if (objectData == null)
        {
            Debug.LogError($"{id}�� �ش��ϴ� ������ ã�� ����");
            return null;
        }
        else
        {
            return objectData.Prefab;
        }
    }

    // Ʃ�丮�� �����Ҷ� �۹� ����ð��� ���̴� �޼���
    public static void SetTutorialGrowthTimes(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        objectData.DecreaseGrowthTimes();
    }

    // Ʃ�丮�󳡳� �� ���� ���� �ٽ� �����ϴ� �޼���
    public static void InitializeTutorialGrowthTimes(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // �ش� ID�� ������Ʈ �����͸� ����
        
        if (id == 48)
        {
            objectData.InitializeGrowthTimes(168);
        }
        else
        {
            objectData.InitializeGrowthTimes(id - 8);
        }
    }
}