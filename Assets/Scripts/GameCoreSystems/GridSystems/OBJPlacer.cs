using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPlacer : MonoBehaviour
{
    [SerializeField]
    private PreviewSystem preview;

    [SerializeField]
    public List<GameObject> placedGameObjects = new(); // ��ġ�� ������Ʈ��

    // ���� ��ġ ����
    public int PlaceObject(GameObject prefab, Vector3 position)     // ��ġ�� ������Ʈ �����հ� ��ġ�� �׸��� ��ġ�� �޾ƿ�
    {
        GameObject newObject = Instantiate(prefab);     // ������Ʈ ����
        newObject.transform.position = position;        // ��ġ�� ��ġ�� �̵�

        for (int i = 0; i < placedGameObjects.Count; i++)
        {
            if (placedGameObjects[i] == null)
            {
                placedGameObjects[i] = newObject;
                SpriteRenderer objectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
                preview.SetAlpha(objectRenderer, 1.0f);  // ������Ʈ�� ������ �ٽ� 1�� ����
                return i;  // null �ڸ��� �ε����� ����
            }
        }

        placedGameObjects.Add(newObject);               // ��ġ�� ������Ʈ ����Ʈ�� ����

        SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        preview.SetAlpha(newObjectRenderer, 1.0f);      // ������Ʈ�� ������ �ٽ� 1�� 

        return placedGameObjects.Count - 1;             // ��ġ�Ͽ� ������ placedGameObjects ����Ʈ�� ������ ������Ʈ�� �ε����� ����
    }

    // ���� ���� ����
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }

        string objName = placedGameObjects[gameObjectIndex].name;   // ������ ������Ʈ�� �̸�

        /*// ������Ʈ�� �̸��� ���� �� ������Ʈ��� �ش� �� ������ ���ҽ�Ŵ
        switch (objName)
        {
            case "1x1_Field(Clone)":
                ObjectsDatabase.PriceDecrease(0);
                break;
            case "2x2_Field(Clone)":
                ObjectsDatabase.PriceDecrease(1);
                break;
            case "3x3_Field(Clone)":
                ObjectsDatabase.PriceDecrease(2);
                break;
            case "4x4_Field(Clone)":
                ObjectsDatabase.PriceDecrease(3);
                break;
        }*/

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
