using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropPlacementState : IBuildingState
{
    private int selectedCropIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData placedCropData;
    OBJPlacer cropPlacer;

    public CropPlacementState(int id,
                              Grid grid,
                              PreviewSystem previewSystem,
                              ObjectsDatabaseSO database,
                              GridData placedCropData,
                              OBJPlacer cropPlacer)
    {
        ID = id;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.placedCropData = placedCropData;
        this.cropPlacer = cropPlacer;

        selectedCropIndex = database.objectsData.FindIndex(data => data.ID == ID);    // 설치할 작물 선택

        if (selectedCropIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedCropIndex].Prefab,
                database.objectsData[selectedCropIndex].Size);
        }
        else
        {
            throw new System.Exception($"No crop with ID {id}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (!IsFieldPlaced(gridPosition) || IsCropPlaced(gridPosition))    // 해당 위치에 밭이 없거나 작물이 있으면
        {
            return;
        }

        // 그리드 위치에 존재하는 밭 오브젝트를 가져옴
        GameObject fieldObject = GetFieldObjectAt(gridPosition);

        if (fieldObject == null)
        {
            Debug.LogWarning("해당 위치에 밭 오브젝트가 없습니다.");
            return;
        }

        int index = cropPlacer.PlaceObject(database.objectsData[selectedCropIndex].Prefab, grid.CellToWorld(gridPosition));

        // CropGrowthManager에 등록
        GameObject placedCrop = cropPlacer.placedGameObjects[index];    // 설치된 오브젝트 가져오기

        if (placedCrop != null)
        {
            placedCrop.transform.SetParent(fieldObject.transform);
        }

        Crop cropScript = placedCrop.GetComponent<Crop>();              // Crop 스크립트 가져오기

        if (cropScript != null)
        {
            cropScript.Initialize(database.objectsData[selectedCropIndex].GrowthTimes);

            // 초기화 이후 씨앗을 심어 상태를 설정
            cropScript.PlantSeed();

            // CropGrowthManager에 등록
            CropGrowthManager.Instance.RegisterCrop(cropScript);
        }

        placedCropData.AddCropAt(gridPosition, database.objectsData[selectedCropIndex].Size,
            database.objectsData[selectedCropIndex].ID, index);

        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    // 그리드 좌표에 밭 오브젝트가 있을 경우 그 오브젝트를 반환하는 메서드
    public GameObject GetFieldObjectAt(Vector3Int gridPosition)
    {
        if (placedCropData.placedFields.TryGetValue(gridPosition, out PlacementData placement))
        {
            int objIndex = placement.PlacedObjectIndex;

            // 인덱스가 유효한 경우 OBJList에서 오브젝트를 반환
            if (objIndex > -1 && objIndex < cropPlacer.placedGameObjects.Count)
            {
                return cropPlacer.placedGameObjects[objIndex];
            }
        }

        return null; // 위치에 밭 오브젝트가 없거나 유효하지 않으면 null 반환
    }

    // 그리드 좌표에 존재하는 오브젝트가 차지하는 모든 그리드 좌표 리스트를 반환하는 메서드
    // 인데 사용하지는 않을 것 같음 근데 나중에 필요할수도 있을 것 같아서 일단 냅둠
    private List<Vector3Int> GetFieldPositions(Vector3Int gridPosition)
    {
        if (placedCropData.placedFields.ContainsKey(gridPosition))
        {
            PlacementData placementData = placedCropData.placedFields[gridPosition];

            if (placementData == null)
            {
                Debug.LogError("PlacementData is null!");
                return null;
            }

            List<Vector3Int> positions = placementData.occupiedPositions;

            return positions;
        }

        return null;
    }

    private bool IsFieldPlaced(Vector3Int gridPosition) // 해당 위치에 밭이 존재하면 true
    {
        return placedCropData.placedFields.ContainsKey(gridPosition);
    }

    private bool IsCropPlaced(Vector3Int gridPosition)  // 해당 위치에 작물이 존재하면 true
    {
        return placedCropData.placedCrops.ContainsKey(gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity;
        if (!IsFieldPlaced(gridPosition) || IsCropPlaced(gridPosition))    // 해당 위치에 밭이 없거나 작물이 있으면 = 심을 수 없으면
        {
            placementValidity = false;
        }
        else
        {
            placementValidity = true;
        }
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), placementValidity);
    }
}