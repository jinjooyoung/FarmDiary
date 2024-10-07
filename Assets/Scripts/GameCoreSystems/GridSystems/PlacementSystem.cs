using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;       // 아무것도 선택되지 않았을 때 기본 인덱스

    [SerializeField]
    private GameObject gridVisualization;

    /*[SerializeField]
    private AudioSource source;     // 설치 사운드 소스*/

    private GridData furnitureData;

    private List<GameObject> placedGameObjects = new();

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private void Start()
    {
        StopPlacement();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        // FindIndex 메서드 : 리스트에서 조건을 만족하는 첫 번째 요소의 인덱스를 찾는 메서드
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        // 주어진 ID와 동일한 ID를 가진 오브젝트 데이터의 인덱스를 찾아서 변수에 저장

        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"Can't find index {ID}");
            return;
        }
        gridVisualization.SetActive(true);              // 전체 그리드 UI
        preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
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

        bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }

        //source.Play();        // 설치할 때 사운드 재생
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);

        SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        preview.SetAlpha(newObjectRenderer, 1.0f);

        placedGameObjects.Add(newObject);
        GridData selectedData = furnitureData;
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID, placedGameObjects.Count - 1);

        preview.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    private bool GetPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return furnitureData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);

            mouseIndicator.transform.position = mousePosition;
            preview.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
    }
}
