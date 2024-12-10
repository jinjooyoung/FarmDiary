using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Crop;
using static Pot;

public class NewSaveData : MonoBehaviour
{
    [Header("설치 오브젝트 데이터를 위한 참조")]
    public Grid grid;
    public PlacementSystem placementSystem;
    public OBJPlacer objPlacer;
    public GridData gridData;

    [Header("데이터 베이스 SO를 위한 참조")]
    public ObjectsDatabaseSO objectsdatabaseSO;
    public AchievementsDatabaseSO achievementsDatabaseSO;

    [Header("AI 데이터 및 현재 시간 체크를 위한 참조")]
    public Transform playerPos;
    public AIStateManager aIStateManager;
    public CropGrowthManager cropGrowthManager;

    [Header("창고 데이터를 위한 참조")]
    public Storage storage;

    // 저장할 데이터 리스트
    private List<OBJData> objDataList = new List<OBJData>();


    private void Awake()
    {
        // 같은 GridData 인스턴스를 공유
        gridData = placementSystem.placedOBJData;
    }

    //========================================================================

    // AI 데이터 저장
    [System.Serializable]
    public class MainData
    {
        //public int currentCropIndexByCropGrowthManagerList;
        public Vector3 playerPosition;
        public List<int> harvestedCropByID;
        public int waterAmount;
        public float savedTime;
        //public string stateName;
    }

    // 생성된 모든 오브젝트의 ID와 위치(gridPos), 설치 순서 (index)를 저장
    [System.Serializable]
    public class OBJData
    {
        public int ID;                  // 고유 ID
        public Vector3Int objPosition;  // 설치 그리드 위치
        public int Index;               // 설치 순서
        public CropData cropData;
        public PotData potData;
    }

    // 작물 데이터
    [System.Serializable]
    public class CropData
    {
        public CropState cropStateData;     // 현재 작물 상태
        public int currentStageData;        // 작물 성장 단계
        public float growthStartTime;       // 작물 성장 시작 시간
    }

    // 솥 데이터
    [System.Serializable]
    public class PotData
    {
        public PotState potStateData;       // 현재 솥의 상태
        public int magicIDData;             // 선택한 마법 작물    선택 안 했다면 기본 값 -1
        public List<int> selectedCropData;  // 선택한 일반 작물 리스트
        public float remainingTimeData;     // 남은 제작 시간
    }

    // 업적 SO 데이터
    [System.Serializable]
    public class AchievementsSOData
    {
        public int progressData;        // 업적 진행도
        public bool IsUnlockedData;     // 업적 해금 유무
        public bool IsClearData;        // 업적 클리어 유무
    }

    // 오브젝트 구매 가격 SO 데이터
    [System.Serializable]
    public class OBJSOData
    {
        public int BuyPriceData;        // 오브젝트 구매 가격
    }

    //========================================================================

    // OBJData 리스트를 담을 래퍼 클래스
    [System.Serializable]
    public class DataListWrapper<T>
    {
        public List<T> DataList;
    }

    // ID를 받아서 프리팹 오브젝트를 리턴받는 함수    로드에서 사용
    public GameObject GetPrefabFromResourcesByID(int id)
    {
        return Resources.Load<GameObject>($"Prefabs/{id}");
    }

    //===================================

