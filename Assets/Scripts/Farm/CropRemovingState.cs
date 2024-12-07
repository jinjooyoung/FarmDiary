using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropRemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData placedCropData;
    OBJPlacer cropPlacer;

    public CropRemovingState(Grid grid, PreviewSystem previewSystem, GridData placedCropData, OBJPlacer cropPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.placedCropData = placedCropData;
        this.cropPlacer = cropPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (IsCropPlaced(gridPosition))
        {
            selectedData = placedCropData;
        }

        if (selectedData == null)
        {
            // 삭제할 오브젝트가 존재하지 않음을 알려주는 사운드 재생
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndexCrop(gridPosition);
        }

        if (gameObjectIndex == -1)
        {
            return;
        }
        placedCropData.RemoveCropAt(gridPosition);
        cropPlacer.RemoveObjectAt(gameObjectIndex); // 수정 필요
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    private bool IsCropPlaced(Vector3Int gridPosition)  // 해당 위치에 작물이 존재하면 true
    {
        return placedCropData.placedCrops.ContainsKey(gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity;
        if (IsCropPlaced(gridPosition))    // 해당 위치에 작물이 있으면
        {
            validity = true;
        }
        else
        {
            validity = false;
        }
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), validity);
    }
}