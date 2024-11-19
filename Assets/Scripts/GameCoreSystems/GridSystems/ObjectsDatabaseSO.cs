using System;
using System.Collections;
using System.Collections.Generic;
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