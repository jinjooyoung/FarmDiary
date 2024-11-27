using System;
using System.Collections;
using System.Collections.Generic;
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
}