    // 설치된 모든 오브젝트 ID, 그리드 위치, 설치 순서 3가지 저장이 기본, 만약 Crop 스크립트가 있는 작물 오브젝트라면
    // 추가적으로 작물의 상태, 단계, 시간을 저장함
    public async Task SaveOBJs()
    {
        objDataList.Clear();  // 리스트 초기화

        // placedGameObjects에 있는 모든 게임 오브젝트에 대해 비동기적으로 처리
        var tasks = objPlacer.placedGameObjects.Select(async placedObj =>
        {
            if (placedObj != null)
            {
                // 게임 오브젝트 이름에서 ID를 추출 (예: "123(Clone)"에서 123을 추출)
                int objectID = int.Parse(placedObj.name.Split('(')[0]);

                // 게임 오브젝트의 transform.position을 Grid의 Cell로 변환
                Vector3Int objPosition = grid.WorldToCell(placedObj.transform.position);

                // OBJData 생성
                OBJData objData = new OBJData
                {
                    ID = objectID,
                    objPosition = objPosition,
                    Index = objDataList.Count // 해당 오브젝트가 리스트에서 차지할 인덱스
                };

                // 작물이라면
                Crop crop = placedObj.GetComponent<Crop>();
                if (crop != null)
                {
                    CropData cropData = new CropData
                    {
                        cropStateData = crop.cropState,         // 작물 상태 Enum
                        currentStageData = crop.currentStage,   // 작물 성장 단계
                        growthStartTime = crop.growthStartTime  // 작물 성장 시작 시간
                    };

                    // CropData도 objData에 추가
                    objData.cropData = cropData;
                }

                // 솥이라면
                Pot pot = placedObj.GetComponent<Pot>();
                if (pot != null)
                {
                    PotData potData = new PotData
                    {
                        potStateData = pot.currentState,
                        magicIDData = pot.magicID,
                        selectedCropData = new List<int>(pot.basicMaterial),
                        remainingTimeData = pot.remainingTime
                    };

                    // potData도 objData에 추가
                    objData.potData = potData;
                }

                // 리스트에 추가
                objDataList.Add(objData);
            }
        });

        // 모든 비동기 작업이 완료될 때까지 기다림
        await Task.WhenAll(tasks);

        // OBJData 리스트를 JSON으로 변환하여 저장
        string json = JsonUtility.ToJson(new DataListWrapper<OBJData> { DataList = objDataList });

        // SaveSystem의 Save 메서드를 사용하여 파일에 저장
        await SaveSystem.SaveAsync(json, "PlacedObjects");
        Debug.LogWarning("저장 완료: 오브젝트");
    }

    // AI 데이터 저장 (AI 위치, 수확해서 AI가 갖고 있는 작물 개수, AI가 갖고있는 물 개수)
    public void SaveAIData()
    {
        // AIData 객체 생성 및 데이터 할당
        MainData aiData = new MainData
        {
            // 플레이어의 현재 위치 저장
            playerPosition = playerPos.position,

            // AIStateManager의 수확된 작물 ID 리스트 저장
            harvestedCropByID = new List<int>(aIStateManager.harvestedCrops),

            // AIStateManager의 현재 물의 양 저장
            waterAmount = aIStateManager.currentWaterAmount,

            // 현재 시간 저장
            savedTime = cropGrowthManager.currentTime
        };

        // JSON 직렬화
        string json = JsonUtility.ToJson(aiData);

        // SaveSystem의 Save 메서드로 파일 저장
        SaveSystem.Save(json, "AIData");
        Debug.LogWarning("저장 완료: AI 데이터");
    }

    // 창고에 존재하는 작물의 양을 저장
    public void SaveStorage()
    {
        if (storage == null || storage.storedCropsByID == null)
        {
            Debug.LogWarning("Storage 또는 저장할 데이터가 없습니다!");
            return;
        }

        // CropStorage 데이터를 DataListWrapper로 감싸기
        DataListWrapper<CropStorage> cropWrapper = new DataListWrapper<CropStorage>
        {
            DataList = storage.storedCropsByID
        };

        // JSON 직렬화
        string json = JsonUtility.ToJson(cropWrapper);

        // 파일에 저장
        SaveSystem.Save(json, "StoredCrops");
        Debug.LogWarning("저장 완료: StoredCrops");
    }

    // 업적에서 변경되는 값만 따로 저장
    public void SaveAchievements()
    {
        // 업적 데이터 리스트에서 Progress, IsUnlocked, Clear 값만 저장
        List<AchievementsSOData> achievementsToSave = new List<AchievementsSOData>();

        foreach (var achievement in achievementsDatabaseSO.achievementsData)
        {
            // 필요한 데이터만 저장
            AchievementsSOData saveData = new AchievementsSOData
            {
                progressData = achievement.Progress,
                IsUnlockedData = achievement.IsUnlocked,
                IsClearData = achievement.Clear
            };

            achievementsToSave.Add(saveData);
        }

        // AchievementData 리스트를 DataListWrapper로 감싸기
        DataListWrapper<AchievementsSOData> achievementWrapper = new DataListWrapper<AchievementsSOData>
        {
            DataList = achievementsToSave
        };

        // JSON 직렬화
        string json = JsonUtility.ToJson(achievementWrapper);

        // 파일에 저장
        SaveSystem.Save(json, "Achievements");
        Debug.LogWarning("저장 완료: Achievements");
    }

