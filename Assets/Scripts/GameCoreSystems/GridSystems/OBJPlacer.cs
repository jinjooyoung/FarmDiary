using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering;
using UnityEngine;

public class OBJPlacer : MonoBehaviour
{
    [SerializeField]
    private PreviewSystem preview;

    [SerializeField]
    public List<GameObject> placedGameObjects = new(); // 설치된 오브젝트들

    public int potCount = 0;
    public int placedObjectIndex = 0;  // 튜토리얼에서만 쓰임

    // 실제 설치 구현
    public int PlaceObject(GameObject prefab, Vector3 position)     // 설치할 오브젝트 프리팹과 설치할 그리드 위치를 받아옴
    {
        GameObject newObject = Instantiate(prefab);     // 오브젝트 생성
        newObject.transform.position = position;        // 설치할 위치로 이동

        Pot potComponent = newObject.GetComponent<Pot>();
        if (potComponent != null)
        {
            PotionManager.instance.AddPot(newObject);
        }

        for (int i = 0; i < placedGameObjects.Count; i++)
        {
            if (placedGameObjects[i] == null)
            {
                placedGameObjects[i] = newObject;
                SpriteRenderer objectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
                preview.SetAlpha(objectRenderer, 1.0f);  // 오브젝트의 투명도를 다시 1로 설정
                placedObjectIndex = i;
                return i;  // null 자리의 인덱스를 리턴
            }
        }

        placedGameObjects.Add(newObject);               // 설치된 오브젝트 리스트에 저장

        SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        preview.SetAlpha(newObjectRenderer, 1.0f);      // 오브젝트의 투명도를 다시 1로 

        placedObjectIndex = placedGameObjects.Count - 1;
        return placedGameObjects.Count - 1;             // 설치하여 증가한 placedGameObjects 리스트의 마지막 오브젝트의 인덱스를 리턴
    }

    // 실제 삭제 구현
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }

        string objName = placedGameObjects[gameObjectIndex].name;   // 삭제될 오브젝트의 이름

        if (objName == "4(Clone)")    // 삭제할 오브젝트가 밭이라면
        {
            potCount--;
            PotionManager.instance.RemovePot(placedGameObjects[gameObjectIndex]);
            ObjectsDatabase.PriceDecrease(4);
            UIManager.instance.Pot.text = ObjectsDatabase.CurrentPrice(4).ToString("N0");
        }

        // 오브젝트의 이름을 보고 밭 오브젝트라면 해당 밭 가격을 감소시킴
        switch (objName)
        {
            case "0(Clone)":
                ObjectsDatabase.PriceDecrease(0);
                UIManager.instance.one.text = ObjectsDatabase.CurrentPrice(0).ToString("N0");
                break;
            case "1(Clone)":
                ObjectsDatabase.PriceDecrease(1);
                UIManager.instance.two.text = ObjectsDatabase.CurrentPrice(1).ToString("N0");
                break;
            case "2(Clone)":
                ObjectsDatabase.PriceDecrease(2);
                UIManager.instance.three.text = ObjectsDatabase.CurrentPrice(2).ToString("N0");
                break;
            case "3(Clone)":
                ObjectsDatabase.PriceDecrease(3);
                UIManager.instance.four.text = ObjectsDatabase.CurrentPrice(3).ToString("N0");
                break;
        }

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
