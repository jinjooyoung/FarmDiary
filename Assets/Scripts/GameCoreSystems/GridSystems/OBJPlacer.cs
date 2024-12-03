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
    public List<GameObject> placedGameObjects = new(); // ��ġ�� ������Ʈ��

    public int potCount = 0;

    // ���� ��ġ ����
    public int PlaceObject(GameObject prefab, Vector3 position)     // ��ġ�� ������Ʈ �����հ� ��ġ�� �׸��� ��ġ�� �޾ƿ�
    {
        GameObject newObject = Instantiate(prefab);     // ������Ʈ ����
        newObject.transform.position = position;        // ��ġ�� ��ġ�� �̵�

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

        if (objName == "Pot(Clone)")    // ������ ������Ʈ�� ���̶��
        {
            potCount--;
            PotionManager.instance.RemovePot(placedGameObjects[gameObjectIndex]);
            ObjectsDatabase.PriceDecrease(4);
            UIManager.instance.Pot.text = ObjectsDatabase.CurrentPrice(4).ToString();
        }

        /*// ������Ʈ�� �̸��� ���� �� ������Ʈ��� �ش� �� ������ ���ҽ�Ŵ
        switch (objName)
        {
            case "1x1_Field(Clone)":
                ObjectsDatabase.PriceDecrease(0);
                UIManager.instance.one.text = ObjectsDatabase.CurrentPrice(0).ToString();
                break;
            case "2x2_Field(Clone)":
                ObjectsDatabase.PriceDecrease(1);
                UIManager.instance.two.text = ObjectsDatabase.CurrentPrice(1).ToString();
                break;
            case "3x3_Field(Clone)":
                ObjectsDatabase.PriceDecrease(2);
                UIManager.instance.three.text = ObjectsDatabase.CurrentPrice(2).ToString();
                break;
            case "4x4_Field(Clone)":
                ObjectsDatabase.PriceDecrease(3);
                UIManager.instance.four.text = ObjectsDatabase.CurrentPrice(3).ToString();
                break;
        }*/

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