    // 변동되는 구매 가격 따로 저장
    public void SaveBuyPrice()
    {
        List<OBJSOData> buyPriceList = new List<OBJSOData>();

        // objectsData에서 0번부터 7번까지 BuyPrice만 가져와서 OBJSOData 리스트에 저장
        // 다른 오브젝트 데이터는 구매 가격 변동이 없음
        for (int i = 0; i < 8 && i < objectsdatabaseSO.objectsData.Count; i++)
        {
            OBJSOData data = new OBJSOData
            {
                BuyPriceData = objectsdatabaseSO.objectsData[i].BuyPrice
            };
            buyPriceList.Add(data);
        }

        // OBJSOData 리스트를 JSON 형식으로 직렬화
        string json = JsonUtility.ToJson(new DataListWrapper<OBJSOData> { DataList = buyPriceList });

        // SaveSystem을 사용해 데이터를 저장
        SaveSystem.Save(json, "ObjectsBuyPrice");
        Debug.LogWarning("저장 완료: ObjectsBuyPrice");
    }

    //-------------------------------------------------------------------

    // 저장한 모든 오브젝트 다시 생성, 데이터 생성 으로 로드
    public async Task LoadOBJs()
    {
        // SaveSystem의 Load 메서드를 사용하여 JSON 데이터를 파일에서 읽어오기
        string json = await SaveSystem.LoadAsync("PlacedObjects");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("저장된 파일이 없습니다!");
            return;
        }

        // JSON 데이터를 OBJData 리스트로 디시리얼라이즈
        DataListWrapper<OBJData> dataWrapper = JsonUtility.FromJson<DataListWrapper<OBJData>>(json);

        if (dataWrapper == null || dataWrapper.DataList == null || dataWrapper.DataList.Count == 0)
        {
            Debug.LogWarning("로드할 오브젝트 데이터가 없습니다!");
            return;
        }

        // 현재 씬에 이미 존재하는 오브젝트가 있다면 제거
        foreach (GameObject obj in objPlacer.placedGameObjects)
        {
            Destroy(obj); // 기존에 배치된 오브젝트 삭제
        }
        objPlacer.placedGameObjects.Clear(); // 리스트 초기화

        // OBJData 리스트를 Index 값 = 설치된 순서 기준으로 정렬
        var sortedObjDataList = dataWrapper.DataList.OrderBy(objData => objData.Index).ToList();

        // 밭 오브젝트 로드
        var fieldDataList = sortedObjDataList.Where(objData => objData.ID < 4).ToList();
        var fieldTasks = fieldDataList.Select(async objData =>
        {
            int selectedCropIndex = objectsdatabaseSO.objectsData.FindIndex(data => data.ID == objData.ID);
            GameObject prefab = GetPrefabFromResourcesByID(objData.ID);

            if (prefab == null)
            {
                Debug.LogWarning($"ID {objData.ID}에 해당하는 프리팹이 Resources 폴더에 없습니다!");
                return;
            }

            GameObject newObject = Instantiate(prefab);
            newObject.transform.position = grid.CellToWorld(objData.objPosition);

            objPlacer.placedGameObjects.Add(newObject);

            // 설치 이후 투명도 1.0으로 복구
            SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
            Color color = newObjectRenderer.color;
            color.a = 1.0f;
            newObjectRenderer.color = color;

            // 오브젝트가 차지하는 모든 그리드 포지션 리스트
            List<Vector3Int> positionToOccupy = CalculatePositions(objData.objPosition, objectsdatabaseSO.objectsData[selectedCropIndex].Size);
            PlacementData data = new PlacementData(positionToOccupy, objData.ID, objData.Index);

            foreach (var pos in positionToOccupy)
            {
                gridData.placedFields[pos] = data;  // 딕셔너리에 정보 저장
            }
        });

        await Task.WhenAll(fieldTasks); // 모든 밭 오브젝트 로드 완료 대기

        var otherDataList = sortedObjDataList.Where(objData => objData.ID >= 4).ToList();

