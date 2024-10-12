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
    public Vector2Int Size { get; private set; } = Vector2Int.one;      // 오브젝트 크기 (설치 가능/ 불가능 판별 용)

    [field: SerializeField]
    public GameObject Prefab { get; private set; }      // 리소스 프리팹
}