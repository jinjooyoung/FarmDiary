using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewSaveData : MonoBehaviour
{
    public Grid grid;
    public PlacementSystem placementSystem;
    public OBJPlacer objPlacer;
    public GridData gridData;
    public ObjectsDatabaseSO objectsdatabase;

    // 저장할 데이터 리스트
    private List<OBJData> objDataList = new List<OBJData>();

    private void Awake()
    {
        // 같은 GridData 인스턴스를 공유
        gridData = placementSystem.placedOBJData;
    }

    // 생성된 모든 오브젝트의 ID와 위치(gridPos), 설치 순서 (index)를 저장
    [System.Serializable]
    public class OBJData
    {
        public int ID;                  // 고유 ID
        public Vector3Int objPosition; // 설치 그리드 위치
        public int Index;               // 설치 순서
    }

    // OBJData 리스트를 담을 래퍼 클래스
    [System.Serializable]
    public class OBJDataListWrapper
    {
        public List<OBJData> objDataList;
    }

    // ID를 받아서 프리팹 오브젝트를 리턴받는 함수    로드에서 사용
    public GameObject GetPrefabFromResourcesByID(int id)
    {
        return Resources.Load<GameObject>($"Prefabs/{id}");
    }

    //===================================

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

                // 리스트에 추가
                objDataList.Add(objData);
            }
        }

        // OBJData 리스트를 JSON으로 변환하여 저장
        string json = JsonUtility.ToJson(new OBJDataListWrapper { objDataList = objDataList });

        // SaveSystem의 Save 메서드를 사용하여 파일에 저장
        SaveSystem.Save(json, "PlacedObjects");
        Debug.Log("오브젝트 저장 완료");
    }

    //-------------------------------------------------------------------

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
        OBJDataListWrapper dataWrapper = JsonUtility.FromJson<OBJDataListWrapper>(json);

        if (dataWrapper == null || dataWrapper.objDataList == null || dataWrapper.objDataList.Count == 0)
        {
            Debug.LogWarning("로드할 데이터가 없습니다!");
            return;
        }

        // 현재 씬에 이미 존재하는 오브젝트가 있다면 제거
        foreach (GameObject obj in objPlacer.placedGameObjects)
        {
            Destroy(obj); // 기존에 배치된 오브젝트 삭제
        }
        objPlacer.placedGameObjects.Clear(); // 리스트 초기화

        // OBJData 리스트를 Index 값 = 설치된 순서 기준으로 정렬
        var sortedObjDataList = dataWrapper.objDataList.OrderBy(objData => objData.Index).ToList();

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
                    cropScript.Initialize(objectsdatabase.objectsData[selectedCropIndex].GrowthTimes);

                    // 초기화 이후 씨앗을 심어 상태를 설정
                    cropScript.PlantSeed();

                    // CropGrowthManager에 등록
                    CropGrowthManager.Instance.RegisterCrop(cropScript, objData.objPosition);
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

        Debug.Log("오브젝트 로드 완료");
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