        // 비동기 작업을 위해 Task.Run을 사용
        var otherTasks = otherDataList.Select(async objData =>
        {
            int selectedCropIndex = objectsdatabaseSO.objectsData.FindIndex(data => data.ID == objData.ID);
            GameObject prefab = GetPrefabFromResourcesByID(objData.ID);     // ID로 프리팹 받아옴

            if (prefab == null)
            {
                Debug.LogWarning($"ID {objData.ID}에 해당하는 프리팹이 Resources 폴더에 없습니다!");
                return;
            }

            GameObject newObject = Instantiate(prefab);                             // 오브젝트 생성
            newObject.transform.position = grid.CellToWorld(objData.objPosition);   // 생성한 OBJ를 그리드 포지션을 월드 포지션으로 바꿔서 이동

            Pot potComponent = newObject.GetComponent<Pot>();

            if (potComponent != null)
            {
                PotionManager.instance.AddPot(newObject);
                objPlacer.potCount++;
            }

            objPlacer.placedGameObjects.Add(newObject);               // 설치된 오브젝트 리스트에 저장

            // 설치 이후 투명도 1.0으로 복구
            SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
            Color color = newObjectRenderer.color;
            color.a = 1.0f;
            newObjectRenderer.color = color;

            // 오브젝트가 차지하는 모든 그리드 포지션 리스트
            List<Vector3Int> positionToOccupy = CalculatePositions(objData.objPosition, objectsdatabaseSO.objectsData[selectedCropIndex].Size);
            PlacementData data = new PlacementData(positionToOccupy, objData.ID, objData.Index);

            // 각 오브젝트 ID에 따른 로직 처리 (시설, 작물, 꾸밈)
            if (objData.ID > 3 && objData.ID < 9)      // 시설 오브젝트라면
            {
                // 솥이라면
                Pot potScript = newObject.GetComponent<Pot>();
                if (potScript != null)
                {
                    potScript.currentState = objData.potData.potStateData;
                    potScript.magicID = objData.potData.magicIDData;
                    potScript.basicMaterial = objData.potData.selectedCropData;
                    potScript.remainingTime = objData.potData.remainingTimeData;

                    if (potScript.currentState == PotState.Crafting)
                    {
                        potScript.animator.SetBool("IsCrafting", true);
                    }
                }

                foreach (var pos in positionToOccupy)
                {
                    gridData.placedFacilities[pos] = data;  // 딕셔너리에 정보 저장
                }
            }
            else if (objData.ID > 8 && objData.ID < 100)    // 작물 오브젝트라면
            {
                

                if (gridData.placedFields.TryGetValue(objData.objPosition, out PlacementData placement))
                {
                    int objIndex = placement.PlacedObjectIndex;

                    // 인덱스가 유효한 경우 OBJList에서 오브젝트를 반환
                    if (objIndex > -1 && objIndex < objPlacer.placedGameObjects.Count)
                    {
                        GameObject fieldObject = objPlacer.placedGameObjects[objIndex];
                        newObject.transform.SetParent(fieldObject.transform);   // 작물 오브젝트를 밭 오브젝트의 하위로 보냄
                    }
                }
                
                Crop cropScript = newObject.GetComponent<Crop>();              // Crop 스크립트 가져오기

                if (cropScript != null)
                {
                    cropScript.Initialize(objectsdatabaseSO.objectsData[selectedCropIndex].GrowthTimes);
                    cropScript.PlantSeed();
                    CropGrowthManager.Instance.RegisterCrop(cropScript, objData.objPosition);

                    cropScript.cropState = objData.cropData.cropStateData;
                    cropScript.currentStage = objData.cropData.currentStageData;
                    cropScript.growthStartTime = objData.cropData.growthStartTime;

                    cropScript.UpdateCropVisual();
                }

                foreach (var pos in positionToOccupy)
                {
                    gridData.placedCrops[pos] = data;  // 딕셔너리에 정보 저장
                }
            }
            else     // 나머지 꾸밈 오브젝트라면
            {
                foreach (var pos in positionToOccupy)
                {
                    gridData.placedDecos[pos] = data;  // 딕셔너리에 정보 저장
                }
            }

        });

        await Task.WhenAll(otherTasks); // 모든 비동기 작업이 완료될 때까지 대기

