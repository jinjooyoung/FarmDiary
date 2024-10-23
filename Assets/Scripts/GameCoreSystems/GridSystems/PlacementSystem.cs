using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    /*[SerializeField]
    private AudioSource source;     // 설치 사운드 소스*/

    public GridData placedOBJData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private OBJPlacer objectPlacer;

    private int ID = -1;


    IBuildingState buildingState;

    private void Awake()
    {
        StopPlacement();
        placedOBJData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);              // 전체 그리드 UI
        this.ID = ID;
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           placedOBJData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // 설치 중일때 항상 입력을 받을 수 있도록 이벤트에 할당
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, placedOBJData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // 삭제 중일때 항상 입력을 받을 수 있도록 이벤트에 할당
        inputManager.OnExit += StopPlacement;
    }

    public void StartCropPlant(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);              // 전체 그리드 UI
        buildingState = new CropPlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           placedOBJData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // 설치 중일때 항상 입력을 받을 수 있도록 이벤트에 할당
        inputManager.OnExit += StopPlacement;
    }

    public void StartCropRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new CropRemovingState(grid, preview, placedOBJData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // 삭제 중일때 항상 입력을 받을 수 있도록 이벤트에 할당
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())     // UI 클릭중 게임 오브젝트 상호작용 무시 (UI를 클릭할 때 클릭한 위치의 뒤에 무언가 설치가 되지 않도록)
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();      // 현재 마우스 위치
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // 현재 그리드 위치

        int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        // 설치할 오브젝트가 차지하는 칸의 필드 ID가 담긴 리스트 (1개에서 2개니까)
        List<int> IDs = GetFieldIDs(gridPosition, database.objectsData[selectedObjectIndex].Size);

        if (IDs.Count == 1)
        {
            if (!fieldManager.IsFieldUnlocked(IDs[0]))
            {
                preview.ApplyFeedbackToCursor(false);
                return;
            }
        }
        else
        {
            if (!fieldManager.IsFieldUnlocked(IDs[0]) || !fieldManager.IsFieldUnlocked(IDs[1]))
            {
                preview.ApplyFeedbackToCursor(false);
                return;
            }
        }

        buildingState.OnAction(gridPosition);       // 해당 그리드 위치에 오브젝트 설치 (실제 구현)
    }

    private int GetFieldIDFromPosition(Vector3Int position)
    {
        int x = position.x;

        // 중앙 필드 ID는 0
        if (x >= -18 && x <= 17)
            return 0;

        // 왼쪽 필드 ID 계산 (ID : -1은 가로가 9칸, 이후는 10칸씩)
        if (x < -18)
            return -((Mathf.Abs(x) - 19) / 10 + 1);

        // 오른쪽 필드 ID 계산 (ID : 1은 가로가 9칸, 이후는 10칸씩)
        if (x > 17)
            return (x - 18) / 10 + 1;

        return 0;  // 기본값 (필요한 경우 조정)
    }

    private List<int> GetFieldIDs(Vector3Int mouseGridPosition, Vector2Int objectSize)
    {
        // 마우스 위치로부터 좌표 리스트를 계산
        List<Vector3Int> positions = CalculatePositions(mouseGridPosition, objectSize);

        HashSet<int> uniqueFieldIDs = new HashSet<int>();

        foreach (var position in positions)
        {
            int fieldID = GetFieldIDFromPosition(position);
            uniqueFieldIDs.Add(fieldID); // 중복이 자동으로 제거
        }

        // HashSet을 List로 변환하여 반환
        return uniqueFieldIDs.ToList();
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

    private void StopPlacement()
    {
        if (buildingState == null)      // 이미 설치가 종료 상태이면 리턴
        {
            return;
        }
        gridVisualization.SetActive(false);         // 그리드 UI 끔
        buildingState.EndState();                   // 프리뷰 종료
        inputManager.OnClicked -= PlaceStructure;   // 설치 상태가 종료되었으므로 이벤트 할당 해제
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)    // 선택된 오브젝트가 없으면 리턴
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)       // 불필요한 업데이트 방지를 위해 마우스가 움직였을 때만 업데이트
        {
            int fieldID = GetFieldIDFromPosition(gridPosition);

            if (!fieldManager.IsFieldUnlocked(fieldID))
            {
                preview.ApplyFeedbackToCursor(false);
            }
            buildingState.UpdateState(gridPosition);    // 프리뷰 오브젝트 스테이트 업데이트
            lastDetectedPosition = gridPosition;
        }
    }
}
