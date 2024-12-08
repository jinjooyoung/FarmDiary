using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public ObjectsDatabaseSO objectsdatabase;

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
    public void SaveOBJs()
    {
        objDataList.Clear();  // 리스트 초기화

        // placedGameObjects에 있는 모든 게임 오브젝트에 대해 처리
        for (int i = 0; i < objPlacer.placedGameObjects.Count; i++)
        {
            GameObject placedObj = objPlacer.placedGameObjects[i];

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
                    Index = i
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
        }

        // OBJData 리스트를 JSON으로 변환하여 저장
        string json = JsonUtility.ToJson(new DataListWrapper<OBJData> { DataList = objDataList });

        // SaveSystem의 Save 메서드를 사용하여 파일에 저장
        SaveSystem.Save(json, "PlacedObjects");
        Debug.Log("저장 완료: 오브젝트");
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
        Debug.Log("저장 완료: AI 데이터");
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
        Debug.Log("저장 완료: StoredCrops");
    }

    //-------------------------------------------------------------------

    // 저장한 모든 오브젝트 다시 생성, 데이터 생성 으로 로드
    public void LoadOBJs()
    {
        // SaveSystem의 Load 메서드를 사용하여 JSON 데이터를 파일에서 읽어오기
        string json = SaveSystem.Load("PlacedObjects");

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

        int currentForeachIndex = 0;

        // 로드한 OBJData 리스트를 설치했던 순서대로 순회하며 오브젝트를 복원
        foreach (OBJData objData in sortedObjDataList)
        {
            int selectedCropIndex = objectsdatabase.objectsData.FindIndex(data => data.ID == objData.ID);

            GameObject prefab = GetPrefabFromResourcesByID(objData.ID);     // ID로 프리팹 받아옴

            if (prefab == null)     // 프리팹이 존재하지 않으면
            {
                Debug.LogWarning($"ID {objData.ID}에 해당하는 프리팹이 Resources 폴더에 없습니다!");
                continue;
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
            List<Vector3Int> positionToOccupy = CalculatePositions(objData.objPosition, objectsdatabase.objectsData[selectedCropIndex].Size);
            PlacementData data = new PlacementData(positionToOccupy, objData.ID, currentForeachIndex);
            
            if (objData.ID < 4)     // 밭 오브젝트라면
            {
                foreach (var pos in positionToOccupy)
                {
                    // 애초에 존재하는 것을 저장한거기 때문에 그 위치에 다른 정보가 저장되어있는지 확인 불필요
                    gridData.placedFields[pos] = data;  // 딕셔너리에 정보 저장
                }
            }
            else if (objData.ID > 3 && objData.ID < 9)      // 시설 오브젝트라면
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
                // 그 위치에 존재하는 밭을 받아옴. 밭이 이미 존재하고 그 위치의 밭의 OBJ 인덱스가 현재 작물 OBJ 인덱스보다 작아야함
                // 근데 항상 밭이 먼저 설치되고 거기에 작물을 설치하니까 정보 받아올 수 있을거임
                GameObject fieldObject = GetFieldObjectAt(objData.objPosition);
                newObject.transform.SetParent(fieldObject.transform);   // 작물 오브젝트를 밭 오브젝트의 하위로 보냄
                Crop cropScript = newObject.GetComponent<Crop>();              // Crop 스크립트 가져오기

                if (cropScript != null)
                {
                    // 작물의 초기화 및 성장 상태 반영
                    cropScript.Initialize(objectsdatabase.objectsData[selectedCropIndex].GrowthTimes);

                    // 초기화 이후 씨앗을 심어 상태를 설정
                    cropScript.PlantSeed();

                    // CropGrowthManager에 등록
                    CropGrowthManager.Instance.RegisterCrop(cropScript, objData.objPosition);

                    // 저장된 cropState, currentStage, growthStartTime 적용
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

            currentForeachIndex++;
        }

        Debug.Log("로드 완료: 오브젝트");
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

        Debug.Log("로드 완료: AI 데이터");
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

    private GameObject GetFieldObjectAt(Vector3Int gridPosition)
    {
        if (gridData.placedFields.TryGetValue(gridPosition, out PlacementData placement))
        {
            int objIndex = placement.PlacedObjectIndex;

            // 인덱스가 유효한 경우 OBJList에서 오브젝트를 반환
            if (objIndex > -1 && objIndex < objPlacer.placedGameObjects.Count)
            {
                return objPlacer.placedGameObjects[objIndex];
            }
        }

        return null; // 위치에 밭 오브젝트가 없거나 유효하지 않으면 null 반환
    }
}