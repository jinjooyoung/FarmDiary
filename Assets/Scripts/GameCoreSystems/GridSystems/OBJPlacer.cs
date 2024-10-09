using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPlacer : MonoBehaviour
{
    [SerializeField]
    private PreviewSystem preview;

    [SerializeField]
    private List<GameObject> placedGameObjects = new(); // 설치된 오브젝트들

    // 실제 설치 구현
    public int PlaceObject(GameObject prefab, Vector3 position)     // 설치할 오브젝트 프리팹과 설치할 그리드 위치를 받아옴
    {
        GameObject newObject = Instantiate(prefab);     // 오브젝트 생성
        newObject.transform.position = position;        // 설치할 위치로 이동
        placedGameObjects.Add(newObject);               // 설치된 오브젝트 리스트에 저장

        SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        preview.SetAlpha(newObjectRenderer, 1.0f);      // 오브젝트의 투명도를 다시 1로 

        return placedGameObjects.Count - 1;             // 설치하여 증가한 placedGameObjects 리스트의 마지막 오브젝트의 인덱스를 리턴
    }

    // 실제 삭제 구현
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
