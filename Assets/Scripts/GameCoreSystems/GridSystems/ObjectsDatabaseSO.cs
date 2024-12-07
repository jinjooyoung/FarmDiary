using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/ObjectsDatabaseSO", fileName = "ObjectsDatabase")]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;    // 설치할 오브젝트
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }        // 오브젝트 이름

    [field: SerializeField]
    public int ID { get; private set; }         // 오브젝트 식별자

    [field: SerializeField]
    public int BuyPrice { get; private set; }      // 구매 가격

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;      // 오브젝트 크기 (설치 가능/ 불가능 판별 용)

    [field: SerializeField]
    public GameObject Prefab { get; private set; }      // 리소스 프리팹

    // 작물 전용 속성 (작물이 아닌 경우 비워두거나 기본값으로)
    [field: SerializeField]
    public bool IsCrop { get; private set; } = false; // 작물 여부

    [field: SerializeField]
    public float[] GrowthTimes { get; private set; } = new float[4]; // 작물의 단계별 성장 시간


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

    // 가격을 증가시키는 메서드
    public static void PriceIncrease(int id)
    {
        Debug.LogWarning("가격 증가 호출됨");
        ObjectData objectData = GetObjectByID(id);      // 해당 ID의 오브젝트 데이터를 얻어옴
        int currentPrice = objectData.BuyPrice;         // 현재 가격을 받아옴
        objectData.Twice(currentPrice, 2);
    }

    // 가격을 감소시키는 메서드
    public static void PriceDecrease(int id)
    {
        Debug.LogWarning("가격 감소 호출됨");
        ObjectData objectData = GetObjectByID(id);      // 해당 ID의 오브젝트 데이터를 얻어옴
        int currentPrice = objectData.BuyPrice;         // 현재 가격을 받아옴
        objectData.Twice(currentPrice, -2);
    }

    // 가격을 리턴하는 메서드
    public static int CurrentPrice(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // 해당 ID의 오브젝트 데이터를 얻어옴
        return objectData.BuyPrice;
    }

    // 튜토리얼 진행할때 작물 성장시간을 줄이는 메서드
    public static void SetTutorialGrowthTimes(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // 해당 ID의 오브젝트 데이터를 얻어옴
        objectData.DecreaseGrowthTimes();
    }

    // 튜토리얼끝난 뒤 원래 값을 다시 설정하는 메서드
    public static void InitializeTutorialGrowthTimes(int id)
    {
        ObjectData objectData = GetObjectByID(id);      // 해당 ID의 오브젝트 데이터를 얻어옴
        
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