        Debug.LogWarning("로드 완료: 오브젝트");
    }

    // AI 데이터 로드해서 할당
    public void LoadAIData()
    {
        // SaveSystem의 Load 메서드를 사용하여 JSON 데이터를 로드
        string json = SaveSystem.Load("AIData");

        // 저장된 파일이 없는 경우 처리
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("저장된 AI 데이터가 없습니다!");
            return;
        }

        // JSON 데이터를 AIData 객체로 디시리얼라이즈
        MainData aiData = JsonUtility.FromJson<MainData>(json);

        if (aiData == null)
        {
            Debug.LogWarning("AI 데이터를 로드하는 데 실패했습니다!");
            return;
        }

        // 로드된 데이터를 각 변수에 다시 할당
        playerPos.position = aiData.playerPosition; // 플레이어 위치 복원
        aIStateManager.harvestedCrops = new List<int>(aiData.harvestedCropByID); // 수확된 작물 ID 리스트 복원
        aIStateManager.currentWaterAmount = aiData.waterAmount; // 물의 양 복원
        cropGrowthManager.loadTime = aiData.savedTime;
        cropGrowthManager.currentTime = aiData.savedTime;

        Debug.LogWarning("로드 완료: AI 데이터");
    }

    // 창고 데이터 로드
    public void LoadStorage()
    {
        // 저장된 JSON 데이터 읽어오기
        string json = SaveSystem.Load("StoredCrops");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("저장된 파일이 없습니다!");
            return;
        }

        // JSON 역직렬화
        DataListWrapper<CropStorage> cropWrapper = JsonUtility.FromJson<DataListWrapper<CropStorage>>(json);

        if (cropWrapper == null || cropWrapper.DataList == null)
        {
            Debug.LogWarning("로드할 데이터가 없습니다!");
            return;
        }

        // 저장된 데이터로 Storage의 storedCropsByID 덮어쓰기
        storage.storedCropsByID = new List<CropStorage>(cropWrapper.DataList);
        Debug.Log("로드 완료: StoredCrops");
    }

    // 업적 데이터 로드
    public void LoadAchievements()
    {
        // 저장된 JSON 데이터를 읽어옴
        string json = SaveSystem.Load("Achievements");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("저장된 Achievements 데이터가 없습니다.");
            return;
        }

        // JSON 데이터를 DataListWrapper로 변환
        DataListWrapper<AchievementsSOData> achievementWrapper = JsonUtility.FromJson<DataListWrapper<AchievementsSOData>>(json);

        // 업적 데이터 복원
        for (int i = 0; i < achievementWrapper.DataList.Count; i++)
        {
            // 저장된 값으로 해당 업적을 업데이트
            AchievementsSOData savedAchievementData = achievementWrapper.DataList[i];
            AchievementData achievement = achievementsDatabaseSO.achievementsData[i];

            achievement.SetProgress(savedAchievementData.progressData);

            if (savedAchievementData.IsUnlockedData)
            {
                achievement.Unlock();
            }
            
            if (savedAchievementData.IsClearData)
            {
                achievement.SetClear();
            }
        }

        Debug.LogWarning("로드 완료: Achievements");
    }

    // 오브젝트 구매가격 로드
    public void LoadBuyPrice()
    {
        // 저장된 JSON 데이터 불러오기
        string json = SaveSystem.Load("ObjectsBuyPrice");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("저장된 구매 가격 데이터가 없습니다!");
            return;
        }

        // JSON을 OBJSOData 리스트로 디시리얼라이즈
        DataListWrapper<OBJSOData> buyPriceDataWrapper = JsonUtility.FromJson<DataListWrapper<OBJSOData>>(json);

        if (buyPriceDataWrapper == null || buyPriceDataWrapper.DataList == null || buyPriceDataWrapper.DataList.Count == 0)
        {
            Debug.LogWarning("불러올 구매 가격 데이터가 없습니다!");
            return;
        }

        // objectsData의 0번부터 7번까지의 ID에 맞는 BuyPrice를 복원
        for (int i = 0; i < buyPriceDataWrapper.DataList.Count && i < objectsdatabaseSO.objectsData.Count; i++)
        {
            objectsdatabaseSO.objectsData[i].SetBuyPrice(buyPriceDataWrapper.DataList[i].BuyPriceData);
        }

        Debug.LogWarning("오브젝트 구매 가격 로드 완료!");
    }

    //========================================================

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {

        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, y, 0));
            }
        }
        return returnVal;
    }
}