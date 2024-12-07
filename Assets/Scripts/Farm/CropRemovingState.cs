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
            // ������ ������Ʈ�� �������� ������ �˷��ִ� ���� ���
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
        cropPlacer.RemoveObjectAt(gameObjectIndex); // ���� �ʿ�
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    private bool IsCropPlaced(Vector3Int gridPosition)  // �ش� ��ġ�� �۹��� �����ϸ� true
    {
        return placedCropData.placedCrops.ContainsKey(gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity;
        if (IsCropPlaced(gridPosition))    // �ش� ��ġ�� �۹��� ������
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