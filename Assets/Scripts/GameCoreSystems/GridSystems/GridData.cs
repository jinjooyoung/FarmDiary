using System.Collections.Generic;
using System;
using UnityEngine;
using static Crop;

[System.Serializable]
public class GridData
{
    // 그리드 셀의 위치(Vector3Int)를 키로 사용하고, 그 위치에 배치된 오브젝트의 정보
    public Dictionary<Vector3Int, PlacementData> placedFields = new();
    public Dictionary<Vector3Int, PlacementData> placedDecos = new();
    public Dictionary<Vector3Int, PlacementData> placedFacilities = new();
    public Dictionary<Vector3Int, PlacementData> placedCrops = new();

    // 설치할 오브젝트의 정보를 저장하고 한 번 더 설치 가능한지 체크하는 메서드
    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // 설치할 위치의 그리드 포지션을 받아와서 리스트에 저장
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // 설치할 오브젝트의 정보를 저장
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        // foreach 반복을 돌면서 이미 차지된 칸이 있는지 확인
        // 어짜피 이 함수를 호출하는 과정에서 설치 가능 유무를 체크하지만 한 번 더 체크함으로서 혹시 모를 버그를 예방
        if (ID < 5) // 밭 ID는 밭 딕셔너리에 추가
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                placedFields[pos] = data;
                // 오브젝트가 차지하는 칸의 pos마다 오브젝트의 정보를 딕셔너리에 저장
            }
        }
        else if (ID >= 200 && ID < 300)     // 데코오브젝트 ID는 데코 딕셔너리에 추가
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                placedDecos[pos] = data;
                // 오브젝트가 차지하는 칸의 pos마다 오브젝트의 정보를 딕셔너리에 저장
            }
        }
        else  // 그 외의 ID는 장비 오브젝트이므로 장비 딕셔너리에 저장 (작물 ID는 어짜피 밑에서 호출하기때문에)
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
                }
                placedFacilities[pos] = data;
                // 오브젝트가 차지하는 칸의 pos마다 오브젝트의 정보를 딕셔너리에 저장
            }
        }
    }

    // 설치할 작물의 정보를 저장하고 한 번 더 설치 가능한지 체크하는 메서드
    public void AddCropAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex, 
                            CropState cropState, 
                            SeedPlantedState seedPlantedState)
    {
        // 설치할 위치의 그리드 포지션을 받아와서 리스트에 저장
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // 설치할 오브젝트의 정보를 저장
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        // foreach 반복을 돌면서 이미 차지된 칸이 있는지 확인
        // 어짜피 이 함수를 호출하는 과정에서 설치 가능 유무를 체크하지만 한 번 더 체크함으로서 혹시 모를 버그를 예방
        foreach (var pos in positionToOccupy)
        {
            if (placedCrops.ContainsKey(pos))
            {
                throw new Exception($"Dictionary 에 이미 이 위치가 존재합니다. {pos}");
            }
            placedCrops[pos] = data;
            // 오브젝트가 차지하는 칸의 pos마다 오브젝트의 정보를 딕셔너리에 저장
        }
    }

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

    // 오브젝트의 사이즈를 받아와서 그 위치에 설치 가능한지 bool값 리턴
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // 오브젝트가 차지할(아직 설치 X 예정) 그리드 칸의 위치를 리스트에 저장
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);

        // 저장된 칸들을 돌면서 그 위치가 이미 다른 오브젝트로 차지되어 있는지 계산
        foreach (var pos in positionToOccupy)
        {
            if (placedFields.ContainsKey(pos) || pos.y > -7 || placedDecos.ContainsKey(pos) || placedFacilities.ContainsKey(pos))     // 한 칸이라도 이미 차지된 위치가 있으면 false를 반환하여 설치 불가능
            {
                return false;
            }
            //Debug.Log(pos);
        }
        return true;        // 설치 가능이면 true 리턴함
    }

    // 선택한 그리드 위치를 통해 그곳에 설치된 오브젝트의 인덱스를 반환하는 메서드
    // 여기에서 말하는 오브젝트 인덱스는 스크립터블오브젝트의 인덱스가 아닌 GridData에 설치할 때 마다 추가되는 리스트의 인덱스
    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedFields.ContainsKey(gridPosition) == false)       // 아무것도 설치되어 있지 않은 칸이라면 -1 리턴
        {
            return -1;
        }
        return placedFields[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedFields[gridPosition].occupiedPositions)
        {
            placedFields.Remove(pos);
            placedCrops.Remove(pos);
        }
    }

    internal void RemoveCropAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedCrops[gridPosition].occupiedPositions)
        {
            placedCrops.Remove(pos);
        }
    }

    public string SaveFieldsAndCrops()
    {
        GridDataSave saveData = new GridDataSave();

        // 작물 저장
        foreach (var crop in placedCrops)
        {
            Renderer renderer = crop.Value != null ? GetRendererFromPositions(crop.Key) : null;
            float alpha = renderer != null ? renderer.material.color.a : 1.0f;

            saveData.crops.Add(new GridCropSave
            {
                position = crop.Key,
                placementData = new PlacementData(
                    crop.Value.occupiedPositions,
                    crop.Value.ID,
                    crop.Value.PlacedObjectIndex,
                    crop.Value.cropState,
                    crop.Value.seedPlantedState,
                    alpha // 알파 값 저장
                ),
                id = crop.Value.ID
            });
        }

        // 밭 저장
        foreach (var field in placedFields)
        {
            Renderer renderer = field.Value != null ? GetRendererFromPositions(field.Key) : null;
            float alpha = renderer != null ? renderer.material.color.a : 1.0f;

            saveData.fields.Add(new GridFieldSave
            {
                position = field.Key,
                placementData = new PlacementData(
                    field.Value.occupiedPositions,
                    field.Value.ID,
                    field.Value.PlacedObjectIndex,
                    field.Value.cropState,
                    field.Value.seedPlantedState,
                    alpha // 알파 값 저장
                ),
                id = field.Value.ID
            });
        }

        return JsonUtility.ToJson(saveData);
    }

    // Helper 함수: 위치에서 Renderer 가져오기
    private Renderer GetRendererFromPositions(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up, Vector3.down, out hit, 10f))
        {
            return hit.collider.GetComponent<Renderer>();
        }
        return null;
    }


    public void LoadFieldsAndCrops(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        GridDataSave saveData = JsonUtility.FromJson<GridDataSave>(json);

        placedCrops.Clear();
        placedFields.Clear();

        // 작물 복원
        foreach (var cropSave in saveData.crops)
        {
            Vector3Int intPosition = Vector3Int.FloorToInt(cropSave.position);
            placedCrops[intPosition] = cropSave.placementData;

            // 알파 값 복원
            Renderer renderer = GetRendererFromPositions(cropSave.position);
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                color.a = cropSave.placementData.alpha;
                material.color = color;
            }

            Debug.Log($"Loading crop data at position: {intPosition}, ID: {cropSave.id}");
        }

        // 밭 복원
        foreach (var fieldSave in saveData.fields)
        {
            Vector3Int intPosition = Vector3Int.FloorToInt(fieldSave.position);
            placedFields[intPosition] = fieldSave.placementData;

            // 알파 값 복원
            Renderer renderer = GetRendererFromPositions(fieldSave.position);
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                color.a = fieldSave.placementData.alpha;
                material.color = color;
            }

            Debug.Log($"Loading field data at position: {intPosition}, ID: {fieldSave.id}");
        }

        Debug.Log($"Loaded Field Positions: {string.Join(", ", placedFields.Keys)}");
        Debug.Log($"Loaded Crop Positions: {string.Join(", ", placedCrops.Keys)}");
    }

}

