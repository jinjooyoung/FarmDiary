using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Crop;

public class SaveDatas : MonoBehaviour
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
    public class CropData
    {
        public int cropId;                // 고유 ID
        public Vector3Int seedPosition;   // 작물 심어진 위치
        public SeedPlantedState seedPlantedState;
        public CropState cropState;       // 작물 상태 (Enum)

        public SerializableHashSet<Vector3Int> cropsPos = new SerializableHashSet<Vector3Int>();  // 직렬화 가능한 HashSet
    }

    [System.Serializable]
    public class PrefabObjectdata
    {
        public Vector3 position; // 오브젝트의 위치
        public int prefabID;     // 해당 오브젝트의 프리팹 ID
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
    public void SaveGameObjects(string fileName, List<GameObject> gameObjects)
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

    // Crop 오브젝트(껍데기)를 저장하는 메서드
    public void SaveCrops(List<GameObject> gameOBJ)
    {
        List<GameObject> safeGameOBJ = new List<GameObject>(gameOBJ);

        List<PrefabObjectdata> savedObjects = new List<PrefabObjectdata>();  // 저장할 오브젝트 직렬화 리스트
        Debug.LogError("CropOBJ 저장 호출됨");

        foreach (GameObject obj in safeGameOBJ)  // 생성된 모든 오브젝트를 돌면서
        {
            if (obj != null)
            {
                Debug.LogError("CropOBJ 설치된 오브젝트 반복 검사 중");
                // Crop 컴포넌트가 붙어 있는지 확인
                Crop crop = obj.GetComponent<Crop>();
                if (crop != null) // Crop이 있는 오브젝트만 저장
                {
                    Debug.LogError("CropOBJ 설치된 오브젝트 Crop 찾음!!!");
                    // 오브젝트의 ID와 위치를 저장
                    PrefabObjectdata savedObject = new PrefabObjectdata
                    {
                        position = obj.transform.position,
                        prefabID = crop.ID  // Crop의 ID 저장
                    };
                    Debug.LogError($"CropOBJ 저장!! 위치 : {obj.transform.position}, ID : {crop.ID}");

                    savedObjects.Add(savedObject);  // 리스트에 추가
                }
            }
        }

        // 저장된 데이터를 파일이나 PlayerPrefs에 저장할 수 있음
        // 예시로 PlayerPrefs에 저장
        string json = JsonUtility.ToJson(new Wrapper<PrefabObjectdata> { list = savedObjects }, true);
        Debug.LogError($"CropOBJ 저장완료! {json}");
        // SaveSystem을 사용하여 저장
        SaveSystem.Save(json, "CropOBJ");
    }

    // ObjectDatabase의 구매가격을 저장하는 로직 (ObjectDatabase의 다른 요소들은 게임이 진행되어도 변경사항이 없어서)
    public void SaveObjectDatabase(ObjectsDatabaseSO objectsDatabase)
    {
        List<int> buyPrices = new List<int>();
        foreach (var objectData in objectsDatabase.objectsData)
        {
            buyPrices.Add(objectData.BuyPrice); // 구매 가격만 저장
        }

        // 예시: 구매 가격을 JSON으로 저장
        string saveString = JsonUtility.ToJson(buyPrices);
        SaveSystem.Save(saveString, "ObjectDatabasePrice");  // 구매 가격만 저장된 데이터
    }

    //=========================로드 로직=========================

    // 저장된 GameObject 데이터를 로드하는 로직
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

    // Crop 오브젝트를 로드하는 메서드
    public void LoadCrops()
    {
        // 저장된 데이터를 파일이나 PlayerPrefs에서 불러옴
        string json = SaveSystem.Load("CropOBJ");  // SaveSystem을 사용하여 저장된 데이터를 불러옴
        Debug.LogError("CropOBJ 로드 호출됨");

        // 데이터가 있을 경우
        if (!string.IsNullOrEmpty(json))
        {
            Debug.LogError("CropOBJ 데이터 존재함");
            // JSON을 PrefabObjectdata 리스트로 변환
            List<PrefabObjectdata> savedObjects = JsonUtility.FromJson<Wrapper<PrefabObjectdata>>(json).list;

            // 저장된 오브젝트들을 씬에 다시 생성
            foreach (PrefabObjectdata savedObject in savedObjects)
            {
                Debug.LogError("CropOBJ 직렬화 리스트 반복 중");
                // ID로 해당 프리팹을 찾아 생성
                GameObject prefab = Resources.Load<GameObject>($"Prefabs/{savedObject.prefabID}");
                if (prefab != null)
                {
                    Debug.LogError("CropOBJ 프리팹 찾아서 생성 완료");
                    // 프리팹을 저장된 위치에 생성
                    Instantiate(prefab, savedObject.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogError("CropOBJ 프리팹 못찾음!! 생성 실패!!!");
                }
            }
        }
    }

    //======================변환 및 복원 로직========================

    public PotData ToPotData(Pot pot)
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

    public Pot FromPotData(PotData data, GameObject potPrefab, Transform parent = null)
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

[System.Serializable]
public class SerializableHashSet<T> where T : IEquatable<T>
{
    [SerializeField]
    private List<T> list = new List<T>();  // 직렬화 가능한 List로 저장

    // HashSet에 값을 추가
    public void Add(T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
    }

    // HashSet에서 값을 제거
    public void Remove(T item)
    {
        list.Remove(item);
    }

    // HashSet에 값이 있는지 확인
    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    // 직렬화된 List를 반환
    public List<T> GetList()
    {
        return list;
    }

    // List를 HashSet으로 변환
    public HashSet<T> ToHashSet()
    {
        return new HashSet<T>(list);
    }

    // List를 HashSet으로 초기화
    public void FromList(List<T> list)
    {
        this.list = list;
    }
}
