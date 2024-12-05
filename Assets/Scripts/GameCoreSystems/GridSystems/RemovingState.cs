using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData placedOBJData;
    OBJPlacer objectPlacer;

    public RemovingState(Grid grid, PreviewSystem previewSystem, GridData placedOBJData, OBJPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.placedOBJData = placedOBJData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Debug.LogWarning("���� Ŭ�� ȣ���");

        GridData selectedData = null;
        // false = �̹� ������Ʈ�� �����Ѵٴ� �ǹ� ��, ������ �� �� �ִ� ��ġ�� ��
        if (placedOBJData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = placedOBJData;
            Debug.LogWarning("���� ������ �Ҵ�");
        }

        if (selectedData == null)
        {
            // ������ ������Ʈ�� �������� ������ �˷��ִ� ���� ���
            Debug.LogWarning("���� ������Ʈ ���� X");
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);    // �� ��ġ�� ������Ʈ�� �ε����� ������
            if (gameObjectIndex == -1)      // ������ ������Ʈ�� �������� ���� ��
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);      // ������Ʈ�� ���� ����
            objectPlacer.RemoveObjectAt(gameObjectIndex);   // ������Ʈ ��ü�� �ı�
            Debug.LogWarning("���� �Ϸ�");
            selectedData.currentDictionary = -1;
        }
        Vector3 cellPos = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePreviewOBJPos(cellPos, CheckIfSelectionIsValid(gridPosition));
    }

    // ��ġ�� �����ϸ� false��, �Ұ����ϸ� true�� ��ȯ�ϴ� �޼���
    // RemoveState�� �� Ŀ���� ������ �ݴ�� ��Ÿ���� ���ؼ�
    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(placedOBJData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    // PlacementState ��ũ��Ʈ�� UpdateState�� ���� ��������� Ŀ�� ���� �ݴ�
    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), validity);
    }
}
