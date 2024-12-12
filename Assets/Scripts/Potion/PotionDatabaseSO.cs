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
    public int ID { get; private set; }               // 고유 ID

    [field: SerializeField]
    public string Name { get; private set; }          // 포션 이름

    [field: SerializeField]
    public float craftingTime { get; private set; }   // 제작 시간

    [field: SerializeField]
    public int price { get; private set; }            // 가격

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

    // 초기화
    public static void Initialize(PotionDatabaseSO db)
    {
        database = db;
    }

    // ID로 가져오기
    public static PotionData GetPotionID(int id)
    {
        return database.potionData.Find(potion => potion.ID == id);
    }

    // 포션 이름
    public static string GetName(int id)
    {
        PotionData potion = GetPotionID(id);
        return potion.Name;
    }

    // 포션 제작 시간
    public static float GetCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        return potion.craftingTime;
    }

    // 포션 판매 가격
    public static int GetPotionPrice(int id)
    {
        PotionData potion = GetPotionID(id);
        return potion.price;
    }

    // 포션 제작 시간 짧게
    public static void TutorialCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        potion.InitializecraftingTime(0);
    }

    // 포션 제작 시간 짧게
    public static void CheatCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        potion.InitializecraftingTimeCheat(0);
    }

    // 포션 제작 시간 다시 길게
    public static void TutorialEndCraftingTime(int id)
    {
        PotionData potion = GetPotionID(id);
        potion.InitializecraftingTime(1);
    }
}
