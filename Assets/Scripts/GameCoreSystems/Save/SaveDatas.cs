using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Crop;

public class SaveDatas : MonoBehaviour
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
    public class CropData
    {
        public int cropId;                // ���� ID
        public Vector3Int seedPosition;   // �۹� �ɾ��� ��ġ
        public SeedPlantedState seedPlantedState;
        public CropState cropState;       // �۹� ���� (Enum)

        public SerializableHashSet<Vector3Int> cropsPos = new SerializableHashSet<Vector3Int>();  // ����ȭ ������ HashSet
    }

    [System.Serializable]
    public class PrefabObjectdata
    {
        public Vector3 position; // ������Ʈ�� ��ġ
        public int prefabID;     // �ش� ������Ʈ�� ������ ID
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
    public void SaveGameObjects(string fileName, List<GameObject> gameObjects)
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

    // Crop ������Ʈ(������)�� �����ϴ� �޼���
    public void SaveCrops(List<GameObject> gameOBJ)
    {
        List<GameObject> safeGameOBJ = new List<GameObject>(gameOBJ);

        List<PrefabObjectdata> savedObjects = new List<PrefabObjectdata>();  // ������ ������Ʈ ����ȭ ����Ʈ
        Debug.LogError("CropOBJ ���� ȣ���");

        foreach (GameObject obj in safeGameOBJ)  // ������ ��� ������Ʈ�� ���鼭
        {
            if (obj != null)
            {
                Debug.LogError("CropOBJ ��ġ�� ������Ʈ �ݺ� �˻� ��");
                // Crop ������Ʈ�� �پ� �ִ��� Ȯ��
                Crop crop = obj.GetComponent<Crop>();
                if (crop != null) // Crop�� �ִ� ������Ʈ�� ����
                {
                    Debug.LogError("CropOBJ ��ġ�� ������Ʈ Crop ã��!!!");
                    // ������Ʈ�� ID�� ��ġ�� ����
                    PrefabObjectdata savedObject = new PrefabObjectdata
                    {
                        position = obj.transform.position,
                        prefabID = crop.ID  // Crop�� ID ����
                    };
                    Debug.LogError($"CropOBJ ����!! ��ġ : {obj.transform.position}, ID : {crop.ID}");

                    savedObjects.Add(savedObject);  // ����Ʈ�� �߰�
                }
            }
        }

        // ����� �����͸� �����̳� PlayerPrefs�� ������ �� ����
        // ���÷� PlayerPrefs�� ����
        string json = JsonUtility.ToJson(new Wrapper<PrefabObjectdata> { list = savedObjects }, true);
        Debug.LogError($"CropOBJ ����Ϸ�! {json}");
        // SaveSystem�� ����Ͽ� ����
        SaveSystem.Save(json, "CropOBJ");
    }

    // ObjectDatabase�� ���Ű����� �����ϴ� ���� (ObjectDatabase�� �ٸ� ��ҵ��� ������ ����Ǿ ��������� ���)
    public void SaveObjectDatabase(ObjectsDatabaseSO objectsDatabase)
    {
        List<int> buyPrices = new List<int>();
        foreach (var objectData in objectsDatabase.objectsData)
        {
            buyPrices.Add(objectData.BuyPrice); // ���� ���ݸ� ����
        }

        // ����: ���� ������ JSON���� ����
        string saveString = JsonUtility.ToJson(buyPrices);
        SaveSystem.Save(saveString, "ObjectDatabasePrice");  // ���� ���ݸ� ����� ������
    }

    //=========================�ε� ����=========================

    // ����� GameObject �����͸� �ε��ϴ� ����
    public List<GameObjectData> LoadGameObjects(string fileName)
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

    // Crop ������Ʈ�� �ε��ϴ� �޼���
    public void LoadCrops()
    {
        // ����� �����͸� �����̳� PlayerPrefs���� �ҷ���
        string json = SaveSystem.Load("CropOBJ");  // SaveSystem�� ����Ͽ� ����� �����͸� �ҷ���
        Debug.LogError("CropOBJ �ε� ȣ���");

        // �����Ͱ� ���� ���
        if (!string.IsNullOrEmpty(json))
        {
            Debug.LogError("CropOBJ ������ ������");
            // JSON�� PrefabObjectdata ����Ʈ�� ��ȯ
            List<PrefabObjectdata> savedObjects = JsonUtility.FromJson<Wrapper<PrefabObjectdata>>(json).list;

            // ����� ������Ʈ���� ���� �ٽ� ����
            foreach (PrefabObjectdata savedObject in savedObjects)
            {
                Debug.LogError("CropOBJ ����ȭ ����Ʈ �ݺ� ��");
                // ID�� �ش� �������� ã�� ����
                GameObject prefab = Resources.Load<GameObject>($"Prefabs/{savedObject.prefabID}");
                if (prefab != null)
                {
                    Debug.LogError("CropOBJ ������ ã�Ƽ� ���� �Ϸ�");
                    // �������� ����� ��ġ�� ����
                    Instantiate(prefab, savedObject.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogError("CropOBJ ������ ��ã��!! ���� ����!!!");
                }
            }
        }
    }

    //======================��ȯ �� ���� ����========================

    public PotData ToPotData(Pot pot)
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

    public Pot FromPotData(PotData data, GameObject potPrefab, Transform parent = null)
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

[System.Serializable]
public class SerializableHashSet<T> where T : IEquatable<T>
{
    [SerializeField]
    private List<T> list = new List<T>();  // ����ȭ ������ List�� ����

    // HashSet�� ���� �߰�
    public void Add(T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
    }

    // HashSet���� ���� ����
    public void Remove(T item)
    {
        list.Remove(item);
    }

    // HashSet�� ���� �ִ��� Ȯ��
    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    // ����ȭ�� List�� ��ȯ
    public List<T> GetList()
    {
        return list;
    }

    // List�� HashSet���� ��ȯ
    public HashSet<T> ToHashSet()
    {
        return new HashSet<T>(list);
    }

    // List�� HashSet���� �ʱ�ȭ
    public void FromList(List<T> list)
    {
        this.list = list;
    }
}
