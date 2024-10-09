using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    /*[SerializeField]
    private AudioSource source;     // 설치 사운드 소스*/

    private GridData placedOBJData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private OBJPlacer objectPlacer;

    IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        placedOBJData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);              // 전체 그리드 UI
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

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())     // UI 클릭중 게임 오브젝트 상호작용 무시 (UI를 클릭할 때 클릭한 위치의 뒤에 무언가 설치가 되지 않도록)
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();      // 현재 마우스 위치
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // 현재 그리드 위치

        buildingState.OnAction(gridPosition);       // 해당 그리드 위치에 오브젝트 설치 (실제 구현)
    }

    // 선택된 (설치할 예정의) 오브젝트의 사이즈를 받아와서 CanPlaceObjectAt로 넘겨, 설치 가능한지 bool값 리턴
    // PlacementState 스크립트로 이동한 메서드. 하지만 나중에 다시 옮겨올수도 있을 것 같아서 일단 주석으로 둠
    /*private bool GetPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return placedOBJData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }*/

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
            buildingState.UpdateState(gridPosition);    // 프리뷰 오브젝트 스테이트 업데이트
            lastDetectedPosition = gridPosition;
        }
    }
}
