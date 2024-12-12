using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/PotionDatabaseSO", fileName = "PotionDatabaseSO")]
public class PotionDatabaseSO : ScriptableObject
{
    public List<PotionData> potionData;
}

[Serializable]
public class PotionData
{
    [field: SerializeField]
    public int ID { get; private set; }               // ���� ID

    [field: SerializeField]
    public string Name { get; private set; }          // ���� �̸�

    [field: SerializeField]
    public float craftingTime { get; private set; }   // ���� �ð�

    [field: SerializeField]
    public int price { get; private set; }            // ����

    public void InitializecraftingTime(int i)
    {
        if (i == 0)
        {
            craftingTime = 15;
        }
        else
        {
            craftingTime = 30;
        }
    }

    public void InitializecraftingTimeCheat(int i)
    {
        if (i == 0)
        {
            craftingTime = 3;
        }
        else
        {
            craftingTime = 30;
        }
    }
}

public static class PotionDatabase
{
    private static PotionDatabaseSO database;

    // �ʱ�ȭ
    public static void Initialize(PotionDatabaseSO db)
    {
        database = db;
    }

    // ID�� ��������
    public static PotionData GetPotionID(int id)
    {
        return database.potionData.Find(potion => potion.ID == id);
    }

    // ���� �̸�
    public static string GetName(int id)
    {
        PotionData potion = GetPotionID(id);
        return potion.Name;
    }

    // ���� ���� �ð�
    public static float GetCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        return potion.craftingTime;
    }

    // ���� �Ǹ� ����
    public static int GetPotionPrice(int id)
    {
        PotionData potion = GetPotionID(id);
        return potion.price;
    }

    // ���� ���� �ð� ª��
    public static void TutorialCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        potion.InitializecraftingTime(0);
    }

    // ���� ���� �ð� ª��
    public static void CheatCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        potion.InitializecraftingTimeCheat(0);
    }

    // ���� ���� �ð� �ٽ� ���
    public static void TutorialEndCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        potion.InitializecraftingTime(1);
    }
}
