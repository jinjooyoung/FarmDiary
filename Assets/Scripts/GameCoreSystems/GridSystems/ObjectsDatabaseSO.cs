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
    public Vector2Int Size { get; private set; } = Vector2Int.one;      // ������Ʈ ũ�� (��ġ ����/ �Ұ��� �Ǻ� ��)

    [field: SerializeField]
    public GameObject Prefab { get; private set; }      // ���ҽ� ������
}