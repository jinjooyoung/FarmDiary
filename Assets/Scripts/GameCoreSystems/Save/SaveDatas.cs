using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDatas
{
    //=========================����ȭ Ŭ����=========================

    [System.Serializable]
    public class Wrapper<T>
    {
        public List<T> list;
    }

    [System.Serializable]
    public class GameObjectData
    {
        public string name;       // ���� ������Ʈ �̸�
        public Vector3 position;  // ��ġ
        public Vector3 rotation;  // ȸ��
        public Vector3 scale;     // ũ��
    }

    [System.Serializable]
    public class PrefabData
    {
        public int id; // �ν����Ϳ��� ������ ID
        public GameObject prefab; // ���� ������
    }

    [System.Serializable]
    public class PotData
    {
        public int id;                      // ���� ���� ID
        public string currentState;         // ���� ���� ���� (enum�� ���ڿ��� ����)
        public int magicID;                 // ���õ� ���� �۹� ID
        public List<int> basicMaterial;     // ���õ� �Ϲ� �۹� ID �迭
        public float totalCraftingTime;     // �� ���� �ð�
        public float remainingTime;         // ���� ���� �ð�
        public Vector3 position;            // ���� ��ġ
        public Vector3 rotation;            // ���� ȸ��
    }

    //=========================���̺� ����=========================

    // GameObject ����Ʈ�� �����ϴ� ����
    public static void SaveGameObjects(string fileName, List<GameObject> gameObjects)
    {
        List<GameObjectData> dataList = new List<GameObjectData>();

        // GameObject �����͸� GameObjectData�� ��ȯ
        foreach (var obj in gameObjects)
        {
            GameObjectData data = new GameObjectData
            {
                name = obj.name,
                position = obj.transform.position,
                rotation = obj.transform.eulerAngles,
                scale = obj.transform.localScale
            };
            dataList.Add(data);
        }

        // JSON���� ��ȯ
        string json = JsonUtility.ToJson(new Wrapper<GameObjectData> { list = dataList }, true);

        // SaveSystem�� ����Ͽ� ����
        SaveSystem.Save(json, fileName);

        Debug.Log($"GameObjects saved to {fileName}");
    }

    //=========================�ε� ����=========================

    // ����� GameObject �����͸� �ε��ϴ� ����
    public static List<GameObjectData> LoadGameObjects(string fileName)
    {
        string json = SaveSystem.Load(fileName);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("No data to load!");
            return null;
        }

        Wrapper<GameObjectData> wrapper = JsonUtility.FromJson<Wrapper<GameObjectData>>(json);
        Debug.Log($"GameObjects loaded from {fileName}");

        return wrapper.list;
    }

    //======================��ȯ �� ���� ����========================

    public static PotData ToPotData(Pot pot)
    {
        return new PotData
        {
            id = pot.id,
            currentState = pot.currentState.ToString(), // enum�� ���ڿ��� ����
            magicID = pot.magicID,
            basicMaterial = new List<int>(pot.basicMaterial), // ����Ʈ ����
            totalCraftingTime = pot.totalCraftingTime,
            remainingTime = pot.remainingTime,
            position = pot.transform.position,
            rotation = pot.transform.eulerAngles
        };
    }

    public static Pot FromPotData(PotData data, GameObject potPrefab, Transform parent = null)
    {
        GameObject potObject = GameObject.Instantiate(potPrefab, data.position, Quaternion.Euler(data.rotation), parent);
        Pot pot = potObject.GetComponent<Pot>();

        pot.id = data.id;
        pot.currentState = Enum.Parse<Pot.PotState>(data.currentState); // ���ڿ��� enum���� ��ȯ
        pot.magicID = data.magicID;
        pot.basicMaterial = new List<int>(data.basicMaterial); // ����Ʈ ����
        pot.totalCraftingTime = data.totalCraftingTime;
        pot.remainingTime = data.remainingTime;

        return pot;
    }
}