[System.Serializable]
public class PlacementData      // 오브젝트의 정보 클래스
{
    // 오브젝트가 차지하는 그리드 셀들의 좌표 (오브젝트 크기가 커서 여러 칸을 차지할 수 있어서 리스트로)
    public List<Vector3Int> occupiedPositions;
    public int ID { get; set; }                 // ObjectsDataBase Scriptable Object 에서 사용되는 오브젝트의 고유 ID
    public int PlacedObjectIndex { get; private set; }  // ObjectsDataBase Scriptable Object 에서 오브젝트의 인덱스

    // 작물 상태 추가
    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public float alpha;

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex,
                        CropState cropState = CropState.NeedsWater, SeedPlantedState seedPlantedState = SeedPlantedState.No,
                        float alpha = 1.0f)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        this.cropState = cropState;
        this.seedPlantedState = seedPlantedState;
        this.alpha = alpha;
    }

    // JSON 저장을 위해 기본 생성자 추가
    public PlacementData() { }
}

[System.Serializable]
public class GridDataSave
{
    public List<GridCropSave> crops = new();
    public List<GridFieldSave> fields = new(); // 밭 데이터 추가
}

[System.Serializable]
public class GridFieldSave
{
    public Vector3 position;       // 밭 위치
    public PlacementData placementData; // PlacementData 정보
    public int id;
}

[System.Serializable]
public class GridCropSave
{
    public Vector3 position;       // 작물 위치
    public PlacementData placementData; // PlacementData 정보
    public int id;
    public int currentStage; // 성장 단계 추가
    public Crop.CropState cropState; // 작물 상태 추가
}