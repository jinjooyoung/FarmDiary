using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPlacer : MonoBehaviour
{
    [SerializeField]
    private PreviewSystem preview;

    [SerializeField]
    private List<GameObject> placedGameObjects = new(); // ��ġ�� ������Ʈ��

    // ���� ��ġ ����
    public int PlaceObject(GameObject prefab, Vector3 position)     // ��ġ�� ������Ʈ �����հ� ��ġ�� �׸��� ��ġ�� �޾ƿ�
    {
        GameObject newObject = Instantiate(prefab);     // ������Ʈ ����
        newObject.transform.position = position;        // ��ġ�� ��ġ�� �̵�
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
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
