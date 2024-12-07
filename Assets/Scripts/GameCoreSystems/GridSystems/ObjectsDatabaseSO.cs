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
        if (ID < 9)
        {
            if (i == 2)
            {
                BuyPrice = price * 2;
            }
            else if (i == -2)
            {
                BuyPrice = price / 2;
            }
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