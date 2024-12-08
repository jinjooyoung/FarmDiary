using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDatas
{
    //=========================직렬화 클래스=========================

    [System.Serializable]
    public class Wrapper<T>
    {
        public List<T> list;
    }

    [System.Serializable]
    public class GameObjectData
    {
        public string name;       // 게임 오브젝트 이름
        public Vector3 position;  // 위치
        public Vector3 rotation;  // 회전
        public Vector3 scale;     // 크기
    }

    [System.Serializable]
    public class PrefabData
    {
        public int id; // 인스펙터에서 설정할 ID
        public GameObject prefab; // 실제 프리팹
    }

    [System.Serializable]
    public class PotData
    {
        public int id;                      // 솥의 고유 ID
        public string currentState;         // 솥의 현재 상태 (enum을 문자열로 저장)
        public int magicID;                 // 선택된 마법 작물 ID
        public List<int> basicMaterial;     // 선택된 일반 작물 ID 배열
        public float totalCraftingTime;     // 총 제작 시간
        public float remainingTime;         // 남은 제작 시간
        public Vector3 position;            // 솥의 위치
        public Vector3 rotation;            // 솥의 회전
    }

    //=========================세이브 로직=========================

    // GameObject 리스트를 저장하는 로직
    public static void SaveGameObjects(string fileName, List<GameObject> gameObjects)
    {
        List<GameObjectData> dataList = new List<GameObjectData>();

        // GameObject 데이터를 GameObjectData로 변환
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

        // JSON으로 변환
        string json = JsonUtility.ToJson(new Wrapper<GameObjectData> { list = dataList }, true);

        // SaveSystem을 사용하여 저장
        SaveSystem.Save(json, fileName);

        Debug.Log($"GameObjects saved to {fileName}");
    }

    //=========================로드 로직=========================

    // 저장된 GameObject 데이터를 로드하는 로직
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

    //======================변환 및 복원 로직========================

    public static PotData ToPotData(Pot pot)
    {
        return new PotData
        {
            id = pot.id,
            currentState = pot.currentState.ToString(), // enum을 문자열로 저장
            magicID = pot.magicID,
            basicMaterial = new List<int>(pot.basicMaterial), // 리스트 복사
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
        pot.currentState = Enum.Parse<Pot.PotState>(data.currentState); // 문자열을 enum으로 변환
        pot.magicID = data.magicID;
        pot.basicMaterial = new List<int>(data.basicMaterial); // 리스트 복사
        pot.totalCraftingTime = data.totalCraftingTime;
        pot.remainingTime = data.remainingTime;

        return pot;
    }
}
