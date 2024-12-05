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
        Debug.LogWarning("삭제 클릭 호출됨");

        GridData selectedData = null;
        // false = 이미 오브젝트가 존재한다는 의미 즉, 삭제를 할 수 있는 위치일 때
        if (placedOBJData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = placedOBJData;
            Debug.LogWarning("삭제 데이터 할당");
        }

        if (selectedData == null)
        {
            // 삭제할 오브젝트가 존재하지 않음을 알려주는 사운드 재생
            Debug.LogWarning("삭제 오브젝트 존재 X");
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);    // 그 위치의 오브젝트의 인덱스를 가져옴
            if (gameObjectIndex == -1)      // 삭제할 오브젝트가 존재하지 않을 때
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);      // 오브젝트의 정보 삭제
            objectPlacer.RemoveObjectAt(gameObjectIndex);   // 오브젝트 자체를 파괴
            Debug.LogWarning("삭제 완료");
            selectedData.currentDictionary = -1;
        }
        Vector3 cellPos = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePreviewOBJPos(cellPos, CheckIfSelectionIsValid(gridPosition));
    }

    // 설치가 가능하면 false를, 불가능하면 true를 반환하는 메서드
    // RemoveState일 때 커서의 색깔을 반대로 나타내기 위해서
    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(placedOBJData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    // PlacementState 스크립트의 UpdateState와 같은 기능이지만 커서 색깔만 반대
    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), validity);
    }
}
